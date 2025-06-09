using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class AveragePriceResponseDto
    {
        public Guid AssetId { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
