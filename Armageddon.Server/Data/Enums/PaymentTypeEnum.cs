using System.ComponentModel.DataAnnotations;

namespace Armageddon.Server.Data.Enums
{
    public enum PaymentTypeEnum
    {
        [Display(Name = "PayCashAfterDelivery")]
        PayCashAfterDelivery = 1,

        [Display(Name = "PayStack")]
        PayStack = 2,

        [Display(Name = "Crypto")]
        Crypto = 3
    }
}
