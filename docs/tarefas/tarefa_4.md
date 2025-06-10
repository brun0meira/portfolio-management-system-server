## Cálculo do Preço Médio Ponderado de Aquisição

O **preço médio ponderado de aquisição** (PMPA) é a métrica que representa o custo médio unitário que um investidor pagou por determinado ativo, considerando todas as operações de compra já realizadas. Ou seja, o custo total ponderado por cada lote adquirido, dividido pelo volume total em carteira.

### 1. Endpoint HTTP

```csharp
[HttpGet("average-price/asset")]
public async Task<ActionResult<AveragePriceResponseDto>> GetAveragePrice([FromQuery] Guid userId, [FromQuery] Guid assetId)

```

Esse endpoint disponibiliza, via HTTP GET, a funcionalidade de obtenção do preço médio de um ativo. Os parâmetros são `userId` e `assetId`, representando respectivamente o investidor e o ativo em questão.

A resposta segue o padrão `ActionResult<T>`, permitindo tratamento tanto para retornos bem-sucedidos (`200 OK`) quanto para exceções (`400 Bad Request`).

### 2. Implementação do Método de Negócio

```csharp
public async Task<decimal> CalculateWeightedAveragePriceAsync(Guid userId, Guid assetId)

```

A lógica central reside aqui. A função realiza os seguintes passos críticos:

1. **Consulta de dados**: utiliza o repositório [ITradeRepository](../../Domain/Interfaces/ITradeRepository.cs) para obter todas as operações de compra (`TradeType.Buy`) do usuário para o ativo especificado.
2. **Validações de entrada**:
    - Se nenhuma compra for encontrada, lança exceção (`InvalidOperationException`) com mensagem específica.
    - Valida se os dados de cada operação estão íntegros: quantidade positiva e preço unitário positivo. Lançar exceção nesses casos evita distorções nos cálculos e garante consistência no modelo de domínio.
3. **Cálculo acumulado**:
    - Soma dos produtos entre `UnitPrice` e `Quantity`, somando ainda o valor da `Fee` (corretagem), que deve ser incorporada ao custo total de aquisição.
    - Acumula a `Quantity` total.
4. **Divisão segura**:
    - A operação de divisão final só ocorre se a `totalQuantity` for maior que zero, evitando erros de divisão por zero e reforçando a segurança da lógica.
---

### Considerações Técnicas Adicionais

### Tratamento de Corretagem

A incorporação da **corretagem (Fee)** no cálculo do custo total é fundamental para que o preço médio represente fielmente o valor efetivamente desembolsado pelo investidor. Em cenários reais, a corretagem pode variar entre operações e precisa ser somada ao custo de aquisição individual de cada lote.

### Garantia de Integridade dos Dados

O uso de exceções específicas para tratar dados inconsistentes nas operações (`Quantity <= 0`, `UnitPrice <= 0`) garante confiabilidade à lógica e previne cenários em que dados corrompidos possam mascarar perdas ou superestimar lucros.

### Performance e Escalabilidade

Embora o método percorra todas as compras do ativo, ele se mantém eficiente pela utilização de consultas filtradas diretamente no banco, retornando apenas os dados relevantes. Além disso, por operar sobre listas em memória e não envolver cálculos estatísticos complexos, o algoritmo mantém performance linear (O(n)) em relação ao número de operações, sendo plenamente escalável mesmo para usuários com grande volume de transações.

---

### Vantagens e Limitações

### Vantagens:

- Implementação robusta e extensível.
- Separação clara de responsabilidades e alta coesão.
- Cálculo financeiro preciso, incorporando custos operacionais.
- Tratamento completo de exceções e falhas de entrada.

### Limitações:

- Atualmente restrito a operações de **compra** (não considera agrupamento de múltiplas contas ou transferências de custódia).
- Não realiza diferenciação de lotes por data (não há controle FIFO/LIFO), embora isso seja desnecessário para o preço médio consolidado.
- Dependente da integridade dos dados no banco de origem — não há fallback para dados corrompidos ou incompletos.