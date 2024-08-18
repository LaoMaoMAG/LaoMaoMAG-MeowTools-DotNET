namespace MeowTools.DebugTest;

class Program
{
    static void Main(string[] args)
    {
        var operationKey = new WebUtility.OperationKey("a.json");
        operationKey.SetKey("aaa");
        Console.WriteLine("aaa");
    }
}