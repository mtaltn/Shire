using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Shire.WebMvc.Dtos;

public class UserAddDto
{
[DisplayName("Kullanıcı Adı")]
[Required(ErrorMessage = "{0} boş geçilmemelidir.")]
[MaxLength(50, ErrorMessage = "{0} {1} karakterden büyük olmamalıdır.")]
[MinLength(2, ErrorMessage = "{0} {1} karakterden küçük olmamalıdır.")]
public string UserName { get; set; }
[DisplayName("E-posta Adresi")]
[Required(ErrorMessage = "{0} boş geçilmemelidir.")]
[MaxLength(100, ErrorMessage = "{0} {1} karakterden büyük olmamalıdır.")]
[MinLength(10, ErrorMessage = "{0} {1} karakterden küçük olmamalıdır.")]
[DataType(DataType.EmailAddress)]
public string Email { get; set; }
[DisplayName("Şifre")]
[Required(ErrorMessage = "{0} boş geçilmemelidir.")]
[MaxLength(30, ErrorMessage = "{0} {1} karakterden büyük olmamalıdır.")]
[MinLength(7, ErrorMessage = "{0} {1} karakterden küçük olmamalıdır.")]
[DataType(DataType.Password)]
public string Password { get; set; }
}