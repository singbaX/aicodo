using AiCodo.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AiCodo.Tests
{
    public class AccountTest
    {
        [Test]
        public void LoginTest()
        {
            //login,get token
            var token = WebService.GetToken("admin", "admin");
            Assert.IsTrue(token != null);
            //store token for coming request
            WebService.Token = token.GetString("access_token");

            //get all users
            var users = WebService.Request<List<DynamicEntity>>("service/sys_user/SelectAll");
            Assert.IsNotNull(users);

            //find user "test1"
            var testUser = users.FirstOrDefault(f => f.GetString("UserName").Equals("test1", StringComparison.OrdinalIgnoreCase));
            if (testUser != null)
            {
                //remove if exists
                var cc = WebService.Request<int>("service/sys_user/Delete", new DynamicEntity("ID", testUser.GetInt32("ID")));
                Assert.IsTrue(cc == 1);
            }

            //create new user
            testUser = new DynamicEntity()
                .Set("UserName", "test1")
                .Set("Email", "test1@xx.com")
                .Set("Password", "123456")
                .Set("CreateUser", 1)
                .Set("CreateTime", DateTime.Now)
                .Set("UpdateUser", 1)
                .Set("UpdateTime", DateTime.Now)
                .Set("IsValid", 1);
            //add user (return id)
            var newID = WebService.Request<int>("service/sys_user/Insert", testUser);
            Assert.IsTrue(newID > 0);

            //change user's value
            testUser.SetValue("ID", newID);
            testUser.SetValue("Email", "test1user@xx.com");

            //update user
            var count = WebService.Request<int>("service/sys_user/Update", testUser);
            Assert.IsTrue(count > 0);

            //remove user
            count = WebService.Request<int>("service/sys_user/Delete", new DynamicEntity("ID", newID));
            Assert.IsTrue(count == 1);
        }

        [Test]
        public void TestAsync()
        {
            var waitDone = false;
            var done = false;
            AsyncService.StartWaitTask(2)
                .ContinueWith(t =>
                {
                    waitDone = true;
                    AsyncService.StartTask(() =>
                    {
                        done = true;
                    });
                }).Wait();
            Assert.IsTrue(waitDone);
            Assert.IsFalse(done);
        }
    }

    static class AsyncService
    {
        public static Task StartTask(Action action)
        {
            return Task.Run(() =>action);
        }

        public static Task StartWaitTask(int second)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(second);
            });
        }
    }
}
