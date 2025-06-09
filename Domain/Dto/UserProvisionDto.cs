using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class UserProvisionDto
    {
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public int Quantity { get; set; }
        public string DividendType { get; set; }
        public decimal ValuePerShare { get; set; }
        public decimal TotalValue => Quantity * ValuePerShare;
        public DateTime ExDate { get; set; }
        public DateTime PaymentDate { get; set; }
    }

}
