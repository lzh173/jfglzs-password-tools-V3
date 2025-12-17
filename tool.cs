using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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


        /// 获取计算机名（对于助手的特征）
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
}