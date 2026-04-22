using System.ComponentModel.DataAnnotations;

namespace Armageddon.Server.Data.Enums
{
    public enum DeliveryStatusEnum
    {
        [Display(Name = "NotStarted")]
        NotStarted = 1,

        [Display(Name = "InProgress")]
        InProgress = 2,

        [Display(Name = "Delivered")]
        Delivered = 3,

        [Display(Name = "Cancelled")]
        Cancelled = 4,

        [Display(Name = "Returned")]
        Returned = 5
    }
}
