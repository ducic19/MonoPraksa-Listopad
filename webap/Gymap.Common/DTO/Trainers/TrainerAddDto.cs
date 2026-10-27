using System.ComponentModel.DataAnnotations;

namespace Gymap.Common.DTO.Trainers
{
    public class TrainerAddDto
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [MaxLength(100, ErrorMessage = "Ime je predugo.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail je obavezan.")]
        [MaxLength(100, ErrorMessage = "E-mail je predug.")]
        [EmailAddress(ErrorMessage = "E-mail nije ispravan.")]
        public string Email { get; set; } = string.Empty;
    }
}
