public class Position
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid AssetId { get; set; }
    public int Quantity { get; set; }
    public decimal AvgPrice { get; set; }
    public decimal PnL { get; set; }

    public User User { get; set; }
    public Asset Asset { get; set; }
}