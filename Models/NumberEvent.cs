namespace ite4160.Models
{
    public class NumberEvent
    {
        public int CallId { get; set; }
        public Event Event { get; set; }
        public string Receiver { get; set; }
        public CallType CallType { get; set; }

        // public NumberEvent(Event e, string receiver, CallType callType)
        // {
        //     Event = e;
        //     Receiver = receiver;
        //     CallType = callType;
        // }
    }
}