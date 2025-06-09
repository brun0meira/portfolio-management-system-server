public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Email { get; set; }
    public decimal FeePercentage { get; set; }

    public ICollection<Trade> Trades { get; set; }
    public ICollection<Position> Positions { get; set; }
}