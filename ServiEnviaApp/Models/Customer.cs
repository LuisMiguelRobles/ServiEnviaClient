using System;

namespace ServiEnviaApp.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string document { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime birthDate { get; set; }
        public string email { get; set; }
        //public ICollection<OrderWindow> Orders { get; set; }


        public override string ToString()
        {
            return $"Document: {document}, Full Name:{firstName} {lastName}";
        }
    }
}
