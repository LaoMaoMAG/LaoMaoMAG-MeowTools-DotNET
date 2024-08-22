namespace MeowTools.DebugTest;

class Program
{
    static void Main(string[] args)
    {
        // var operationKey = new WebUtility.OperationKey("a.json");
        // operationKey.SetKey("aaa");

        var configJson = new ConfigJson("config.json");
        
        configJson.Set("a",1);
        configJson.Set("b","2");
        
        configJson.Save();
        configJson.Read();
        
        Console.WriteLine((int)configJson.Get("a"));
        Console.WriteLine((string)configJson.Get("b"));
        Console.WriteLine(configJson.IsExist("a"));
        Console.WriteLine(configJson.IsExist("b"));
        Console.WriteLine(configJson.IsExist("c"));


        ServiceLog serviceLog = new ServiceLog("log");
        
        for (int i = 0; i < 200; i++)
        {
            serviceLog.AddLog(ServiceLog.Type.Message, $"消息{i}", $"消息内容{i}");
            serviceLog.AddLog(ServiceLog.Type.Tip, $"提示{i}", $"提示内容{i}");
            serviceLog.AddLog(ServiceLog.Type.Warning, $"警告{i}", $"消警告内容{i}");
            serviceLog.AddLog(ServiceLog.Type.Error, $"错误{i}", $"错误内容{i}");
        }

        foreach (var filePath in serviceLog.GetFilePathList())
        {
            foreach (var data in ServiceLog.ReadLogFile(filePath))
            {
                Console.WriteLine($"[{data.Time.ToString("yyyy-MM-dd HH:mm:ss.fff")}][{data.Type.ToString()}][{data.Name}] {data.Content}");
            }
        }
    }
}