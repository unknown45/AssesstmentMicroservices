using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesstmentMicroservices.Application.Events
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public List<int> ProductIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
