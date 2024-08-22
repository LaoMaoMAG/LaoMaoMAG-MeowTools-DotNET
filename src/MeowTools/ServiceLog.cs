using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace MeowTools;

public class ServiceLog
{
    // 日志文件夹
    public string LogFolderPath { get; private set; }
    
    // 日志文件路径
    private string? LogFilePath { get; set; }
    
    // 接收日志回调方法
    public Action<Data>? ReceiveLog { get; set; }
    
    // 最大的日志行数
    public int MaxLogLine { get; set; }


    private const int DefaultMaxLogLine = 1000;
    
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum Type
    {
        Message,
        Tip,
        Warning,
        Error
    }
    
    /// <summary>
    /// 日志数据结构体
    /// </summary>
    public class Data
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }

        /// <summary>
        /// 结构体转换文本
        /// </summary>
        /// <returns>日志单行文本</returns>
        public string ToText()
        {
            var time = Time.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var type = Type.ToString();
            var name = Convert.ToBase64String(Encoding.UTF8.GetBytes(Name));
            var content = Convert.ToBase64String(Encoding.UTF8.GetBytes(Content));
            return $"[{time}][{type}][{name}] {content}";
        }

        /// <summary>
        /// 文本转换结构体
        /// </summary>
        /// <param name="text">日志单行文本</param>
        /// <returns>日志单行结构体数据</returns>
        public static Data? ToData(string text)
        {
            // 正则表达式模式，用于匹配方括号中的内容以及方括号外的文本
            string pattern = @"\[(.*?)\](?:\[(.*?)\]\[(.*?)\]) ?(.*)";

            // 使用正则表达式进行匹配
            Match match = Regex.Match(text, pattern);
            if (!match.Success) return null; 
            
            // 数据转换
            var time = DateTime.ParseExact(match.Groups[1].Value, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var type = (Type)Enum.Parse(typeof(Type), match.Groups[2].Value, true); // true 表示忽略大小写
            var name = Encoding.UTF8.GetString(Convert.FromBase64String(match.Groups[3].Value));
            var content = Encoding.UTF8.GetString(Convert.FromBase64String(match.Groups[4].Value));
            
            // 返回结构体
            return new Data
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
    public ServiceLog(string logFolderPath, int maxLogLine = DefaultMaxLogLine)
    {
        // 初始化成员属性
        LogFolderPath = logFolderPath;
        MaxLogLine = maxLogLine;
        
        // 如果文件夹不存在，则创建它
        if (!Directory.Exists(LogFolderPath)) Directory.CreateDirectory(LogFolderPath);
    }
    
    
    /// <summary>
    /// 添加日志
    /// </summary>
    /// <param name="type">日志类型</param>
    /// <param name="name">日志名称</param>
    /// <param name="content">日志内容</param>
    public void AddLog(Type type, string name, string content)
    {
        // 数据存入结构体
        var data = new Data
        {
            Name = name,
            Type = type,
            Content = content,
            Time = DateTime.Now
        };

        // 文件检查
        FileInspect();
        
        // 追加一行文本，自动换行写入
        if (LogFilePath != null)
        {
            using StreamWriter writer = new StreamWriter(LogFilePath, append: true);
            writer.WriteLine(data.ToText());
        }

        // 日志数据传入回调方法
        ReceiveLog?.Invoke(data);
    }


    /// <summary>
    /// 文件检查
    /// </summary>
    private void FileInspect()
    {
        // 如果文件路径不存在者创新文件
        if (LogFilePath == null)
        {
            // 日志文件名
            var time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff");
            LogFilePath = Path.Combine(LogFolderPath, $"{time}.log");
            
            // 寻找不重复的文件名
            var i = 0;
            while (true)
            {
                // 文件不存在，跳出循环
                if (!File.Exists(LogFilePath)) break;
                // 拼接文件名
                i ++;
                LogFilePath = Path.Combine(LogFolderPath, $"{time}-{i}.log");
            }
            
            // 创建文件
            File.WriteAllText(LogFilePath, "");
        }
        
        // 获取文件行数
        int lineCount = 0;
        foreach (var unused in File.ReadLines(LogFilePath)) lineCount++;

        // 如果文件行数大于等于最大行数递归重新创建
        if (MaxLogLine <= lineCount)
        {
            LogFilePath = null;
            FileInspect();
        }
    }
    
    
    /// <summary>
    /// 读取文件路径
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public static List<Data?> ReadLogFile(string filePath)
    {
        var data = new List<Data?>();
        
        // 使用 FileStream 和 StreamReader 来读取文件并设置 FileShare
        using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader reader = new StreamReader(fs);
        while (reader.ReadLine() is { } line)
        {
            data.Add(Data.ToData(line));
        }

        return data;
    }


    /// <summary>
    /// 获取日志文件路径列表
    /// </summary>
    /// <returns>日志文件路径列表</returns>
    public string[] GetFilePathList()
    {
        return Directory.GetFiles(LogFolderPath, "*.log");
    }
}