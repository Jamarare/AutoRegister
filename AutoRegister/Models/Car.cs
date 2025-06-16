using System.ComponentModel.DataAnnotations;

namespace AutoRegister.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Make on kohustuslik")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Model on kohustuslik")]
        public string Model { get; set; }

        [Range(1900, 2100, ErrorMessage = "Year peab olema vahemikus 1900 kuni 2100")]
        public int Year { get; set; }

        [Required(ErrorMessage = "LicensePlate on kohustuslik")]
        public string LicensePlate { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}