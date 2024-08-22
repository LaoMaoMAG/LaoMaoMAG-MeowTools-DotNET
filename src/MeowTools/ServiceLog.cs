using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TGCC;

public class ServiceLog
{
    // 日志文件夹
    public string LogFolderPath { get; private set; }
    // 日志文件路径
    public string LogFilePath { get; private set; }
    // 接收日志回调方法
    public Action<LogData> ReceiveLog { get; set; }
    
    
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        Message,
        Tip,
        Warning,
        Error
    }
    
    /// <summary>
    /// 日志数据结构体
    /// </summary>
    public class LogData
    {
        public string Name { get; set; }
        public LogType Type { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }

        /// <summary>
        /// 结构体转换文本
        /// </summary>
        /// <param name="data">日志单行结构体数据</param>
        /// <returns>日志单行文本</returns>
        public static string ToText(LogData data)
        {
            var time = data.Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var type = data.Type.ToString();
            var name = Convert.ToBase64String(Encoding.UTF8.GetBytes(data.Name));
            var content = Convert.ToBase64String(Encoding.UTF8.GetBytes(data.Content));
            return $"[{time}][{type}][{name}]<content>{content}</content>";
        }

        /// <summary>
        /// 文本转换结构体
        /// </summary>
        /// <param name="text">日志单行文本</param>
        /// <returns>日志单行结构体数据</returns>
        public static LogData? ToData(string text)
        {
            // 正则表达式模式，用于匹配方括号中的内容以及方括号外的文本
            string pattern = @"\[(.*?)\](?:\[(.*?)\]\[(.*?)\])?(.*)";

            // 使用正则表达式进行匹配
            Match match = Regex.Match(text, pattern);
            if (!match.Success) return null; 
            
            // 数据转换
            var time = DateTime.ParseExact(match.Groups[1].Value, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var type = (LogType)Enum.Parse(typeof(LogType), match.Groups[2].Value, true); // true 表示忽略大小写
            var name = Encoding.UTF8.GetString(Convert.FromBase64String(match.Groups[3].Value));
            var content = Encoding.UTF8.GetString(Convert.FromBase64String(match.Groups[4].Value));
            
            // 返回结构体
            return new LogData
            {
                Name = name,
                Type = type,
                Content = content,
                Time = time
            };
        }
    }
    
    
    /// <summary>
    /// 构造方法
    /// </summary>
    public ServiceLog(string logFilePath)
    {
        // 初始化成员属性
        LogFilePath = logFilePath;
        
        // 文件不存在，创建文件
        if (!File.Exists(LogFilePath)) File.WriteAllText(LogFilePath, "");
    }

    
    /// <summary>
    /// 添加日志
    /// </summary>
    /// <param name="type">日志类型</param>
    /// <param name="name">日志名称</param>
    /// <param name="content">日志内容</param>
    public void AddLog(LogType type, string name, string content)
    {
        // 数据存入结构体
        var data = new LogData
        {
            Name = name,
            Type = type,
            Content = content,
            Time = DateTime.Now
        };
        
        // 追加一行文本，自动换行写入
        using StreamWriter writer = new StreamWriter(LogFilePath, append: true);
        writer.WriteLine(LogData.ToText(data)); 
        
        // 日志数据传入回调方法
        ReceiveLog(data);
    }

    
    /// <summary>
    /// 读取文件路径
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public static void ReadLogFile(string filePath)
    {
        // 读取整个文件内容
        string content = File.ReadAllText(filePath);
        
    }
}