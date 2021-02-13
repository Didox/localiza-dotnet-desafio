using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Entities.Enums;

namespace Domain.Entities
{
    [Table("vehicles")]
    public class Vehicle
    {
        [Key]
        [Column]
        public int Id { get; set; }

        [Column]
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Column]
        [Required]
        public int BrandId { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }

        [Column]
        [Required]
        public int ModelId { get; set; }

        [JsonIgnore]
        public Model Model { get; set; }

        [Column]
        [Required]
        public int Year { get; set; }

        [Column]
        [Required]
        [MaxLength(50)]
        public string LicensePlate { get; set; }

        [Column]
        [Required]
        [MaxLength(50)]
        public DateTime Date { get; set; }

        [Column]
        [Required]
        [MaxLength(50)]
        public int LuggageCapacity { get; set; }

        [Column]
        [Required]
        public TankCapacity TankCapacity { get; set; }

        [Column]
        [Required]
        public int CategoryId { get; set; }

		[JsonIgnore]
        public Category Category { get; set; }

    }
}
