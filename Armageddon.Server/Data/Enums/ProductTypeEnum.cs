using System.ComponentModel.DataAnnotations;

namespace Armageddon.Server.Data.Enums
{
    public enum ProductTypeEnum
    {
        [Display(Name = "Cana")]
        Cana = 1,

        [Display(Name = "Couch")]           
        Couch = 2,

        [Display(Name = "Baki")]
        Baki = 3,

        [Display(Name = "GhanaLoud")]
        GhanaLoud = 4,

        [Display(Name = "Skunk")]
        Skunk = 5,

        [Display(Name = "Ice")]
        Ice = 6,

        [Display(Name = "Shisha")]
        Shisha = 7,

        [Display(Name = "Colo")]
        Colo = 8,

        [Display(Name = "Tobacco")]
        Tobacco = 9
    }
}
