namespace Domain.Dto
{
    public class CreateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal FeePercentage { get; set; }
    }
}