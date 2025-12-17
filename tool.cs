using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace jfglzs_password_tool_v3.Utils
{
    /// <summary>
    /// 注册表操作工具类
    /// </summary>
    public static class RegistryHelper
    {
        private const string REGISTRY_PATH = "Software";

        /// <summary>
        /// 检查注册表项是否存在
        /// </summary>
        /// <param name="valueName">要读取的值名称</param>
        /// <returns>读取到的值，如果不存在返回null</returns>
        public static string GetRegistryValue(string valueName)
        {
            try
            {
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey hkSoftware = hklm.OpenSubKey(REGISTRY_PATH);

                if (hkSoftware != null)
                {
                    return hkSoftware.GetValue(valueName)?.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"读取注册表失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 写入注册表值
        /// </summary>
        /// <param name="valueName">值名称</param>
        /// <param name="value">值</param>
        /// <param name="valueKind">值类型</param>
        public static void SetRegistryValue(string valueName, object value, RegistryValueKind valueKind = RegistryValueKind.String)
        {
            try
            {
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey hkSoftware = hklm.OpenSubKey(REGISTRY_PATH, true);

                if (hkSoftware == null)
                {
                    hkSoftware = hklm.CreateSubKey(REGISTRY_PATH);
                }

                hkSoftware.SetValue(valueName, value, valueKind);
                hkSoftware.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"写入注册表失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 删除注册表值
        /// </summary>
        /// <param name="valueName">值名称</param>
        public static void DeleteRegistryValue(string valueName)
        {
            try
            {
                RegistryKey hklm = Registry.CurrentUser;
                RegistryKey hkSoftware = hklm.OpenSubKey(REGISTRY_PATH, true);

                if (hkSoftware != null)
                {
                    hkSoftware.DeleteValue(valueName, false);
                    hkSoftware.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"删除注册表值失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 检查助手是否已安装
        /// </summary>
        /// <param name="expectedValueName">预期的注册表值名称</param>
        /// <returns>是否已安装</returns>
        public static bool IsAssistantInstalled(string expectedValueName = "n")
        {
            try
            {
                string value = GetRegistryValue(expectedValueName);
                return !string.IsNullOrEmpty(value);
            }
            catch
            {
                return false;
            }
        }
    }
    public static class PasswordGenerator
    {
        /// <summary>
        /// 生成随机标题
        /// </summary>
        /// <returns>生成的标题字符串</returns>
        public static string GenerateTitle()
        {
            Random rand = new Random();
            int r1 = rand.Next(1, 992001);
            int r2 = rand.Next(1, 992001);
            int r3 = rand.Next(1, 992001);
            int colorCount = 256;
            int r4 = rand.Next(colorCount, 100001);
            int r5 = rand.Next(91, 1146);

            int sum = r1 + r2;
            int product = r4 * r5;


            return $"{sum} 机房-管理-助手_密码{r3}工具 {product}";
        }


        /// <summary>
        /// 获取计算机名（对于助手的特征）
        /// </summary>
        /// <returns>计算机名最后一位的ASCII-str</returns>
        public static string GetComputerLastCharAscii()
        {
            string computerName = Environment.MachineName;
            byte asciiValue = (byte)computerName[computerName.Length - 1];
            return asciiValue.ToString();
        }

        /// <summary>
        /// 十进制转七进制
        /// </summary>
        /// <param name="decimalNumber">十进制数字</param>
        /// <returns>七进制字符串</returns>
        public static string DecimalToBase7(int decimalNumber)
        {
            if (decimalNumber == 0) return "0";

            bool isNegative = decimalNumber < 0;
            int number = Math.Abs(decimalNumber);
            string result = "";

            while (number > 0)
            {
                int remainder = number % 7;
                result = remainder.ToString() + result;
                number /= 7;
            }

            return isNegative ? "-" + result : result;
        }

        /// <summary>
        /// 十进制转八进制
        /// </summary>
        /// <param name="decimalNumber">十进制数字</param>
        /// <returns>八进制字符串</returns>
        public static string DecimalToBase8(int decimalNumber)
        {
            if (decimalNumber == 0) return "0";

            bool isNegative = decimalNumber < 0;
            int number = Math.Abs(decimalNumber);
            string octalResult = "";

            while (number > 0)
            {
                int remainder = number % 8;
                octalResult = remainder.ToString() + octalResult;
                number /= 8;
            }

            return isNegative ? "-" + octalResult : octalResult;
        }

        /// <summary>
        /// 根据模式生成密码
        /// </summary>
        /// <param name="mode">模式编号（1-5）</param>
        /// <param name="dateTime">日期时间</param>
        /// <param name="inputText">输入文本（模式1需要）</param>
        /// <returns>生成的密码</returns>
        public static string GeneratePassword(int mode, DateTime dateTime, string inputText = "")
        {
            switch (mode)
            {
                case 1:
                    //助手1106+特征
                    int temp1 = dateTime.Month * 159 + dateTime.Day * 357 + int.Parse(inputText) * 258;
                    return DecimalToBase7(temp1);

                case 2:
                    //助手1106-
                    int temp2 = dateTime.Month * 123 + dateTime.Day * 456 + dateTime.Year * 789 + 111;
                    return DecimalToBase8(temp2);

                case 3:
                    //助手1100+
                    int temp3 = dateTime.Month * 123 + dateTime.Day * 456 + dateTime.Year * 789 + 111;
                    return temp3.ToString();

                case 4:
                    //助手1010+
                    int temp4 = (dateTime.Month * 13 + dateTime.Day * 57 + dateTime.Year * 91) * 16 + 11;
                    return "8" + temp4.ToString();

                case 5:
                    //助手1010-
                    int temp5 = (dateTime.Month * 13 + dateTime.Day * 57 + dateTime.Year * 91) * 16;
                    return "8" + temp5.ToString();

                default:
                    return "E";
            }
        }
    }

    /// <summary>
    /// 优化截断解密器
    /// </summary>
    public class OptimizedTruncatedDecryptor
    {
        private const string WINDOWS_PATH = "C:\\WINDOWS";
        private const int TRUNCATE_COUNT = 2;

        /// <summary>
        /// 解密截断的加密文本
        /// </summary>
        /// <param name="truncatedEncrypted">截断的加密文本</param>
        /// <returns>解密后的原文</returns>
        public static string DecryptTruncatedOptimized(string truncatedEncrypted)
        {
            List<int> likelyFirstChars = new List<int>();
            List<int> likelyLastChars = new List<int>();

            for (int c = 32; c <= 126; c++)
            {
                int originalChar = c + 10;
                if (IsBase64Char(originalChar))
                {
                    likelyFirstChars.Add(c);
                    likelyLastChars.Add(c);
                }
            }

            foreach (int firstChar in likelyFirstChars)
            {
                foreach (int lastChar in likelyLastChars)
                {
                    try
                    {
                        string fullEncrypted = $"{(char)firstChar}{truncatedEncrypted}{(char)lastChar}";
                        string decrypted = DecryptFull(fullEncrypted);

                        if (IsLikelyPlaintext(decrypted))
                        {
                            return decrypted;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return DecryptTruncatedFallback(truncatedEncrypted);
        }

        /// <summary>
        /// 回退方法：完整搜索解密
        /// </summary>
        private static string DecryptTruncatedFallback(string truncatedEncrypted)
        {
            for (int firstChar = 32; firstChar <= 126; firstChar++)
            {
                for (int lastChar = 32; lastChar <= 126; lastChar++)
                {
                    try
                    {
                        string fullEncrypted = $"{(char)firstChar}{truncatedEncrypted}{(char)lastChar}";
                        string decrypted = DecryptFull(fullEncrypted);
                        return decrypted;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            throw new ArgumentException("解密失败：无法找到有效的首尾字符");
        }

        /// <summary>
        /// 检查是否为Base64字符
        /// </summary>
        private static bool IsBase64Char(int asciiCode)
        {
            char c = (char)asciiCode;
            return (c >= 'A' && c <= 'Z') ||
                   (c >= 'a' && c <= 'z') ||
                   (c >= '0' && c <= '9') ||
                   c == '+' || c == '/' || c == '=';
        }

        /// <summary>
        /// 检查是否为可能的明文
        /// </summary>
        private static bool IsLikelyPlaintext(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length > 1000)
                return false;

            int printableCount = 0;
            foreach (char c in text)
            {
                if (c >= 32 && c <= 126 || c == '\n' || c == '\r' || c == '\t')
                    printableCount++;
            }

            return (double)printableCount / text.Length > 0.95;
        }

        /// <summary>
        /// 完整解密流程
        /// </summary>
        private static string DecryptFull(string encryptedStr)
        {
            string base64Str = ReverseCharacterShift(encryptedStr);
            string key = WINDOWS_PATH.Substring(0, 8);
            string iv = WINDOWS_PATH.Substring(1, 8);

            byte[] encryptedData = Convert.FromBase64String(base64Str);
            return DESDecrypt(encryptedData, key, iv);
        }

        /// <summary>
        /// 逆向字符位移
        /// </summary>
        private static string ReverseCharacterShift(string input)
        {
            char[] chars = input.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(chars[i] + 10);
            }
            return new string(chars);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        private static string DESDecrypt(byte[] encryptedData, string key, string iv)
        {
            using (DES des = DES.Create())
            {
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(iv);
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = des.CreateDecryptor())
                {
                    byte[] decrypted = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                    return Encoding.UTF8.GetString(decrypted);
                }
            }
        }
    }
    public class EncryptZSPassword
    {
        // 精简版加密方法 - 输出截短前后各一个字符
        public static string CompactEncrypt(string input)
        {
            // DES加密
            string key = "C:\\WINDOWS".Substring(0, 8);
            string iv = "C:\\WINDOWS".Substring(1, 8);

            byte[] encryptedData = DESEncrypt(input, key, iv);

            // Base64编码
            string base64Str = Convert.ToBase64String(encryptedData);

            // 字符位移
            string shifted = RateIssuer(base64Str);

            // 截短：去掉第一个和最后一个字符
            if (shifted.Length >= 2)
            {
                return shifted.Substring(1, shifted.Length - 2);
            }

            return shifted; // 如果长度不足2，返回原字符串
        }

        // 字符位移加密
        private static string RateIssuer(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                result.Append((char)(c - 10));
            }
            return result.ToString();
        }

        // DES加密
        private static byte[] DESEncrypt(string input, string key, string iv)
        {
            using (DES des = DES.Create())
            {
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(iv);
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;

                using (var encryptor = des.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(input);
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }
    }
    public class Tools
    {
        public static bool HasSpecialCharacters(string input, string allowedChars = "")
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // 定义基本模式：字母、数字、空格和中文
            string pattern = @"^[\p{L}\p{N}\s" + Regex.Escape(allowedChars) + @"]+$";

            // Unicode类别说明：
            // \p{L} - 任何语言的字母（包括中文、英文、日文等）
            // \p{N} - 任何数字（包括中文数字、罗马数字等）
            // \s - 空白字符（空格、制表符、换行等）

            return !Regex.IsMatch(input, pattern);
        }
    }




/// <summary>
/// 机房管理助手检测类
/// 用于检测和退出机房管理助手的各种版本
/// </summary>
public class ComputerRoomManagerDetector
    {
        #region 静态变量（对应易语言的匿名程序集变量）

        private static string anonymousAssemblyVar1 = string.Empty;  // 安装目录
        private static string anonymousAssemblyVar2 = string.Empty;  // prozs文件名
        private static long anonymousAssemblyVar3 = 0;               // 文件大小特征
        private static string anonymousAssemblyVar4 = string.Empty;  // 文件MD5哈希特征
        private static string anonymousAssemblyVar5 = string.Empty;  // 服务程序路径

        private static int anonymousGlobalVar1 = 0;                  // 服务检测状态

        private static readonly HashSet<string> excludedDirectories = new HashSet<string>
    {
        "KuGou", "PerfLogs", "Program Files", "Program Files (x86)", "Users", "Windows"
    };

        #endregion

        #region 公共方法

        /// <summary>
        /// ECK_机房管理助手_安装检测
        /// 返回安装目录
        /// </summary>
        public static string InstallationDetection()
        {
            try
            {
                DebugOutput("[安装检测] 开始检测安装目录");

                string cDrive = @"C:\";

                if (!Directory.Exists(cDrive))
                {
                    DebugOutput("[安装检测] C盘不存在");
                    return string.Empty;
                }

                // 获取C盘下的所有目录
                string[] directories = Directory.GetDirectories(cDrive);
                DebugOutput($"[安装检测] 发现 {directories.Length} 个目录");

                foreach (string directory in directories)
                {
                    try
                    {
                        string dirName = Path.GetFileName(directory);

                        // 跳过排除的目录
                        if (excludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        // 检查目录下是否有特定的exe文件
                        string[] exeFiles = Directory.GetFiles(directory, "*.exe", SearchOption.TopDirectoryOnly);

                        bool hasZmserv = false;
                        bool hasJfglzs = false;

                        foreach (string exeFile in exeFiles)
                        {
                            string fileName = Path.GetFileName(exeFile);
                            if (fileName.Equals("zmserv.exe", StringComparison.OrdinalIgnoreCase))
                            {
                                hasZmserv = true;
                            }
                            else if (fileName.Equals("jfglzs.exe", StringComparison.OrdinalIgnoreCase))
                            {
                                hasJfglzs = true;
                            }

                            if (hasZmserv && hasJfglzs)
                            {
                                break;
                            }
                        }

                        if (hasZmserv && hasJfglzs)
                        {
                            anonymousAssemblyVar1 = directory;
                            DebugOutput($"[安装检测] 已找到安装目录: {anonymousAssemblyVar1}");
                            return anonymousAssemblyVar1;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // 无权限访问的目录，跳过
                        continue;
                    }
                    catch (Exception ex)
                    {
                        DebugOutput($"[安装检测] 检查目录 {directory} 时出错: {ex.Message}");
                        continue;
                    }
                }

                // 如果未找到安装目录，尝试通过服务检测
                if (string.IsNullOrEmpty(anonymousAssemblyVar5))
                {
                    ServiceDetection();
                }

                if (!string.IsNullOrEmpty(anonymousAssemblyVar5))
                {
                    anonymousAssemblyVar1 = Path.GetDirectoryName(anonymousAssemblyVar5);
                    return anonymousAssemblyVar1;
                }

                DebugOutput("[安装检测] 未发现安装目录");
                return string.Empty;
            }
            catch (Exception ex)
            {
                DebugOutput($"[安装检测] 发生异常: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// ECK_机房管理助手_随机进程检测1
        /// 检测安装目录下的随机进程，返回随机进程路径。适用7.1及以下版本
        /// </summary>
        public static string RandomProcessDetection1()
        {
            try
            {
                DebugOutput("[随机进程检测1] 开始检测");

                if (string.IsNullOrEmpty(anonymousAssemblyVar1) &&
                    string.IsNullOrEmpty(InstallationDetection()))
                {
                    DebugOutput("[随机进程检测1] 未找到安装目录");
                    return string.Empty;
                }

                if (!GetProzsFileFeature())
                {
                    return string.Empty;
                }

                if (!Directory.Exists(anonymousAssemblyVar1))
                {
                    DebugOutput($"[随机进程检测1] 安装目录不存在: {anonymousAssemblyVar1}");
                    return string.Empty;
                }

                string[] exeFiles = Directory.GetFiles(anonymousAssemblyVar1, "*.exe", SearchOption.TopDirectoryOnly);

                foreach (string exeFile in exeFiles)
                {
                    string fileName = Path.GetFileName(exeFile);

                    // 检查文件名长度是否为7
                    if (fileName.Length == 7)
                    {
                        string fileHash = CalculateFileHash(exeFile);
                        if (fileHash == anonymousAssemblyVar4)
                        {
                            DebugOutput($"[随机进程检测1] 已找到: {exeFile}");
                            return exeFile;
                        }
                    }
                }

                DebugOutput("[随机进程检测1] 未发现");
                return string.Empty;
            }
            catch (Exception ex)
            {
                DebugOutput($"[随机进程检测1] 发生异常: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// ECK_机房管理助手_随机进程检测2
        /// 返回随机进程路径，根据文件夹名称长度特征搜索 Program Files 子目录中的随机进程。
        /// 适用 [7.2, 9.95) 版本区间
        /// </summary>
        public static string RandomProcessDetection2()
        {
            try
            {
                DebugOutput("[随机进程检测2] 开始检测");

                if (string.IsNullOrEmpty(anonymousAssemblyVar1) &&
                    string.IsNullOrEmpty(InstallationDetection()))
                {
                    DebugOutput("[随机进程检测2] 未找到安装目录");
                    return string.Empty;
                }

                DebugOutput($"[随机进程检测2] prozs_size: {anonymousAssemblyVar3}, prozs_md5: {anonymousAssemblyVar4}");

                if (!GetProzsFileFeature())
                {
                    return string.Empty;
                }

                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                return CheckProgramFilesWithPattern(programFilesPath);
            }
            catch (Exception ex)
            {
                DebugOutput($"[随机进程检测2] 发生异常: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// ECK_机房管理助手_随机进程检测3
        /// 返回随机进程路径，适用范围较广。本函数遍历进程列表，识别prozs复制体。
        /// 适用7.x及以后的所有版本
        /// </summary>
        public static string RandomProcessDetection3()
        {
            try
            {
                DebugOutput("[随机进程检测3] 开始检测");

                if (string.IsNullOrEmpty(anonymousAssemblyVar1) &&
                    string.IsNullOrEmpty(InstallationDetection()))
                {
                    DebugOutput("[随机进程检测3] 未找到安装目录");
                    return string.Empty;
                }

                DebugOutput($"[随机进程检测3] prozs_size: {anonymousAssemblyVar3}, prozs_md5: {anonymousAssemblyVar4}");

                if (!GetProzsFileFeature())
                {
                    return string.Empty;
                }

                Process[] processes = Process.GetProcesses();

                foreach (Process process in processes)
                {
                    try
                    {
                        string processName = process.ProcessName;

                        // 检查进程名长度特征
                        if (processName.Length >= 7 && processName.Length <= 15)
                        {
                            string processPath = GetProcessPath(process);
                            if (!string.IsNullOrEmpty(processPath))
                            {
                                FileInfo fileInfo = new FileInfo(processPath);
                                if (fileInfo.Length == anonymousAssemblyVar3)
                                {
                                    string fileHash = CalculateFileHash(processPath);
                                    if (fileHash == anonymousAssemblyVar4)
                                    {
                                        // 检查不是安装目录下的原始文件
                                        string originalFile = Path.Combine(anonymousAssemblyVar1, anonymousAssemblyVar2);
                                        if (!string.Equals(processPath.Trim(), originalFile.Trim(), StringComparison.OrdinalIgnoreCase))
                                        {
                                            DebugOutput($"[随机进程检测3] 已找到: {processPath}");
                                            return processPath;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugOutput($"[随机进程检测3] 检查进程 {process.ProcessName} 时出错: {ex.Message}");
                        continue;
                    }
                }

                DebugOutput("[随机进程检测3] 未发现");
                return string.Empty;
            }
            catch (Exception ex)
            {
                DebugOutput($"[随机进程检测3] 发生异常: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// ECK_机房管理助手_随机进程检测4
        /// 返回随机进程路径，本函数遍历Program Files和Program Files (x86)目录，
        /// 对只有一个exe文件的目录进行判别。适用7.2及以上版本
        /// </summary>
        public static string RandomProcessDetection4()
        {
            try
            {
                DebugOutput("[随机进程检测4] 开始检测");

                if (string.IsNullOrEmpty(anonymousAssemblyVar1) &&
                    string.IsNullOrEmpty(InstallationDetection()))
                {
                    DebugOutput("[随机进程检测4] 未找到安装目录");
                    return string.Empty;
                }

                if (!GetProzsFileFeature())
                {
                    return string.Empty;
                }

                // 检查 Program Files 目录
                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string result = CheckSingleExeDirectory(programFilesPath);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }

                // 检查 Program Files (x86) 目录
                if (Environment.Is64BitOperatingSystem)
                {
                    string programFilesX86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                    if (!string.IsNullOrEmpty(programFilesX86Path))
                    {
                        result = CheckSingleExeDirectory(programFilesX86Path);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                }

                DebugOutput("[随机进程检测4] 未发现");
                return string.Empty;
            }
            catch (Exception ex)
            {
                DebugOutput($"[随机进程检测4] 发生异常: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// ECK_机房管理助手_随机进程检测5
        /// 返回随机进程路径，本函数检测助手的编号显示窗口。适用9.9x及以上版本
        /// </summary>
        public static string RandomProcessDetection5()
        {
            // 这个方法已经在前面的代码中实现
            // 这里只是占位符，实际实现需要完整的Windows API封装
            DebugOutput("[随机进程检测5] 开始检测");

            try
            {
                string computerName = Environment.MachineName;

                // 这里需要实现窗口枚举和检测逻辑
                // 由于代码较长，这里只提供框架

                // 实际实现需要调用Windows API枚举窗口
                // 查找类名为"bianhao"的窗口
                // 然后检查子窗口标题是否包含计算机名

                DebugOutput("[随机进程检测5] 功能需要完整的Windows API实现");
                return string.Empty;
            }
            catch (Exception ex)
            {
                DebugOutput($"[随机进程检测5] 发生异常: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// ECK_机房管理助手_退出
        /// 结束所有检测到的机房管理助手进程
        /// </summary>
        public static void ExitComputerRoomManager()
        {
            try
            {
                DebugOutput("[退出] 开始结束机房管理助手进程");

                HashSet<string> processNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "jfglzs.exe",
                "jfglzsp.exe"
            };

                // 收集所有检测到的进程名
                string[] detectionMethods = new string[]
                {
                RandomProcessDetection1(),
                RandomProcessDetection2(),
                RandomProcessDetection3(),
                RandomProcessDetection4()
                };

                foreach (string processPath in detectionMethods)
                {
                    if (!string.IsNullOrEmpty(processPath))
                    {
                        string fileName = Path.GetFileName(processPath);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            processNames.Add(fileName);
                        }
                    }
                }

                DebugOutput($"[退出] 需要结束的进程: {string.Join(", ", processNames)}");

                // 结束所有检测到的进程
                foreach (string processName in processNames)
                {
                    int retryCount = 0;
                    while (retryCount < 10)
                    {
                        Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName));
                        if (processes.Length == 0)
                        {
                            break;
                        }

                        foreach (Process process in processes)
                        {
                            try
                            {
                                process.Kill();
                                process.WaitForExit(1000);
                                DebugOutput($"[退出] 已结束进程: {process.ProcessName}");
                            }
                            catch (Exception ex)
                            {
                                DebugOutput($"[退出] 结束进程 {process.ProcessName} 失败: {ex.Message}");
                            }
                        }

                        retryCount++;
                        System.Threading.Thread.Sleep(200);
                    }
                }

                // 停止服务
                StopService("zmserv");
                System.Threading.Thread.Sleep(3000);

                // 确保zmserv进程结束
                int serviceRetry = 0;
                while (serviceRetry < 10)
                {
                    Process[] zmservProcesses = Process.GetProcessesByName("zmserv");
                    if (zmservProcesses.Length == 0)
                    {
                        break;
                    }

                    foreach (Process process in zmservProcesses)
                    {
                        try
                        {
                            process.Kill();
                            process.WaitForExit(1000);
                        }
                        catch { }
                    }

                    serviceRetry++;
                    System.Threading.Thread.Sleep(200);
                }

                StopService("zmserv");

                DebugOutput("[退出] 机房管理助手退出完成");
            }
            catch (Exception ex)
            {
                DebugOutput($"[退出] 发生异常: {ex.Message}");
            }
        }

        #endregion

        #region 私有辅助方法

        /// <summary>
        /// ECK_机房管理助手_服务检测
        /// 返回服务程序路径
        /// </summary>
        private static string ServiceDetection()
        {
            try
            {
                anonymousGlobalVar1 = 1;
                string servicePath = string.Empty;

                // 尝试从注册表获取服务路径
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\zmserv\Parameters"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("Application");
                        if (value != null)
                        {
                            servicePath = value.ToString();
                        }
                    }
                }

                if (string.IsNullOrEmpty(servicePath))
                {
                    anonymousGlobalVar1 = 2;
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\zmserv"))
                    {
                        if (key != null)
                        {
                            object value = key.GetValue("ImagePath");
                            if (value != null)
                            {
                                servicePath = value.ToString();
                                // 清理路径（可能包含引号）
                                servicePath = servicePath.Trim('"');
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(servicePath))
                {
                    DebugOutput("[服务检测] 未发现");
                    anonymousGlobalVar1 = 0;
                }
                else
                {
                    DebugOutput($"[服务检测] 已找到服务程序: {servicePath}");
                    anonymousAssemblyVar5 = servicePath;
                }

                return servicePath;
            }
            catch (Exception ex)
            {
                DebugOutput($"[服务检测] 发生异常: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// ECK_机房管理助手_获取prozs文件特征
        /// </summary>
        private static bool GetProzsFileFeature()
        {
            try
            {
                if (string.IsNullOrEmpty(anonymousAssemblyVar1))
                {
                    DebugOutput("[获取prozs文件特征] 安装目录为空");
                    return false;
                }

                anonymousAssemblyVar2 = GetProzsFileName();
                if (string.IsNullOrEmpty(anonymousAssemblyVar2))
                {
                    DebugOutput("[获取prozs文件特征] 未找到prozs文件名");
                    return false;
                }

                string prozsFilePath = Path.Combine(anonymousAssemblyVar1, anonymousAssemblyVar2);

                if (!File.Exists(prozsFilePath))
                {
                    DebugOutput($"[获取prozs文件特征] 文件不存在: {prozsFilePath}");
                    return false;
                }

                FileInfo fileInfo = new FileInfo(prozsFilePath);
                anonymousAssemblyVar3 = fileInfo.Length;
                DebugOutput($"[获取prozs文件特征] prozs_size: {anonymousAssemblyVar3}");

                if (anonymousAssemblyVar3 <= 0)
                {
                    DebugOutput("[获取prozs文件特征] 文件大小为0");
                    return false;
                }

                anonymousAssemblyVar4 = CalculateFileHash(prozsFilePath);
                DebugOutput($"[获取prozs文件特征] prozs_md5: {anonymousAssemblyVar4}");

                if (string.IsNullOrEmpty(anonymousAssemblyVar4))
                {
                    DebugOutput("[获取prozs文件特征] 计算MD5失败");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                DebugOutput($"[获取prozs文件特征] 发生异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ECK_机房管理助手_获取prozs文件名
        /// </summary>
        private static string GetProzsFileName()
        {
            try
            {
                if (string.IsNullOrEmpty(anonymousAssemblyVar1))
                {
                    return string.Empty;
                }

                // 检查prozs.exe
                if (anonymousGlobalVar1 != 2 && File.Exists(Path.Combine(anonymousAssemblyVar1, "prozs.exe")))
                {
                    return "prozs.exe";
                }

                // 检查przs.exe
                if (File.Exists(Path.Combine(anonymousAssemblyVar1, "przs.exe")))
                {
                    return "przs.exe";
                }

                // 枚举所有exe文件，检查文件版本信息
                string[] exeFiles = Directory.GetFiles(anonymousAssemblyVar1, "*.exe", SearchOption.TopDirectoryOnly);

                foreach (string exeFile in exeFiles)
                {
                    try
                    {
                        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(exeFile);

                        if (versionInfo.FileDescription?.Trim() == "Windows 服务主进程" &&
                            versionInfo.LegalTrademarks?.Trim() == "z")
                        {
                            return Path.GetFileName(exeFile);
                        }
                    }
                    catch
                    {
                        // 跳过无法读取版本信息的文件
                        continue;
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                DebugOutput($"[获取prozs文件名] 发生异常: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 检查Program Files目录（带模式匹配）
        /// </summary>
        private static string CheckProgramFilesWithPattern(string basePath)
        {
            try
            {
                if (!Directory.Exists(basePath))
                {
                    DebugOutput($"[随机进程检测2] 目录不存在: {basePath}");
                    return string.Empty;
                }

                string[] subDirectories = Directory.GetDirectories(basePath);
                DebugOutput($"[随机进程检测2] 发现 {subDirectories.Length} 个目录");

                foreach (string subDir in subDirectories)
                {
                    try
                    {
                        string dirName = Path.GetFileName(subDir);
                        int dirNameLength = dirName.Length;

                        bool shouldCheck = false;

                        // 模式匹配
                        if (dirNameLength >= 4 && dirName.Substring(0, 4).Equals("temp", StringComparison.OrdinalIgnoreCase) && dirNameLength == 7)
                        {
                            shouldCheck = true;
                        }
                        else if (dirNameLength >= 2 && dirName.Substring(0, 2).Equals("pr", StringComparison.OrdinalIgnoreCase) && dirNameLength == 5)
                        {
                            shouldCheck = true;
                        }
                        else if (dirNameLength == 4 || dirNameLength == 3)
                        {
                            shouldCheck = true;
                        }

                        if (shouldCheck)
                        {
                            string[] exeFiles = Directory.GetFiles(subDir, "*.exe", SearchOption.TopDirectoryOnly);

                            foreach (string exeFile in exeFiles)
                            {
                                string fileName = Path.GetFileName(exeFile);
                                int fileNameLength = fileName.Length;

                                if (fileNameLength == 8 || fileNameLength == 9 || fileNameLength == 11 || fileNameLength == 14)
                                {
                                    FileInfo fileInfo = new FileInfo(exeFile);
                                    if (fileInfo.Length == anonymousAssemblyVar3)
                                    {
                                        string fileHash = CalculateFileHash(exeFile);
                                        if (fileHash == anonymousAssemblyVar4)
                                        {
                                            DebugOutput($"[随机进程检测2] 已找到: {exeFile}");
                                            return exeFile;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        DebugOutput($"[随机进程检测2] 检查目录 {subDir} 时出错: {ex.Message}");
                        continue;
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                DebugOutput($"[随机进程检测2] 检查目录 {basePath} 时出错: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 检查只有一个exe文件的目录
        /// </summary>
        private static string CheckSingleExeDirectory(string basePath)
        {
            try
            {
                if (!Directory.Exists(basePath))
                {
                    return string.Empty;
                }

                DebugOutput($"[随机进程检测4] 扫描目录: {basePath}");

                string[] subDirectories = Directory.GetDirectories(basePath);
                DebugOutput($"[随机进程检测4] 发现 {subDirectories.Length} 个目录");

                foreach (string subDir in subDirectories)
                {
                    try
                    {
                        string[] exeFiles = Directory.GetFiles(subDir, "*.exe", SearchOption.TopDirectoryOnly);

                        if (exeFiles.Length != 1)
                        {
                            continue;
                        }

                        string exeFile = exeFiles[0];
                        FileInfo fileInfo = new FileInfo(exeFile);

                        if (fileInfo.Length == anonymousAssemblyVar3)
                        {
                            string fileHash = CalculateFileHash(exeFile);
                            if (fileHash == anonymousAssemblyVar4)
                            {
                                DebugOutput($"[随机进程检测4] 已找到: {exeFile}");
                                return exeFile;
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        DebugOutput($"[随机进程检测4] 检查目录 {subDir} 时出错: {ex.Message}");
                        continue;
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                DebugOutput($"[随机进程检测4] 检查目录 {basePath} 时出错: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 计算文件MD5哈希
        /// </summary>
        private static string CalculateFileHash(string filePath)
        {
            try
            {
                using (FileStream stream = File.OpenRead(filePath))
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch (Exception ex)
            {
                DebugOutput($"[计算文件哈希] 失败 {filePath}: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取进程路径
        /// </summary>
        private static string GetProcessPath(Process process)
        {
            try
            {
                return process.MainModule?.FileName ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        private static void StopService(string serviceName)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "net";
                process.StartInfo.Arguments = $"stop {serviceName}";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit(5000);
                DebugOutput($"[停止服务] 已尝试停止服务: {serviceName}");
            }
            catch (Exception ex)
            {
                DebugOutput($"[停止服务] 停止服务 {serviceName} 失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 文本比对
        /// </summary>
        private static bool TextCompare(string text1, string text2, bool caseSensitive = false)
        {
            if (caseSensitive)
            {
                return text1 == text2;
            }
            else
            {
                return string.Equals(text1, text2, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// 调试输出
        /// </summary>
        private static void DebugOutput(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        #endregion
    }
    public static class HostsFileReset
    {
        private const string DefaultHostsContent = @"# Copyright (c) 1993-2009 Microsoft Corp.
#
# This is a sample HOSTS file used by Microsoft TCP/IP for Windows.
#
# This file contains the mappings of IP addresses to host names. Each
# entry should be kept on an individual line. The IP address should
# be placed in the first column followed by the corresponding host name.
# The IP address and the host name should be separated by at least one
# space.
#
# Additionally, comments (such as these) may be inserted on individual
# lines or following the machine name denoted by a '#' symbol.
#
# For example:
#
#      102.54.94.97     rhino.acme.com          # source server
#       38.25.63.10     x.acme.com              # x client host

# localhost name resolution is handled within DNS itself.
# 127.0.0.1       localhost
# ::1             localhost";

        // Windows API 声明
        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern uint DnsFlushResolverCache();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowsDirectory(StringBuilder lpBuffer, int uSize);

        public static bool ResetHosts()
        {
            try
            {
                Console.Write(" -> 检查文件是否存在...\n");

                // 获取 hosts 文件路径
                string hostsPath = GetHostsFilePath();

                if (!File.Exists(hostsPath))
                {
                    Console.Write(" (!) hosts 文件不存在.\n");
                    return false;
                }

                Console.Write(" -> 获取原文件权限...\n");
                FileAttributes originalAttributes = File.GetAttributes(hostsPath);

                Console.Write(" -> 设置文件权限...\n");
                // 移除只读属性
                File.SetAttributes(hostsPath, originalAttributes & ~FileAttributes.ReadOnly);

                Console.Write(" -> 重置文件...\n");

                // 备份原文件（可选）
                string backupPath = hostsPath + ".backup";
                if (!File.Exists(backupPath))
                {
                    File.Copy(hostsPath, backupPath, true);
                }

                // 写入默认内容
                using (FileStream fs = new FileStream(hostsPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    sw.Write(DefaultHostsContent);
                    sw.Flush();
                }

                if (!VerifyHostsFile(hostsPath))
                {
                    Console.Write("\n (!) 重置失败, 无法写入.\n\n");

                    // 尝试恢复备份
                    if (File.Exists(backupPath))
                    {
                        File.Copy(backupPath, hostsPath, true);
                    }

                    return false;
                }

                Console.Write(" -> 恢复文件权限...\n");
                File.SetAttributes(hostsPath, originalAttributes);

                Console.Write(" -> 刷新 DNS 缓存...\n");
                bool dnsFlushed = FlushDnsCache();

                if (!dnsFlushed)
                {
                    Console.Write("\n (!) 刷新 DNS 失败.\n\n");
                }

                Console.Write(" -> hosts 文件重置成功！\n");
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                Console.Write("\n (!) 权限不足，请以管理员身份运行此程序.\n\n");
                return false;
            }
            catch (IOException ex)
            {
                Console.Write($"\n (!) 文件操作失败: {ex.Message}\n\n");
                return false;
            }
            catch (Exception ex)
            {
                Console.Write($"\n (!) 重置失败: {ex.Message}\n\n");
                return false;
            }
        }

        private static string GetHostsFilePath()
        {
            // 获取 Windows 目录
            StringBuilder windowsDir = new StringBuilder(260);
            GetWindowsDirectory(windowsDir, windowsDir.Capacity);

            return Path.Combine(windowsDir.ToString(), @"System32\drivers\etc\hosts");
        }

        private static bool VerifyHostsFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                string content = File.ReadAllText(filePath);
                return content.Contains("# Copyright (c) 1993-2009 Microsoft Corp.");
            }
            catch
            {
                return false;
            }
        }

        private static bool FlushDnsCache()
        {
            try
            {
                uint result = DnsFlushResolverCache();
                return result == 0; // 返回 0 表示成功
            }
            catch (DllNotFoundException)
            {
                Console.Write(" (!) 找不到 dnsapi.dll\n");
                return false;
            }
            catch (EntryPointNotFoundException)
            {
                Console.Write(" (!) 找不到 DnsFlushResolverCache 函数\n");
                return false;
            }
            catch
            {
                return false;
            }
        }

        // 更完整的权限处理方法
        private static void SetFullControlPermissions(string filePath)
        {
            try
            {
                // .NET 4.8 中可以使用 System.Security.AccessControl 处理更复杂的权限
                // 但对于简单的只读属性，上面的方法已经足够
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                }
            }
            catch
            {
                // 忽略权限设置错误
            }
        }
    }

    /// <summary>
    /// 机房管理助手工具类 - DLL 封装
    /// </summary>
    public class JfglzsHelper
    {
        // DLL 导入
        private const string DLL_NAME = "g.dll"; // 请替换为实际的DLL文件名

        /// <summary>
        /// 获取守护进程ID
        /// </summary>
        /// <returns>进程ID，如果失败返回0</returns>
        [DllImport(DLL_NAME, EntryPoint = "GetDaemonProcessID", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetDaemonProcessID();

        /// <summary>
        /// 获取机房管理助手主进程ID
        /// </summary>
        /// <returns>进程ID，如果失败返回0</returns>
        [DllImport(DLL_NAME, EntryPoint = "GetJfglzsnProcessID", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetJfglzsmProcessID();

        /// <summary>
        /// 获取服务进程ID
        /// </summary>
        /// <returns>进程ID，如果失败返回0</returns>
        [DllImport(DLL_NAME, EntryPoint = "GetServiceProcessId", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetServiceProcessId();

        /// <summary>
        /// 清理辅助进程
        /// </summary>
        /// <returns>成功清理的进程数量</returns>
        [DllImport(DLL_NAME, EntryPoint = "HelperCleanupProcesses", CallingConvention = CallingConvention.Cdecl)]
        public static extern int HelperCleanupProcesses();

        /// <summary>
        /// 终止进程扩展方法
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="force">是否强制终止</param>
        /// <returns>是否成功</returns>
        [DllImport(DLL_NAME, EntryPoint = "KillProcessEx", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool KillProcessEx(uint processId, bool force);

        /// <summary>
        /// 移除浏览器限制
        /// </summary>
        /// <returns>是否成功</returns>
        [DllImport(DLL_NAME, EntryPoint = "RemoveBrowserRestrictions", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RemoveBrowserRestrictions();

        // 可选：如果DLL有字符串参数或需要复杂的调用，可以添加更多封装
    }
}

 
