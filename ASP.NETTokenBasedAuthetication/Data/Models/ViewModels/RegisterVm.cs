using System.ComponentModel.DataAnnotations;

namespace ASP.NETTokenBasedAuthetication.Data.Models.ViewModels
{
    public class RegisterVm
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        public string? Email { get; set; }
        [Required]
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
