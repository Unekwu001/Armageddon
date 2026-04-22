using System.ComponentModel.DataAnnotations;

namespace Armageddon.Server.Data.Enums
{
    public enum PaymentStatusEnum
    {
        [Display(Name = "NotStarted")]
        NotStarted = 1,

        [Display(Name = "Pending")]
        Pending = 2,

        [Display(Name = "Completed")]
        Completed = 3,

        [Display(Name = "Failed")]
        Failed = 4,

        [Display(Name = "Cancelled")]
        Cancelled = 5,

        [Display(Name = "Refunded")]
        Refunded = 6
    }
}
