// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using AiCodo;
using AiCodo.Cmds;
using AiCodo.Codes;
using AiCodo.Data;
using System.Diagnostics;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var error = "";

Console.WriteLine(@"AiCodo命令行执行模式，支持以下命令：");
if (args == null || args.Length == 0)
{
    Console.WriteLine(@"你使用的是无参数模式，如果需要修改配置路径及代码路径，可以使用带参数的方式
AiCodo.Cmds {配置路径} {代码路径}
路径可以使用相对本程序运行目录的路径
");
}
Console.WriteLine(@"刷新表：reloadtables");
Console.WriteLine(@"列出所有表：listtables");
Console.WriteLine(@"列出所有表：listtables");
Console.WriteLine(@"基于表结果生成代码：codetable {表名} {模板名} {文件名}");
Console.WriteLine(@"基于表命令生成代码：codesql {表名} {模板名} {文件名}");
Console.WriteLine(@"基于配置文件代码：code {模板名称} {文件名}");
Console.WriteLine(@"开始直接支持CodeService的命令，命令codecmd
导出excel表结构：codecmd export filename schema.xlsx 
用xslt样式转换xml：codecmd xslt xmlfile a.xml xsltfile x.xslt filename newfile.xx
为了方便调用其它命令，可以直接运行本目录下其它命令，更多命令，请参考cmd.md
");

if (args != null && args.Length > 0)
{
    var configRoot = args[0].FixedAppBasePath();
    if (System.IO.Directory.Exists(configRoot))
    {
        ApplicationConfig.LocalConfigFolder = configRoot;
    }

    if (args.Length > 1 && args[1].IsNotEmpty())
    {
        var codeRoot = args[1].FixedAppBasePath();
        if (!System.IO.Directory.Exists(codeRoot))
        {
            System.IO.Directory.CreateDirectory(codeRoot);
        }
        CodeService.CodeRoot = codeRoot;
    }
}
else
{
    ApplicationConfig.LocalConfigFolder = "configs".FixedAppBasePath();
}
Console.WriteLine($"配置路径：{ApplicationConfig.LocalConfigFolder}");
Console.WriteLine($"代码路径：{CodeService.CodeRoot}");

//AiCodo:Set db provider
DbProviderFactories.SetFactory("mysql", MySqlProvider.Instance);

Dictionary<string, Action<string[]>> methods = new Dictionary<string, Action<string[]>>();
CodeCommands.GetMethods().ForEach(item => methods[item.Key] = item.Value);

Console.WriteLine("代码模板");

//RazorEngine.Compilation.ReferenceResolver.UseCurrentAssembliesReferenceResolver

var codeSettingFile = "CodeSetting.xml".FixedAppConfigPath();
if (codeSettingFile.IsFileNotExists())
{
    new CodeSetting().SaveXDoc(codeSettingFile);
    "APP".Log($"代码设置文件不存在，创建默认文件：{codeSettingFile}");
}

CodeSetting.Current.Templates.ForEach(t =>
{
    Console.WriteLine($"[{t.Name}] - [{t.FileName}]");
});

methods.Add("showerror", (arr) => Console.WriteLine(error));

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
            "Program".Log($"执行命令错误：{ex.Message}");
            error = ex.ToString();
            var errIndex = error.IndexOf("error: (");
            if (errIndex > 0)
            {
                var endIndex = error.IndexOfAny(new char[] { '\r', '\n' }, errIndex + 1);
                if (endIndex > 0)
                {
                    "Program".Log($"执行命令错误：{error.Substring(errIndex, endIndex - errIndex)}");
                }
            }
        }
    }
    else
    {
        try
        {
            RunCommand(line, out string result);
            Console.WriteLine(line);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine($"无效的命令：{name}");
        }
    }
}


static void RunCommand(string commandLine, out string result)
{
    Process cmd = new Process();
    cmd.StartInfo.WorkingDirectory = ApplicationConfig.BaseDirectory;
    cmd.StartInfo.FileName = "cmd.exe";
    cmd.StartInfo.RedirectStandardInput = true;
    cmd.StartInfo.RedirectStandardOutput = true;
    cmd.StartInfo.CreateNoWindow = true;
    cmd.StartInfo.UseShellExecute = false;
    cmd.Start();

    cmd.StandardInput.WriteLine(commandLine);
    cmd.StandardInput.Flush();
    result = cmd.StandardOutput.ReadToEnd();
    cmd.StandardInput.Close();
}

