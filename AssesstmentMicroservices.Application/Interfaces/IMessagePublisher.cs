using AssesstmentMicroservices.Application.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesstmentMicroservices.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishOrderCreatedAsync(OrderCreatedEvent orderEvent);
    }
}
