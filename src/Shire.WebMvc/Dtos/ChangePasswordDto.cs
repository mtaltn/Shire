namespace Shire.WebMvc.Dtos;

public class ChangePasswordDto
{
    public int Id { get; set; }
    public string NewPassword { get; set; }
    public string OldPassword { get; set; }
}
