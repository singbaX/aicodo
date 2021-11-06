using AiCodo.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AiCodo.Web.Services
{
    public class UserService : IUserService
    {
        public IUser Login(string username, string password)
        {
            var user = SqlService.ExecuteQuery<DynamicEntity>("sys_user.SelectByUserName", "UserName", username).FirstOrDefault();
            if (user == null)
            {
                this.Log($"login user:{username},not exists");
                return null;
            }
            var pwd = user.GetString("Password");
            if (!pwd.Equals(password))
            {
                this.Log($"login user:{username},password error");
                return null;
            }

            this.Log($"login user:{username}");
            return new UserModel
            {
                UserID = user.GetString("ID"),
                UserName = username,
            };
        }
    }

    public class UserModel : Entity, IUser
    {
        public string UserName { get; set; }
        public string UserID { get; set; }
    }
}
