// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
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
