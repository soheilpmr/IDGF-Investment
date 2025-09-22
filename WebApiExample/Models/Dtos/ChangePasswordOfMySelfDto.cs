namespace IDGF.Auth.Models.Dtos
{
    public class ChangePasswordOfMySelfDto
    {
        public string RepeatNewPassword { get; set; }
        //public string UserID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
