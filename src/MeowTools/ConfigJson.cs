using System.Text.Json;
using DanKeJson;

namespace MeowTools;

/// <summary>
/// 配置文件类
/// 基于 DanKeJson 实现
/// </summary>
public class ConfigJson
{
    public string ConfigFilePath { get; }
    
    private JsonData _configData = new(JsonData.Type.Object); 
    
    
    private class Data
    {
        public string Key { get; set; }
        public object? Value { get; set; }
    }
    
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="configFilePath">配置文件路径</param>
    public ConfigJson(string configFilePath)
    {
        ConfigFilePath = configFilePath;
        
        // 如果文件不存在，则创建文件并写入字符串  
        if (!File.Exists(ConfigFilePath))
        {
            using StreamWriter writer = new StreamWriter(ConfigFilePath, false);
            writer.WriteLine("{}");
        }
    }

    
    public void Init(string key, JsonData value)
    {
        if (!IsExist(key)) Set(key, value);
    }
    
    
    public JsonData Get(string key)
    {
        return _configData[key];
    }

    
    public void Set(string key, JsonData value)
    {
        _configData[key] = value;
    }

    
    public bool IsExist(string key)
    {
        return _configData.HasKey(key);
    }
    
    
    public void Delete(string key)
    {
        if (_configData.HasKey(key))  
        {  
            _configData[key] = null!;  
        }  
    }

    
    public void Read()
    {
        string json = File.ReadAllText(ConfigFilePath);
        _configData = JSON.ToData(json);
    }

    
    public void Save()
    {
        // Json序列化
        string json = JSON.ToJson(_configData);
        
        // Json字符串格式化
        var options = new JsonSerializerOptions  
        {  
            WriteIndented = true  
        }; 
        json = JsonSerializer.Serialize(JsonDocument.Parse(json), options);  

        // 写入文件
        using (StreamWriter writer = new StreamWriter(ConfigFilePath, false))
        {
            writer.WriteLine(json);   
        }

        // 读取JSON
        Read();
    }
}