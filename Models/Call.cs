using System.Collections.Generic;

namespace ite4160.Models
{
    public class Call
    {
        public int ID { get; set; }
        public string Caller { get; set; }
        public string Receiver { get; set; }
        public CallType Type { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}