using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    // Mesajlarda genellikle ekstra bilgiler tutarız.
    public class OrderItemMessage
    {
        public Guid ProductId { get; set; }
        public long Count { get; set; }
    }
}
