namespace Infrastructure.Queue
{
    public class KafkaConsumerConfig
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public string Topic { get; set; }
    }
}
