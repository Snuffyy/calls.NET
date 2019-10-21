using System;
using System.ComponentModel.DataAnnotations;

namespace ite4160.Models
{
    public class Event
    {
        public int ID { get; set; }

        [Display(Name = "Event")]
        public EventType Type { get; set; }

        public DateTime Timestamp { get; set; }

        [Display(Name = "Caller")]
        public Call Call { get; set; }
    }
}