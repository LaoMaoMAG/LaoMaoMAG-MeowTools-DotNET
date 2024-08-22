using System.Security.Cryptography;
using System.Text;

namespace MeowTools.WebUtility;

/// <summary>
/// 密码加密以及验证，使用SHA256加密和加盐算法
/// </summary>
public class Password
{
    /// <summary>
    /// 加密密码
    /// </summary>
    /// <param name="password">密码</param>
    /// <param name="salt">盐</param>
    /// <returns></returns>
    public static string Encryption(string password, byte[] salt)
    {
        using var hmac = new HMACSHA256(salt);
        byte[] hashMessage = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));  
        return Convert.ToBase64String(hashMessage);
    }  
  
    
    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="inputPassword">密码</param>
    /// <param name="storedHash">密文</param>
    /// <param name="salt">盐</param>
    /// <returns></returns>
    public static bool Verify(string inputPassword, string storedHash, byte[] salt)  
    {  
        var hashedInput = Encryption(inputPassword, salt);  
        return hashedInput == storedHash;  
    }  
  
    
    /// <summary>
    /// 密码加盐
    /// </summary>
    /// <returns></returns>
    public static byte[] GenerateSalt() 
    {  
        const int saltSize = 16; // 你可以根据需要调整这个大小  
        var salt = new byte[saltSize];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        return salt;  
    }  
}