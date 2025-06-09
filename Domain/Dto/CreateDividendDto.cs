using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class CreateDividendDto
    {
        public Guid AssetId { get; set; }
        public DividendType DividendType { get; set; }
        public decimal ValuePerShare { get; set; }
        public DateTime ExDate { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
