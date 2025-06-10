## Escalabilidade e Performance

Com o aumento expressivo no volume de operações em renda variável, passando da marca de **1 milhão de operações por dia** é necessário que o sistema esteja pronto para escalar e manter a performance mesmo sob alta carga. Isso traz desafios tanto na infraestrutura quanto na lógica da aplicação, como é o caso do serviço [TradeBusiness](../../Domain/Business/TradeBusiness.cs).

### Escalabilidade Horizontal (Auto-Scaling)

Escalar horizontalmente significa rodar várias instâncias do serviço para dividir a carga. Esse modelo funcionaria muito bem para as operações independentes e *stateless* contidas no `TradeBusiness`.

#### Como aplicar

1. **Contêineres e orquestração**:

   * Usar **Docker** para empacotar o serviço.
   * Orquestrar com **Kubernetes**, ativando o *Horizontal Pod Autoscaler (HPA)* para ajustar os pods com base em CPU, memória ou métricas personalizadas como número de requisições na Api.

2. **Idempotência e desacoplamento**:

   * As operações precisam ser **idempotentes** para gerar o mesmo resultado mesmo se forem executadas mais de uma vez.
   * O `TradeBusiness` já está alinhado com esse conceito, usando padrão **Repository** e tratando estados de forma determinística.

3. **Mensageria assíncrona**:

   * Em momentos de pico, usar a fila **Kafka** para separar o recebimento da execução das operações.

5. **Resiliência com Polly**:

   * O uso de **circuit breakers** e **fallbacks** com Polly ajuda a evitar que falhas externas derrubem todo o sistema, principalmente quando temos várias instâncias rodando ao mesmo tempo.

---

### Balanceamento de Carga

Num ambiente com várias instâncias, é o **balanceador de carga** que tem a responsabilidade de distribuir as requisições entre elas. As duas estratégias que se destacam são:

#### 1. Round-Robin

Bem simples: as requisições são enviadas em ordem para as instâncias — A, B, C, D e depois volta pra A.

**Prós**:

* Fácil de implementar.
* Funciona bem quando todas as instâncias são parecidas.

**Contras**:

* Não considera o tempo de resposta real de cada instância.
* Pode sobrecarregar instâncias que já estão lentas.

**Indicado para**: sistemas com carga estável e instâncias homogêneas.

#### 2. Por Latência

Aqui o balanceador escolhe a instância mais rápida no momento da requisição (menor latência).

**Prós**:

* Usa melhor os recursos em tempo real.
* Ajuda a reduzir a latência percebida.

**Contras**:

* Mais complexo de implementar.
* Pode concentrar muita carga nas instâncias mais rápidas — então é bom ter *rate limiting* e *circuit breakers*.

**Indicado para**: sistemas com variações de carga e serviços críticos de tempo (como cotação ou ordens em tempo real).

---

### Pontos de atenção

* **Concorrência e transações**: como estamos lidando com dados financeiros (como `AvgPrice`, `Quantity`, `PnL`), é importante garantir transações seguras e evitar conflitos em acessos simultâneos.
* **Cache distribuído**: usar Redis ou Memcached pra guardar cotações e dados intermediários vai ajudar a aliviar o banco.
* **Observabilidade**:

  * **Prometheus + Grafana** para monitoramento.
  * **Tracing distribuído** com OpenTelemetry para analisar a performance.
