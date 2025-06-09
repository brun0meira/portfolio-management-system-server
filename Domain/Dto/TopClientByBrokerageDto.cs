namespace Domain.Dto
{
    public class TopClientByBrokerageDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal TotalBrokerage { get; set; }
    }
}
