using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto
{
    public class UserBrokerageTotalDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public decimal TotalBrokeragePaid { get; set; }
    }
}
