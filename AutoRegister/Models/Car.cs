using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Car
{
    public int Id { get; set; }

    [Required]
    public string Make { get; set; }

    [Required]
    public string Model { get; set; }

    public int Year { get; set; }

    [Required]
    public string PlateNumber { get; set; }

    [ForeignKey("Owner")]
    public string OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }
}