namespace ite4160.Models
{
    /*
    Full means that the whole call flow from PickUp to HangUp { PickUp, Dial, CallEstablished, CallEnd, HangUp }
    Cancelled means that receiver is not answering {PickUp, Dial, CallEnd}
    Non-dialed means that the caller picks up the phone but changes his mind and doesn't call {PickUp, HangUp}
    */
    public enum CallType
    {
        Full,
        Cancelled,
        NonDialled
    }
}