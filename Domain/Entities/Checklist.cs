using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    [Table("checklists")]
    public class Checklist
    {
        [Key]
        [Column]
        public int Id { get; set; }

        [Column]
        [Required]
        public bool CleanCar { get; set; }

        [Column]
        [Required]
        public bool FullTank { get; set; }

        [Column]
        [Required]
        public bool PendingCleanCar { get; set; }

        [Column]
        [Required]
        public bool Wrinkled { get; set; }

        [Column]
        [Required]
        public bool Scratches { get; set; }
    }
}
