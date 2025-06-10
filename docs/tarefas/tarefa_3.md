# Documentação Técnica - Portfolio Management System

## Visão Geral

O **Portfolio Management System** é uma aplicação desenvolvida em **.NET Core** que calcula e fornece dados financeiros estratégicos para usuários investidores, como:

- Total investido por ativo;
- Posição individual por papel (ação);
- Posição global do investidor (com lucro/prejuízo);
- Total de corretagem por cliente.

A aplicação segue princípios de **arquitetura em camadas**, **separação de responsabilidades** e uso de **async/await** com Entity Framework para garantir escalabilidade e manutenção facilitada.

---

## Estrutura da Solução

```
portfolio-management-system-server
│
├── Application                          # Solução de aplicações (WebApi e Worker)
│   ├── WebApi                           # Projeto da API RESTful
│   │   └── Controllers/                 # Controladores que expõem os endpoints
│   └── WorkerService                    # Worker Service que processa dados assíncronos
│
├── Core                                 # Núcleo da aplicação (regras de negócio, entidades, DTOs)
│   └── Domain                           # Biblioteca de classes
│       ├── Business/                    # Camada de serviços com lógica de negócio
│       ├── Dto/                         # Objetos de transferência de dados (entrada/saída)
│       ├── Entities/                    # Entidades persistidas no banco de dados
│       ├── Enum/                        # Enumerações utilizadas pela aplicação
│       └── Interfaces/                  # Interfaces para abstração das dependências
│
├── Infrastructure                       # Implementações de persistência e mensageria
│   ├── Infrastructure.Data              # Projeto de infraestrutura de dados (EF Core)
│   │   ├── Migrations/                  # Migrations geradas para o banco de dados
│   │   └── Repositories/                # Repositórios concretos com acesso ao banco
│   └── Infrastructure.Queue             # Integração com sistemas de fila (ex: Kafka)
│       └── Services/                    # Serviços para leitura/escrita nas filas
│
└── Tests                                # Projeto de testes automatizados
    └── Tests                            # Projeto Xunit
        └── Business/                    # Testes de unidade das regras de negócio

```

---

## Tecnologias Utilizadas

- [.NET Core](https://dotnet.microsoft.com/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- C#
- [xUnit](https://xunit.net/)
- Kafka

---

## Boas Práticas Utilizadas

- **Separação de responsabilidades (SRP):** Cada camada tem sua responsabilidade clara.
- **Async/Await:** Todas as chamadas de banco e tarefas assíncronas são feitas com `async/await`.
- **Inversão de Dependência (IoC):** Interfaces definidas no Core e injetadas via DI.
- **Testabilidade:** Projeto de testes unitários com cobertura da lógica de negócio.
- **DTOs:** Separação clara entre modelos de domínio e dados trafegados na API.

---

## Funcionalidades Principais

### 1. Cálculo do Total Investido por Ativo

**Descrição:** Soma o total investido por código de ativo com base nas operações de compra.

**Método:** `GET /api/positions/{userId}/by-asset`

---

### 2. Posição por Papel de um Investidor

**Descrição:** Retorna a quantidade atual, o preço médio e o valor atual de cada ativo da carteira do usuário.

**Método:** `GET /api/positions/{userId}/detailed`

---

### 3. Posição Global com Lucro ou Prejuízo

**Descrição:** Retorna o total investido, o valor atual da carteira, e o resultado (lucro/prejuízo) geral.

**Método:** `GET /api/positions/{userId}/summary`

---

### 4. Total de Corretagem por Cliente

**Descrição:** Soma todas as taxas de corretagem pagas por um determinado usuário.

**Método:** `GET /api/users/{userId}/brokerage-fees`

## Como Rodar a Aplicação

```bash
dotnet build
dotnet ef database update --project Infrastructure.Data
dotnet run --project Application/WebApi

```

Para testes:

```bash
dotnet test

```