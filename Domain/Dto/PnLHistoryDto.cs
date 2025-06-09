using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class PnLHistoryDto
    {
        public DateTime Date { get; set; }
        public decimal PnL { get; set; }
    }
}
