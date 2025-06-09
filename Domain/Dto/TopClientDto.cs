namespace Domain.Dto
{
    public class TopClientDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal TotalPositionValue { get; set; }
    }
}
