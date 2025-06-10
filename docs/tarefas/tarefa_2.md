### Índices e Performance no Controle de Investimentos

Um sistema de controle de investimentos em renda variável precisa lidar com grandes volumes de dados transacionais com alta frequência de leitura e atualização. Isso exige um sistema que forneça respostas rápidas a consultas relacionadas a operações recentes dos usuários, bem como uma estrutura eficiente para atualizar, em tempo quase real, as **posições consolidadas** de cada cliente, com foco em métricas como **quantidade**, **preço médio** e **P&L (Profit and Loss)**.

### Proposição e Justificativa de Índices

A primeira demanda é a consulta de todas as operações de um determinado usuário em um ativo específico nos últimos 30 dias. A modelagem relacional proposta para a tabela `Trades` já contempla colunas críticas como `UserId`, `AssetId` e `TradeTime`, que são exatamente os filtros utilizados na cláusula `WHERE` da consulta de negócio. Assim, a criação do índice composto:

```sql
CREATE INDEX idx_trade_user_asset_time ON Trades (UserId, AssetId, TradeTime);

```

é uma decisão técnica adequada e essencial. Este índice é particularmente eficiente por três motivos:

1. **Selectividade progressiva**: o índice é construído na ordem dos filtros da cláusula `WHERE`, permitindo que o otimizador do MySQL faça uso da filtragem composta.
2. **Otimização de range temporal**: o campo `TradeTime` aparece por último, o que facilita a execução de consultas que envolvam uma faixa de datas (ex: últimos 30 dias).
3. **Aproveitamento em `ORDER BY`**: o índice também favorece a ordenação por `TradeTime`, permitindo leitura ordenada diretamente pelo índice (index range scan).

Além disso, a tabela `Quotes`, que armazena as cotações dos ativos, precisa também suportar buscas eficientes pelo par `(AssetId, QuoteTime)`, geralmente buscando a cotação mais recente. O índice:

```sql
CREATE INDEX idx_quote_asset_time ON Quotes (AssetId, QuoteTime);

```

permite que o banco execute consultas do tipo `ORDER BY QuoteTime DESC LIMIT 1` de forma altamente performática, sem exigir um `filesort`.

### Consulta SQL Otimizada

A seguinte consulta permite recuperar de forma eficiente todas as operações de um determinado usuário em um ativo nos últimos 30 dias, com ordenação decrescente por data:

```sql
SELECT
    T.Id,
    T.Quantity,
    T.UnitPrice,
    T.TradeType,
    T.Fee,
    T.TradeTime
FROM Trades T
WHERE
    T.UserId = 'USER-ID-AQUI'
    AND T.AssetId = 'ASSET-ID-AQUI'
    AND T.TradeTime >= NOW() - INTERVAL 30 DAY
ORDER BY T.TradeTime DESC;

```

Essa estrutura aproveita o índice `idx_trade_user_asset_time`, garantindo que o banco filtre pelas colunas `UserId` e `AssetId` de forma eficaz, restringindo os registros por `TradeTime` e ordenando sem custo adicional.

### Atualização da Posição com Base em Cotação

Em um sistema real-time ou near-real-time, as posições dos clientes devem refletir instantaneamente qualquer variação de preço no mercado. Para isso, propõe-se uma **atualização da tabela `Positions` baseada na última cotação disponível e nas operações realizadas**. O cálculo leva em consideração:

- A **quantidade líquida** de ativos (compras menos vendas).
- O **preço médio ponderado**, calculado apenas sobre operações de compra.
- O **P&L**, que é o valor potencial de lucro ou prejuízo considerando o preço médio e a última cotação.

A seguinte instrução SQL representa esse processamento de forma integrada e eficiente:

```sql
UPDATE Positions P
JOIN (
    SELECT
        T.UserId,
        T.AssetId,
        SUM(CASE WHEN T.TradeType = 1 THEN T.Quantity ELSE -T.Quantity END) AS TotalQty,
        SUM(CASE WHEN T.TradeType = 1 THEN (T.Quantity * T.UnitPrice) + T.Fee ELSE 0 END) AS TotalCost
    FROM Trades T
    WHERE T.UserId = 'USER-ID-AQUI'
      AND T.AssetId = 'ASSET-ID-AQUI'
    GROUP BY T.UserId, T.AssetId
) AS Calc ON Calc.UserId = P.UserId AND Calc.AssetId = P.AssetId
JOIN (
    SELECT AssetId, UnitPrice AS LastPrice
    FROM Quotes
    WHERE AssetId = 'ASSET-ID-AQUI'
    ORDER BY QuoteTime DESC
    LIMIT 1
) AS Q ON Q.AssetId = P.AssetId
SET
    P.Quantity = Calc.TotalQty,
    P.AvgPrice = IF(Calc.TotalQty > 0, Calc.TotalCost / Calc.TotalQty, 0),
    P.PnL = (Q.LastPrice - P.AvgPrice) * P.Quantity;

```

**Explicações relevantes**:

- O `CASE WHEN T.TradeType = 1` distingue entre operações de **compra** (tipo 1) e **venda** (tipo 0 ou 2). Apenas compras influenciam no preço médio.
- A divisão de `TotalCost / TotalQty` evita distorções no preço médio. Em casos de liquidação total do ativo (TotalQty = 0), evita-se divisão por zero.
- O `PnL` reflete o impacto imediato de uma cotação atualizada, sendo essencial para análises de rentabilidade e gestão de risco.