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
    }
}