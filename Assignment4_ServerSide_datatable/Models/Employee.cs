using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Assignment4_ServerSide_datatable.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public string Address { get; set; }

        [Display(Name ="Birth Date")]
        public DateTime BirthDate { get; set; }
    }
}
