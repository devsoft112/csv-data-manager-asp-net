using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;

namespace CSVDataManager.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Name("FirstName")]
        public string Firstname { get; set; }

        [Name("Surname")]
        public string Surname { get; set; }

        [Name("Age")]
        [Range(1, 120, ErrorMessage = "Age must be a valid number between 1 and 120")]
        public int Age { get; set; } // Age as integer

        [Name("Sex")]
        [Required]
        [RegularExpression("^[MF]$", ErrorMessage = "Sex must be 'M' or 'F'")]
        public string Sex { get; set; } // Sex must be 'M' or 'F'

        [Name("Mobile")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must consist only of numbers")]
        public string Mobile { get; set; } // Mobile as string but validated for numeric only

        [Name("Active")]
        public bool Active { get; set; }
    }
}