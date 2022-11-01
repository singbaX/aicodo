using AiCodo;
using AiCodo.WebLogger;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var logRoot = "";

var port = 51666;
if (args.Length > 0)
{
    port = int.Parse(args[0]);
}

if (args.Length > 1)
{
    logRoot = args[0].FixedAppBasePath();
}
else
{
    logRoot = "log".FixedAppBasePath();
}

Console.WriteLine($"�洢·��{logRoot}");
if (!Directory.Exists(logRoot))
{
    Directory.CreateDirectory(logRoot);
}

var receiveCount = 0;
var writeCount = 0;

var queue = new Queue<DynamicEntity>();
var writerLock = new object();
Dictionary<string, LogFile> _LogFiles = new Dictionary<string, LogFile>();
var msgQueue = new ParallelQueue<DynamicEntity>(OnLogItem, 5);
void OnLogItem(DynamicEntity obj)
{
    var group = obj.GetString("group", "");
    var fileName = group.IsNullOrEmpty() ? $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt" : $"{group}.txt";
    fileName = Path.Combine(logRoot, fileName);

    if (_LogFiles.TryGetValue(fileName, out var logFile))
    {
        logFile.AddItem(obj);
    }
    else
    {
        lock (_LogFiles)
        {
            if (_LogFiles.TryGetValue(fileName, out logFile))
            {
                logFile.AddItem(obj);
                Console.WriteLine($"�����ļ�{fileName}");
                return;
            }

            logFile = new LogFile(fileName);
            _LogFiles.Add(fileName, logFile);
            logFile.AddItem(obj);
            logFile.Closed += LogFile_Closed;
        }
    }
}

void LogFile_Closed(object? sender, EventArgs e)
{
    var logFile = sender as LogFile;
    logFile.Closed -= LogFile_Closed;

    lock (_LogFiles)
    {
        writeCount += logFile.WriteCount;
        _LogFiles.Remove(logFile.FileName);
        Console.WriteLine($"�ļ��ر�{logFile.FileName},���ļ�д��[{logFile.WriteCount}]��[{logFile.ReceivedCount}]");
        Console.WriteLine($"����[{_LogFiles.Count}]���ļ���д���ܼ���д��[{writeCount}]���ܼ��յ�[{receiveCount}]");
    }
}

Task OnAddLog(HttpContext context)
{
    var ss = context.Request.Body.ReadToEnd();
    DynamicEntity data = ss;
    if (data != null)
    {
        msgQueue.AddItem(data);
        Interlocked.Increment(ref receiveCount);
    }
    else
    {
        Console.WriteLine($"��Ч��{ss}");
    }
    return context.Response.WriteAsync(new DynamicEntity("wait", msgQueue.ItemCount).ToJson());
}

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGet("/", () => new DynamicEntity(
    "time", "2022-08-19 14:58:32.323",
    "app", "��������",
    "module", "ģ������",
    "level", "INFO",
    "thread", 0,
    "group", "������",
    "id", "�߼�ID",
    "msg", "�ðɣ���������",
    "content", "{}").ToJson());

app.MapPost("/addlog", OnAddLog);
msgQueue.Start();

app.Run($"http://*:{port}");

public class CloseArgs : EventArgs
{
    public bool Closed { get; set; }
}
