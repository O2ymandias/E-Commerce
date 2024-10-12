using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Auth.AdminViewModels
{
    public class LoginVM
    {
        [Required, DisplayName("UserName Or Email")]
        public string UserNameOrEmail { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
