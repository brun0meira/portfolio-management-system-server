namespace Infrastructure.Queue
{
    public class DividendConsumerConfig : KafkaConsumerConfig
    {
        public DividendConsumerConfig()
        {
            Topic = "dividendos";
            GroupId = "dividendos-consumer-group";
        }
    }
}
