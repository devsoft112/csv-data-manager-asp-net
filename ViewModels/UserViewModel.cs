using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CSVDataManager.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string Mobile { get; set; }
        public bool Active { get; set; }

        // This property should only be used in views for rendering the dropdown
        public IEnumerable<SelectListItem> SexOptions { get; set; }
    }
}
