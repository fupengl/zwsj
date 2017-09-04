
using System.ComponentModel.DataAnnotations;


namespace ZhiDiExt.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "请输入用户名")]  
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }

        public string Message { get; set; }

    }
}