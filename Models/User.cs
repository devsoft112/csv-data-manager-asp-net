using CsvHelper.Configuration.Attributes;

namespace CSVDataManager.Models
{
    public class User
    {
        [Name("Identity")]
        public int Id { get; set; }
        [Name("FirstName")]
        public string Firstname { get; set; }
        [Name("Surname")]
        public string Surname { get; set; }
        [Name("Age")]
        public int Age { get; set; }
        [Name("Sex")]
        public string Sex { get; set; }
        [Name("Mobile")]
        public int? Mobile { get; set; }
        [Name("Active")]
        public bool Active { get; set; }
    }
}
