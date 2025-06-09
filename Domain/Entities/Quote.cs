public class Quote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssetId { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime QuoteTime { get; set; }

    public Asset Asset { get; set; }
}
