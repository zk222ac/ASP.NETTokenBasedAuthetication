namespace ASP.NETTokenBasedAuthetication.Data.Models.ViewModels
{
    public class AuthResultVm
    {
        public string? Token { get; set; }
        public DateTime ExpireAt { get; set; }
        public string? RefreshToken { get; set; }
    }
}
