using Domain.Enum;

namespace Domain.Dto
{
    public class KafkaDividendMessageDto
    {
        public Guid UserId { get; set; }
        public Guid AssetId { get; set; }
        public DividendType DividendType { get; set; }
        public decimal ValuePerShare { get; set; }
        public DateTime ExDate { get; set; }
        public DateTime PaymentDate { get; set; }
    }

}
