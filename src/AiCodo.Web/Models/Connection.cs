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
