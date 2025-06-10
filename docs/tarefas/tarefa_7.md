## Integração entre Sistemas – WorkerService para Consumo Assíncrono de Mensagens com Kafka

A estrutura do `WorkerService` é necessário em um cenário onde o acompanhamento e a persistência de cotações de ativos e dividendos são essenciais para alimentar sistemas de precificação, projeções de lucro e cálculo de rentabilidade de carteiras de clientes. Com base na volatilidade das informações envolvidas – como **preços de ativos** e **anúncios de proventos** (dividendos e juros sobre capital próprio), a implementação de um sistema reativo e resiliente torna-se um diferencial crítico.

O serviço implementado é dividido em dois **workers** distintos que escutam tópicos separados do Kafka: um responsável pelas **cotações de ativos** (`QuoteWorker`) e outro pelos **dividendos** (`DividendWorker`).



### Arquitetura da WorkerService
---
A arquitetura baseia-se no padrão de **consumo resiliente e desacoplado** de mensagens Kafka, com isso podemos escalar os consumidores de maneira independente e manter o controle sobre a **lógica de retry**, **validação de duplicidade** e **persistência assíncrona**.

As principais camadas envolvidas são:

* **Infraestrutura de Fila ([Infrastructure.Queue](../../Infrastructure.Queue/))**: responsável pela configuração, consumo e deserialização das mensagens Kafka.
* **Serviços de Domínio ([Domain.Interfaces](../../Domain/Interfaces/) / [Domain.Dto](../../Domain/Dto/))**: provê abstrações e DTOs que isolam regras de negócio da lógica de transporte.
* **Persistência ([Infrastructure.Data.Repositories](../../Infrastructure.Data/Repositories/))**: manipula o banco de dados com repositórios orientados por interfaces.
* **Orquestração ([WorkerService](../../WorkerService/))**: provê os workers e injeta os serviços por meio da DI (Dependency Injection).

### Padrões e Boas Práticas implementadas
---
#### 1. **Separação de Responsabilidades**

Cada componente possui responsabilidade única:

* Worker = executor contínuo
* ConsumerService = processamento de mensagens
* Repository = persistência

#### 2. **Injeção de Dependência**

Todos os serviços e configurações são injetados no `Startup.cs`, garantindo desacoplamento, facilidade de testes e centralização da configuração.

#### 3. **Reutilização de Código com Abstração Genérica**

A classe `KafkaConsumerService<TMessage>` permite reaproveitar lógica comum de consumo e desserialização, evitando duplicações entre cotação e dividendos.

#### 4. **Logging e Observabilidade**

Apesar do uso do `Console.WriteLine()` para efeitos de desenvolvimento, o local é adequado para injeção de uma ferramenta de logging robusta.