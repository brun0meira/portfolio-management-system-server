namespace Domain.Dto
{
    public class PositionDto
    {
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public int Quantity { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal PnL { get; set; }
    }
}
