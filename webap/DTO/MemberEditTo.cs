using System.ComponentModel.DataAnnotations;
namespace DTO;

public class MemberEditDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name is too long")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(100, ErrorMessage = "Email is too long")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Subscription ID is required")]
    public Guid SubsId { get; set; }
}
