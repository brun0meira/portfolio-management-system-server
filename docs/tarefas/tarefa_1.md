```sql
CREATE TABLE Users (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    FeePercentage DECIMAL(5,2) NOT NULL
);

CREATE TABLE Assets (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    Code VARCHAR(50) NOT NULL,
    Name VARCHAR(255) NOT NULL
);

CREATE TABLE Trades (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    AssetId CHAR(36) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(15,4) NOT NULL,
    TradeType INT NOT NULL,
    Fee DECIMAL(15,4) NOT NULL,
    TradeTime DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (AssetId) REFERENCES Assets(Id),
    INDEX idx_trade_user_asset_time (UserId, AssetId, TradeTime),
    INDEX idx_trade_user (UserId)
);

CREATE TABLE Quotes (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    AssetId CHAR(36) NOT NULL,
    UnitPrice DECIMAL(15,4) NOT NULL,
    QuoteTime DATETIME NOT NULL,
    FOREIGN KEY (AssetId) REFERENCES Assets(Id),
    INDEX idx_quote_asset_time (AssetId, QuoteTime)
);

CREATE TABLE Positions (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    AssetId CHAR(36) NOT NULL,
    Quantity INT NOT NULL,
    AvgPrice DECIMAL(15,4) NOT NULL,
    PnL DECIMAL(15,4) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (AssetId) REFERENCES Assets(Id)
);

CREATE TABLE Dividends (
    Id CHAR(36) NOT NULL PRIMARY KEY,
    AssetId CHAR(36) NOT NULL,
    DividendType INT NOT NULL,
    ValuePerShare DECIMAL(15,4) NOT NULL,
    ExDate DATE NOT NULL,
    PaymentDate DATE NOT NULL,
    FOREIGN KEY (AssetId) REFERENCES Assets(Id)
);

```

---

## **1. Visão Geral do Modelo Relacional**

A proposta consiste na modelagem de um sistema de controle de investimentos com foco em ativos de renda variável. O modelo foi desenhado com base na necessidade de registrar usuários, ativos negociáveis, operações de compra e venda (trades), cotações históricas de preços, dividendos distribuídos, e o consolidado de posição do investidor (posição atual com cálculo de preço médio e P&L).

A seguir, detalha-se cada tabela com sua estrutura e justificativas técnicas para os tipos de dados escolhidos.

---

## **2. Tabela `users`**

```sql
CREATE TABLE users (
    id CHAR(36) NOT NULL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    fee_percentage DECIMAL(5,2) NOT NULL
);

```

### **Descrição**

Armazena informações básicas dos investidores, incluindo nome, e-mail e a taxa de corretagem individual associada ao usuário.

### **Justificativa Técnica**

- `id CHAR(36)`: UUIDs são utilizados para garantir unicidade global e evitar colisões, especialmente em sistemas distribuídos ou com possíveis sincronizações offline.
- `name VARCHAR(255)`: A escolha de 255 caracteres proporciona flexibilidade suficiente para nomes completos, respeitando padrões comuns.
- `email VARCHAR(255)`: Mesmo raciocínio do campo `name`, com suporte a emails longos.
- `fee_percentage DECIMAL(5,2)`: Permite armazenar taxas até 999.99%, sendo tecnicamente suficiente para cenários de corretagem. DECIMAL evita problemas de precisão típicos de floats.

---

## **3. Tabela `assets`**

```sql
CREATE TABLE assets (
    id CHAR(36) NOT NULL PRIMARY KEY,
    code VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL
);

```

### **Descrição**

Armazena informações dos ativos financeiros negociáveis. O campo `code` contém o ticker do ativo, como ITSA3, BOVA11 etc.

### **Justificativa Técnica**

- `code VARCHAR(50)`: Tickers podem conter letras, números e até códigos complexos em alguns casos. O tamanho de 50 é conservador e cobre variações de mercado nacional e internacional.
- `name VARCHAR(255)`: Usado para registrar a descrição do ativo (ex: Itaúsa PN N1).

---

## **4. Tabela `trades`**

```sql
CREATE TABLE trades (
    id CHAR(36) NOT NULL PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    asset_id CHAR(36) NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(15,4) NOT NULL,
    trade_type INT NOT NULL,
    fee DECIMAL(15,4) NOT NULL,
    trade_time DATETIME NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (asset_id) REFERENCES assets(id),
    INDEX idx_trade_user_asset_time (user_id, asset_id, trade_time),
    INDEX idx_trade_user (user_id)
);

```

### **Descrição**

Registra cada operação de compra ou venda de ativos realizada por um usuário. Contém todos os dados necessários para cálculo de posição, P&L e preço médio.

### **Justificativa Técnica**

- `unit_price DECIMAL(15,4)`: Precisão necessária para armazenar preços em mercado de capitais. Pode representar valores entre 0,0001 até trilhões com precisão adequada.
- `trade_type INT`: Representa Compra (1) ou Venda (2). Prefere-se INT ao invés de VARCHAR por questões de performance e integridade.
- `fee DECIMAL(15,4)`: Representa o custo da operação individual.
- Índices: Os índices criados aceleram buscas frequentes, como extrato de operações por usuário ou análise de histórico por ativo e período.

---

## **5. Tabela `quotes`**

```sql
CREATE TABLE quotes (
    id CHAR(36) NOT NULL PRIMARY KEY,
    asset_id CHAR(36) NOT NULL,
    unit_price DECIMAL(15,4) NOT NULL,
    quote_time DATETIME NOT NULL,
    FOREIGN KEY (asset_id) REFERENCES assets(id),
    INDEX idx_quote_asset_time (asset_id, quote_time)
);

```

### **Descrição**

Armazena a cotação histórica de cada ativo. Essencial para cálculo de P&L em tempo real e para geração de gráficos.

### **Justificativa Técnica**

- Cotações podem variar ao longo do dia. `quote_time` registra o instante da cotação.
- Índice composto por `asset_id` e `quote_time` é fundamental para consultas temporais (ex: última cotação do dia).

---

## **6. Tabela `positions`**

```sql
CREATE TABLE positions (
    id CHAR(36) NOT NULL PRIMARY KEY,
    user_id CHAR(36) NOT NULL,
    asset_id CHAR(36) NOT NULL,
    quantity INT NOT NULL,
    avg_price DECIMAL(15,4) NOT NULL,
    pnl DECIMAL(15,4) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (asset_id) REFERENCES assets(id)
);

```

### **Descrição**

Contém o consolidado de cada usuário por ativo. A posição reflete a quantidade total em carteira, o preço médio ajustado e o lucro/prejuízo atual.

### **Justificativa Técnica**

- Esta tabela pode ser recalculada em batch ou mantida atualizada via triggers e/ou lógica de aplicação, dependendo da arquitetura.
- `pnl`: Calculado como `(cotação_atual - avg_price) * quantidade`, usado para mensurar performance de cada investimento.

---

## **7. Tabela `dividends`**

```sql
CREATE TABLE dividends (
    id CHAR(36) NOT NULL PRIMARY KEY,
    asset_id CHAR(36) NOT NULL,
    dividend_type INT NOT NULL,
    value_per_share DECIMAL(15,4) NOT NULL,
    ex_date DATE NOT NULL,
    payment_date DATE NOT NULL,
    FOREIGN KEY (asset_id) REFERENCES assets(id)
);

```

### **Descrição**

Registra eventos de distribuição de proventos (dividendos, juros sobre capital próprio etc.). Fundamental para estratégias focadas em renda passiva e cálculo de retorno real.

### **Justificativa Técnica**

- `dividend_type INT`: Poderia ser mapeado como 1 = Dividendo, 2 = JCP. Facilitando o tratamento em queries e cálculos.
- `value_per_share`: Multiplicado pela quantidade em carteira do usuário na data ex para calcular o valor recebido.
- `ex_date`: Data que determina direito ao recebimento.
- `payment_date`: Quando o valor será efetivamente creditado.

---

## **8. Considerações Finais e Boas Práticas**

### **Integridade Referencial**

Todos os relacionamentos entre entidades são modelados com `FOREIGN KEY`, garantindo integridade dos dados mesmo em ambientes com múltiplas fontes de entrada.

### **Nomenclatura e Convenções**

Optou-se por `snake_case` nos nomes das colunas, conforme recomendado para padronização em sistemas legados e compatibilidade com ORMs.

### **Escalabilidade**

A utilização de `CHAR(36)` para chaves primárias permite escalabilidade horizontal com sincronização entre ambientes.

### **Performance**

Índices foram cuidadosamente posicionados nas tabelas com maior volume de acesso temporal (como `trades` e `quotes`) para garantir performance em consultas analíticas e relatórios.