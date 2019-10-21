namespace ite4160.Models
{
    public class Call
    {
        public int ID { get; set; }
        public int Caller { get; set; }
        public int? Receiver { get; set; }
        public CallType Type { get; set; }
    }
}