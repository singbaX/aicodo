using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCodo.Services
{
    public static partial class UserService
    {
        public static bool Login(string userName, string password)
        {
            return true;
        }

        public static IEnumerable<DynamicEntity> GetAllUsers()
        {
            return WebService.Request<List<DynamicEntity>>("service/sys_user/selectall"); 
        }
    }


    public static partial class UserService
    {
        const string TableName = "sys_user";
        public static T Insert<T>(string userName, string password, int createUser, DateTime createTime, bool isValid)
        {
            return WebService.Request<T>("sys_user.Insert", "UserName", userName, "Password", password, "CreateUser", createUser, "CreateTime", createTime, "IsValid", isValid);
        }

        public static int Delete(int iD)
        {
            return WebService.Request<int>("sys_user.Delete", "ID", iD);
        }

        public static int Update(string userName, string password, int createUser, DateTime createTime, bool isValid, int iD)
        {
            return WebService.Request<int>("sys_user.Update", "UserName", userName, "Password", password, "CreateUser", createUser, "CreateTime", createTime, "IsValid", isValid, "ID", iD);
        }

        public static IEnumerable<T> SelectAll<T>() where T : IEntity, new()
        {
            return WebService.Request<List<T>>("sys_user.SelectAll");
        }

        public static IEnumerable<T> SelectByKeys<T>(int iD) where T : IEntity, new()
        {
            return WebService.Request<List<T>>("sys_user.SelectByKeys", "ID", iD);
        }

        public static T Count<T>()
        {
            return WebService.Request<T>("sys_user.Count");
        }

    }
}
