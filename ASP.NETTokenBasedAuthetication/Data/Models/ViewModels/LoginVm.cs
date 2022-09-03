using System.ComponentModel.DataAnnotations;

namespace ASP.NETTokenBasedAuthetication.Data.Models.ViewModels
{
    public class LoginVm
    {
        [Required]
        public string? Email { get; set; }       
        public string? Password { get; set; }
    }
}
