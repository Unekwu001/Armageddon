using System.ComponentModel.DataAnnotations;

namespace Armageddon.Server.Data.Enums
{
    public enum UserTypeEnum
    {
        [Display(Name = "Seller")]
        Seller =1,

        [Display(Name = "Buyer")]
        Buyer =2,

        [Display(Name = "Admin")]
        Admin = 3,

        [Display(Name = "SuperAdmin")]
        SuperAdmin = 4
    }
}
