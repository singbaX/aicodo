// See https://aka.ms/new-console-template for more information
using AiCodo;
using AiCodo.Cmds;
using AiCodo.Codes;
using AiCodo.Data;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

Console.WriteLine(@"AiCode命令行执行模式，支持以下命令：");
Console.WriteLine(@"刷新表：reloadtables");
Console.WriteLine(@"生成代码：codetable sys_user entity User.cs");
Console.WriteLine(@"生成代码：codesql sys_user entity User.cs");
Console.WriteLine(@"任何问题，请联系作者，邮件singba@163.com");

if (args != null && args.Length > 0)
{
    var configRoot = args[0];
    if (System.IO.Directory.Exists(configRoot))
    {
        "Program".Log($"加载文件夹[{args[0]}]");
        ApplicationConfig.LocalConfigFolder = configRoot;
    }
    else
    {
    }
}
else
{
    ApplicationConfig.LocalConfigFolder = "configs".FixedAppBasePath();
}

//AiCodo:Set db provider
DbProviderFactories.SetFactory("mysql", MySqlProvider.Instance);

Dictionary<string, Action<string[]>> methods = new Dictionary<string, Action<string[]>>();
CodeCommands.GetMethods().ForEach(item => methods[item.Key] = item.Value);

Console.WriteLine("代码模板");
CodeSetting.Current.Templates.ForEach(t =>
{
    Console.WriteLine($"[{t.Name}] - [{t.FileName}]");
});

var line = "";
while (true)
{
    Console.Write("请输入命令：");
    line = Console.ReadLine();
    if (line.Length == 0)
    {
        Console.WriteLine("请问是要关闭程序吗？（y/n）");
        line = Console.ReadLine();
        if (line.IsNullOrEmpty() || line.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }
        continue;
    }

    var name = CodeCommands.ReadCommandName(line, out string[] cmdArgs);
    if (methods.TryGetValue(name.ToLower(), out var action))
    {
        try
        {
            action(cmdArgs);
        }
        catch (Exception ex)
        {
            "Program".Log($"执行命令错误：{ex.ToString()}");
        }
    }
    else
    {
        Console.WriteLine($"无效的命令：{name}");
    }
}

