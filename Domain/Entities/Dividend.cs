using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Dividend
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AssetId { get; set; }
        public DividendType DividendType { get; set; }
        public decimal ValuePerShare { get; set; }
        public DateTime ExDate { get; set; }
        public DateTime PaymentDate { get; set; }

        public Asset Asset { get; set; }
    }
}
