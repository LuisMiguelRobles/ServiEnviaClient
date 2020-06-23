using System;

namespace ServiEnviaApp.Models
{
    public class Order
    {
        public Guid id { get; set; }
        public string senderDocument { get; set; }
        public string receiverDocument { get; set; }
        public string from { get; set; }
        public string destination { get; set; }
        public decimal weight { get; set; }
        public decimal price { get; set; }
        public State state { get; set; }
        public Guid customerId { get; set; }
    }

    public enum State
    {
        Pending,
        Collected,
        Sending,
        Delivery
    }
}
