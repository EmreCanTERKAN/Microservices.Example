
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    // Bizler mikroservislerin kuyruklarını düzenli, merkezi bir sistemde tutmak mecburiyetindeyiz. Bundan dolayı ise böyle bir değişmeyen const verileri tutabileceğimiz class tanımlarız.
    public static class RabbitMQSettings
    {
        public const string Stock_OrderCreatedEventQueue = "stock-order-created-event-queue";
    }
}
