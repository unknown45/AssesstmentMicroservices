using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesstmentMicroservices.Application.DTO
{
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public List<int> ProductIds { get; set; } = new();
    }
}
