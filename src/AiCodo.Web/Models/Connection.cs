// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    public class ConnectionItem
    {
        [Required]
        [Display(Name ="连接名称",Description ="本连接在配置文件中的名称，是很重要的引用名称")]        
        public string Name { get; set; }

        [Required]
        [Display(Name ="数据库类型",Description ="对应连接的数据库处理类型")]
        public string ProviderName { get; set; }

        [Required]
        [Display(Name ="服务地址",Description ="本机地址为localhost，其它服务器一般为IP地址")]
        public string Server { get; set; }

        [Required]
        [Display(Name ="服务端口")]
        public int Port { get; set; } = 3306;

        [Required]
        [Display(Name ="用户")]
        public string Uid { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="密码")]
        public string Password { get; set; }

        [Required]
        [Display(Name ="数据库")]
        public string Database { get; set; }

        public DynamicEntity ToDynamicEntity()
        {
            return new DynamicEntity()
                .Set("name", Name)
                .Set("provider",ProviderName)
                .Set("server", Server)
                .Set("port", Port)
                .Set("uid", Uid)
                .Set("password", Password)
                .Set("database", Database);                
        }
    }
}
