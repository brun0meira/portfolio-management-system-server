# Documentação Técnica - Portfolio Management System

## Links Úteis

* **WebAPI (Swagger):** [Acesse aqui](https://testedeploy-dyjw.onrender.com/swagger/index.html)
* **Frontend (Interface Web):** [Acesse aqui](https://portfolio-management-system-client.vercel.app/)

### Informações adicionais

* O **backend** está hospedado em um serviço gratuito (Render), que pode sofrer **spin down por inatividade**. Isso significa que, caso o serviço fique inativo por algum tempo, ele pode levar alguns segundos extras para responder à **primeira requisição** após o período ocioso. Esse comportamento é esperado.
* Os **dados presentes na API (Swagger)** são **totalmente fictícios** e foram criados exclusivamente para fins de demonstração e estudo. Nenhuma informação representa dados reais ou de mercado.
* O código-fonte do **frontend** está em um repositório separado. Você pode acessá-lo aqui:
  👉 [Repositório do Frontend no GitHub](https://github.com/brun0meira/portfolio-management-system-client)
* Os **códigos** e as **explicações** de cada tarefa específica estão disponíveis no seguinte diretório: [docs/tarefas](./docs/tarefas/).
  Acesse-o para obter mais **detalhes sobre a implementação** de cada etapa do projeto.

## Visão Geral

O **Portfolio Management System** é uma aplicação desenvolvida em **.NET Core** que calcula e fornece dados financeiros estratégicos para usuários investidores, abordando conceitos como ativos, operações de compra e venda, dividendos, P&L, entre outros, como:

- Total investido por ativo;
- Posição individual por papel (ação);
- Posição global do investidor (com lucro/prejuízo);
- Total de corretagem por cliente.

## Contexto

Renda variável é uma modalidade de investimento em que os retornos não são fixos ou previsíveis, diferentemente da renda fixa. A principal característica desse tipo de investimento é a oscilação de preços, que pode gerar ganhos ou perdas.

Ativos como ações, fundos imobiliários (FIIs) e ETFs fazem parte deste universo. Este projeto simula a jornada de um investidor, permitindo o registro de operações, o cálculo de indicadores financeiros e a visualização de posições.

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
- [C#](https://dotnet.microsoft.com/pt-br/languages/csharp)
- [MySQL](https://www.mysql.com/)
- [Kafka](https://kafka.apache.org/)
- [React](https://react.dev/)

---