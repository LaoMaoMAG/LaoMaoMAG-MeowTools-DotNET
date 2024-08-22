using System.Text.Json;
using DanKeJson;

namespace MeowTools.WebUtility;

/// <summary>
/// 操作密钥
/// 用于服务器的敏感操作验证
/// </summary>
public class OperationKey
{
    // 密钥文件路径
    public string KeyFilePath { get; }
    
    // 密钥表
    public JsonData KeyTable { get; private set; }
    
    // 密钥长度
    public int KeyLength { get; set; }
    
    // 默认密钥长度
    public const int DefaultKeyLength = 64;
    

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="keyLength"></param>
    public OperationKey(string filePath, int keyLength = DefaultKeyLength)
    {
        // 初始化成员
        KeyFilePath = filePath;
        KeyLength = keyLength;
        KeyTable = new JsonData(JsonData.Type.Object);
        
        // 如果文件不存在，则创建文件并写入字符串  
        if (!File.Exists(KeyFilePath))
        {
            using StreamWriter writer = new StreamWriter(KeyFilePath, false);
            writer.WriteLine("{}");
            KeyTable = new(JsonData.Type.Object);
        }
        else
        {
            // 读取密钥
            ReadKeyFile();
        }
    }

    
    /// <summary>
    /// 验证密钥
    /// </summary>
    /// <returns></returns>
    public bool VerificationKey(string name, string key)
    {
        return KeyTable[name] == key;
    }


    /// <summary>
    /// 设置密钥
    /// </summary>
    /// <param name="name"></param>
    /// <param name="key"></param>
    public void SetKey(string name, string? key = null)
    {
        // 如果密钥为空随机密钥
        key ??= GenerateKey(KeyLength);
        
        // 设置密钥
        KeyTable[name] = key;
        
        // 保存后读取
        SaveKeyFile();
        ReadKeyFile();
    }


    /// <summary>
    /// 获取密钥
    /// </summary>
    public string GetKey(string name)
    {
        return KeyTable[name];
    }


    /// <summary>
    /// 读取密钥文件
    /// </summary>
    private void ReadKeyFile()
    {
        string json = File.ReadAllText(KeyFilePath);
        KeyTable = JSON.ToData(json);
    }


    /// <summary>
    /// 保存密钥
    /// </summary>
    private void SaveKeyFile()
    {
         string json = JSON.ToJson(KeyTable);
         
         var options = new JsonSerializerOptions  
         {  
             WriteIndented = true  
         }; 
  
         // Json字符串格式化
         json = JsonSerializer.Serialize(JsonDocument.Parse(json), options);  

         // 写入文件
         using StreamWriter writer = new StreamWriter(KeyFilePath, false);
         writer.WriteLine(json);
    }
    
    
    /// <summary>
    /// 生成随机密钥
    /// </summary>
    /// <param name="length">密钥长度</param>
    /// <returns></returns>
    private static string GenerateKey(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new char[length];
        for (int i = 0; i < length; i++) result[i] = chars[random.Next(chars.Length)];
        return new string(result);
    }
}