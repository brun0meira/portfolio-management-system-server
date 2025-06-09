namespace Infrastructure.Queue
{
    public class QuoteConsumerConfig : KafkaConsumerConfig
    {
        public QuoteConsumerConfig()
        {
            Topic = "cotas";
            GroupId = "cotas-consumer-group";
        }
    }
}