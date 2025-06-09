using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class QuoteDto
    {
        public string AssetCode { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime QuoteTime { get; set; }
    }
}
