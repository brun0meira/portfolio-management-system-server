using Domain.Enum;

namespace Domain.Dto
{
    public class CreateTradeDto
    {
        public Guid AssetId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public TradeType TradeType { get; set; }
        public DateTime TradeTime { get; set; }
    }
}
