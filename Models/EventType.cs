using System.ComponentModel.DataAnnotations;

namespace ite4160.Models
{
    public enum EventType
    {
        [Display(Name = "Pick-up")]
        PickUp,

        [Display(Name = "Dialling")]
        Dial,

        [Display(Name = "Call Established")]
        CallEstablished,

        [Display(Name = "Call End")]
        CallEnd,

        [Display(Name = "Hang-up")]
        HangUp
    }
}