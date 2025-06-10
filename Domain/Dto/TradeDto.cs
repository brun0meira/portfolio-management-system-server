using Domain.Enum;

namespace Domain.Dto
{
    public class TradeDto
    {
        public Guid Id { get; set; }
        public DateTime TradeTime { get; set; }
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public TradeType TradeType { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}