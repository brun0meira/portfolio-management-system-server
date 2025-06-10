# 📊 API Financeira - Documentação de Endpoints

Esta API gerencia operações financeiras (trades), cálculo de P\&L, registro de proventos, preços médios e rankings de clientes.

## 1. UserController

**Base Route:** `/api/user`

| Método | Rota                                       | Descrição                                                 |
| ------ | ------------------------------------------ | --------------------------------------------------------- |
| GET    | `/api/user/{userId}/operations`            | Lista todas as operações de compra e venda do usuário.    |
| GET    | `/api/user/{userId}/positions`             | Retorna todas as posições consolidadas do usuário.        |
| GET    | `/api/user/{userId}/positions/{assetCode}` | Retorna a posição de um ativo específico.                 |
| GET    | `/api/user/{userId}/brokerage-total`       | Retorna o total de corretagem paga pelo usuário.          |
| GET    | `/api/user/{userId}/provisions`            | Lista os proventos registrados do usuário.                |
| GET    | `/api/user/{userId}/pnl-history`           | Histórico de P\&L (lucro/prejuízo) do usuário.            |
| POST   | `/api/user/{userId}/operations`            | Registra uma nova operação de compra ou venda.            |
| POST   | `/api/user/{userId}/dividends`             | Registra um novo dividendo/JCP para o usuário.            |
| POST   | `/api/user/{userId}/simulate-operation`    | Simula o impacto de uma operação no P\&L e na posição.    |
| GET    | `/api/user/summary?year={y}&month={m}`     | Retorna um resumo mensal de P\&L, corretagem e proventos. |
| POST   | `/api/user/create`                         | Cria um novo usuário.                                     |


## 2. AssetsController

**Base Route:** `/api/assets`

| Método | Rota                                                       | Descrição                                     |
| ------ | ---------------------------------------------------------- | --------------------------------------------- |
| GET    | `/api/assets/`                                             | Lista todos os ativos disponíveis.            |
| GET    | `/api/assets/{assetSymbol}/quote`                          | Retorna a última cotação de um ativo.         |
| GET    | `/api/assets/{assetSymbol}/history?period={p}`             | Histórico de preços (7d, 30d, 1y, 5y).        |
| GET    | `/api/assets/{assetSymbol}/volatility?period={p}`          | Calcula a volatilidade do ativo.              |
| POST   | `/api/assets/asset/create`                                 | Registra um novo ativo.                       |
| GET    | `/api/assets/average-price/asset?userId={id}&assetId={id}` | Retorna o preço médio ponderado de aquisição. |


## 3. ReportsController

**Base Route:** `/api/reports`

| Método | Rota                                              | Descrição                                              |
| ------ | ------------------------------------------------- | ------------------------------------------------------ |
| GET    | `/api/reports/top-clients-by-position?limit={n}`  | Top N usuários com maior valor de posição consolidada. |
| GET    | `/api/reports/top-clients-by-brokerage?limit={n}` | Top N usuários que mais pagaram corretagem.            |

## 3. Status HTTP Comuns

| Código | Significado                             |
| ------ | --------------------------------------- |
| 200    | OK - Requisição bem-sucedida            |
| 400    | Bad Request - Dados inválidos           |
| 404    | Not Found - Recurso não encontrado      |
| 500    | Internal Server Error - Erro inesperado |

## 4. Escoboço da API

A API foi esboçada tanto de forma visual, por meio do Swagger dos controllers da própria Web API, quanto em formato de arquivo `.yaml`, que você pode encontrar [aqui](../files/Api.yaml).
