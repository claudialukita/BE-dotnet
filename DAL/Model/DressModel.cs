using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Model
{
    public class DressModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Type { get; set; }

        [StringLength(8)]
        public string Color { get; set; }

        [StringLength(5)]
        public string Size { get; set; }

        public double Price { get; set; }

        public DateTime CreatedDate { get; set; }

        public DressModel()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
