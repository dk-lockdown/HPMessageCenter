using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageCenter.Framework.Encryption
{
    public class RC4Encrypt
    {
        public enum EncoderMode
        {
            Base64Encoder,
            HexEncoder
        }
        private static System.Text.Encoding Encode = System.Text.Encoding.Default;
        public static string Encrypt(string data, string pass, RC4Encrypt.EncoderMode em)
        {
            string result;
            if (data == null || pass == null)
            {
                result = null;
            }
            else
            {
                if (em == RC4Encrypt.EncoderMode.Base64Encoder)
                {
                    result = System.Convert.ToBase64String(RC4Encrypt.EncryptEx(RC4Encrypt.Encode.GetBytes(data), pass));
                }
                else
                {
                    result = RC4Encrypt.ByteToHex(RC4Encrypt.EncryptEx(RC4Encrypt.Encode.GetBytes(data), pass));
                }
            }
            return result;
        }
        public static string Decrypt(string data, string pass, RC4Encrypt.EncoderMode em)
        {
            string result;
            if (data == null || pass == null)
            {
                result = null;
            }
            else
            {
                if (em == RC4Encrypt.EncoderMode.Base64Encoder)
                {
                    result = RC4Encrypt.Encode.GetString(RC4Encrypt.DecryptEx(System.Convert.FromBase64String(data), pass));
                }
                else
                {
                    result = RC4Encrypt.Encode.GetString(RC4Encrypt.DecryptEx(RC4Encrypt.HexToByte(data), pass));
                }
            }
            return result;
        }
        public static string Encrypt(string data, string pass)
        {
            return RC4Encrypt.Encrypt(data, pass, RC4Encrypt.EncoderMode.Base64Encoder);
        }
        public static string Decrypt(string data, string pass)
        {
            return RC4Encrypt.Decrypt(data, pass, RC4Encrypt.EncoderMode.Base64Encoder);
        }
        private static byte[] EncryptEx(byte[] data, string pass)
        {
            byte[] result;
            if (data == null || pass == null)
            {
                result = null;
            }
            else
            {
                byte[] array = new byte[data.Length];
                long num = 0L;
                long num2 = 0L;
                byte[] key = RC4Encrypt.GetKey(RC4Encrypt.Encode.GetBytes(pass), 256);
                for (long num3 = 0L; num3 < (long)data.Length; num3 += 1L)
                {
                    num = (num + 1L) % (long)key.Length;
                    num2 = (num2 + (long)((ulong)key[(int)checked((System.IntPtr)num)])) % (long)key.Length;
                    checked
                    {
                        byte b = key[(int)((System.IntPtr)num)];
                        key[(int)((System.IntPtr)num)] = key[(int)((System.IntPtr)num2)];
                        key[(int)((System.IntPtr)num2)] = b;
                        byte b2 = data[(int)((System.IntPtr)num3)];
                        byte b3 = key[(int)unchecked(key[(int)checked((System.IntPtr)num)] + key[(int)checked((System.IntPtr)num2)]) % key.Length];
                        array[(int)((System.IntPtr)num3)] = (byte)checked(b2 ^ b3);
                    }
                }
                result = array;
            }
            return result;
        }
        private static byte[] DecryptEx(byte[] data, string pass)
        {
            return RC4Encrypt.EncryptEx(data, pass);
        }
        public static byte[] HexToByte(string szHex)
        {
            int length = szHex.Length;
            byte[] result;
            if (length <= 0 || 0 != length % 2)
            {
                result = null;
            }
            else
            {
                int num = length / 2;
                byte[] array = new byte[num];
                for (int i = 0; i < num; i++)
                {
                    uint num2 = (uint)(szHex[i * 2] - ((szHex[i * 2] >= 'A') ? '7' : '0'));
                    if (num2 >= 16u)
                    {
                        result = null;
                        return result;
                    }
                    uint num3 = (uint)(szHex[i * 2 + 1] - ((szHex[i * 2 + 1] >= 'A') ? '7' : '0'));
                    if (num3 >= 16u)
                    {
                        result = null;
                        return result;
                    }
                    array[i] = (byte)(num2 * 16u + num3);
                }
                result = array;
            }
            return result;
        }
        public static string ByteToHex(byte[] vByte)
        {
            string result;
            if (vByte == null || vByte.Length < 1)
            {
                result = null;
            }
            else
            {
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(vByte.Length * 2);
                for (int i = 0; i < vByte.Length; i++)
                {
                    if (vByte[i] < 0)
                    {
                        result = null;
                        return result;
                    }
                    uint num = (uint)(vByte[i] / 16);
                    stringBuilder.Append((char)((ulong)num + (ulong)((num > 9u) ? 55L : 48L)));
                    num = (uint)(vByte[i] % 16);
                    stringBuilder.Append((char)((ulong)num + (ulong)((num > 9u) ? 55L : 48L)));
                }
                result = stringBuilder.ToString();
            }
            return result;
        }
        private static byte[] GetKey(byte[] pass, int kLen)
        {
            byte[] array = new byte[kLen];
            for (long num = 0L; num < (long)kLen; num += 1L)
            {
                array[(int)checked((System.IntPtr)num)] = (byte)num;
            }
            long num2 = 0L;
            for (long num = 0L; num < (long)kLen; num += 1L)
            {
                num2 = (num2 + (long)((ulong)array[(int)checked((System.IntPtr)num)]) + (long)((ulong)pass[(int)checked((System.IntPtr)(num % unchecked((long)pass.Length)))])) % (long)kLen;
                checked
                {
                    byte b = array[(int)((System.IntPtr)num)];
                    array[(int)((System.IntPtr)num)] = array[(int)((System.IntPtr)num2)];
                    array[(int)((System.IntPtr)num2)] = b;
                }
            }
            return array;
        }
    }
}
