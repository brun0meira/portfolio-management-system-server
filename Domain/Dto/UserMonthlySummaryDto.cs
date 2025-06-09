namespace Domain.Dto
{
    public class UserMonthlySummaryDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalPnL { get; set; }
        public decimal TotalBrokerage { get; set; }
        public decimal TotalDividends { get; set; }
    }
}
