using System.ComponentModel.DataAnnotations;

namespace Gymap.Common.DTO.Auth
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [MaxLength(100, ErrorMessage = "Ime je predugo.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail je obavezan.")]
        [EmailAddress(ErrorMessage = "E-mail nije ispravan.")]
        [MaxLength(100, ErrorMessage = "E-mail je predug.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [MinLength(8, ErrorMessage = "Lozinka mora imati najmanje 8 znakova.")]
        public string Password { get; set; } = string.Empty;
    }

    public class UserLoginDto
    {
        [Required(ErrorMessage = "E-mail je obavezan.")]
        [EmailAddress(ErrorMessage = "E-mail nije ispravan.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string ActiveOffer { get; set; } = string.Empty;
    }
}
