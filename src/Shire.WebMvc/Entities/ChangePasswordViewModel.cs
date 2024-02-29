using System.ComponentModel.DataAnnotations;

namespace Shire.WebMvc.Entities;

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Parola")]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Parolayı Onayla")]
    [Compare("NewPassword", ErrorMessage = "Parolalar eşleşmiyor.")]
    public string ConfirmPassword { get; set; }
}
