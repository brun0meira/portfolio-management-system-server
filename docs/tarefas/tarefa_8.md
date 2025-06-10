### Engenharia do Caos
---
No contexto da renda variável, onde oscilações de mercado são intensas e constantes, a resiliência do sistema se torna um diferencial crítico. A indisponibilidade não pode comprometer a continuidade dos processos de negócios, especialmente em ambientes de alta responsabilidade como o bancário. Por isso, a aplicação de princípios da Engenharia do Caos é fundamental.

### Aplicação de Circuit Breaker
---
O padrão **circuit breaker** tem como objetivo evitar chamadas contínuas a um serviço instável, protegendo a aplicação de falhas em cascata e ajudar no tempo de recuperação. No [TradeBusiness](../../Domain/Business/TradeBusiness.cs) o padrão é implementado com `Polly.CircuitBreakerAsync`, configurado com `handledEventsAllowedBeforeBreaking: 3` e `durationOfBreak: 30 segundos`.

Isso significa que, após 3 exceções consecutivas durante a tentativa de obtenção da última cotação (`_tradeRepository.GetLatestQuoteByAssetAsync`), o circuito entra em estado de “aberto” por 30 segundos. Durante esse tempo, todas as chamadas subsequentes falham imediatamente, sem tentativa de execução. Após esse intervalo, o circuito testa uma nova chamada. Se ela for bem-sucedida, o estado volta a "fechado" e caso contrário, retorna a "aberto".

Este mecanismo se torna essencial para:

* Evitar sobrecarga em um serviço já degradado;
* Proteger threads e recursos do sistema;
* Isolar falhas de forma controlada, permitindo recuperação gradual.

### Fallback: Degradação Controlada
---
Complementando o circuit breaker, a política de **fallback** implementada com `Policy.FallbackAsync<Quote>` define uma resposta padrão quando a chamada ao serviço de cotações falha. Então um `Quote` com `UnitPrice = 100` é retornado como valor de substituição.

O fallback não apenas permite continuidade da simulação ou criação de operação, mas também incorpora um `onFallbackAsync`, que registra um aviso (`_logger.LogWarning(...)`) para rastreabilidade e auditoria.

**Vantagens desse padrão:**

* Garantia de continuidade do fluxo de negócios;
* Redução de impacto negativo na experiência do usuário;
* Definição clara de um valor padrão, que pode ser calibrado conforme o perfil do ativo ou estratégia de risco.

**Limitações e considerações:**

* O valor fixo de fallback (100) pode impactar na precisão do P\&L estimado, caso o preço real seja significativamente diferente;
* Pode ser interessante evoluir para uma estratégia mais contextualizada (último preço conhecido armazenado localmente).

### Observabilidade: Logging como Aliado Estratégico
---
O uso do `_logger` com `LogWarning` e `LogInformation` permite a implementação de observabilidade no sistema. A rastreabilidade de eventos excepcionais (como a ativação do fallback) e o registro de cotações utilizadas para cálculo de P\&L tornam o sistema mais fácil de monitorar.

Para ambientes de produção, a estratégia de observabilidade seria expandida para incluir:

* **Tracing distribuído** com OpenTelemetry;
* **Métricas customizadas**: tempo de resposta de APIs externas, número de circuitos abertos, quantidade de fallbacks acionados;
* **Alertas proativos** via dashboards em ferramentas como Prometheus + Grafana.

### Resiliência em Simulações e Operações
---
Os métodos `SimulateOperationAsync` e `CreateTradeAsync` tem o mesmo padrão. Em ambos, a ausência de cotações não compromete a execução da lógica de negócios, mantendo a simulação ou o registro de uma nova trade funcional.

**Destaques técnicos:**

* Separação clara de responsabilidades entre os repositórios;
* Uso da média ponderada para cálculo de preço médio em compras sucessivas;
* Atualização correta de `PnL` com base na última cotação disponível;
* Lógica de fallback para impede falha total do serviço mesmo com falhas sistêmicas externas.
