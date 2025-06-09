using Domain.Enum;

public class Trade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid AssetId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public TradeType TradeType { get; set; }
    public decimal Fee { get; set; }
    public DateTime TradeTime { get; set; }

    public User User { get; set; }
    public Asset Asset { get; set; }
}
