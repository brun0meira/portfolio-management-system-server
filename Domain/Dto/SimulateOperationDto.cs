using Domain.Enum;

namespace Domain.Dto
{
    public class SimulateOperationRequest
    {
        public Guid AssetId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public TradeType TradeType { get; set; }
        public decimal Fee { get; set; }
    }

    public class SimulateOperationResponse
    {
        public int NewQuantity { get; set; }
        public decimal NewAvgPrice { get; set; }
        public decimal EstimatedPnL { get; set; }
    }

}
