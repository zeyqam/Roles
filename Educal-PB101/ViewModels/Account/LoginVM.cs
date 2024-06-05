using System.ComponentModel.DataAnnotations;

namespace Educal_PB101.ViewModels.Account
{
    public class LoginVM
    {
        [Required]
        public string EmailorUsername { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
