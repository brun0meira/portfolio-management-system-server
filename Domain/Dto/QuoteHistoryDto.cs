using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class QuoteHistoryDto
    {
        public DateTime QuoteTime { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
