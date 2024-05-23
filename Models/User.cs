using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;

namespace CSVDataManager.Models
{
    public class User
    {
        // Primary key with identity column
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Firstname property with CSV mapping attribute
        [Name("FirstName")]
        public string Firstname { get; set; }

        // Surname property with CSV mapping attribute
        [Name("Surname")]
        public string Surname { get; set; }

        // Age property with CSV mapping attribute and validation
        [Name("Age")]
        [Range(1, 120, ErrorMessage = "Age must be a valid number between 1 and 120")]
        public int Age { get; set; } // Age as integer

        // Sex property with CSV mapping attribute
        [Name("Sex")]
        public string Sex { get; set; } // Sex must be 'M' or 'F'

        // Mobile property with CSV mapping attribute and validation for numeric only
        [Name("Mobile")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must consist only of numbers")]
        public string Mobile { get; set; } // Mobile as string but validated for numeric only

        // Active property with CSV mapping attribute
        [Name("Active")]
        public bool Active { get; set; }
    }
}
