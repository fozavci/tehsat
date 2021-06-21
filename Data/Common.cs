using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;


namespace Tehsat
{
    public class Common
    {
        public static string[] channelTypes = { "HTTP", "HTTP Websocket", "TCP", "UDP" };
        public static byte[] key = new byte[16];
        public static byte[] iv = new byte[16];

        // Hard-coded keys for encryption tests
        private static string session_key = "SESSION_KEY";
        private static string session_iv = "SESSION_IV";

        // Random string generator for profile identifiers
        private static Random random = new Random();

        public static string RandomStringGenerator(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static string Encrypt(string text)
        {
            Array.Copy(Encoding.UTF8.GetBytes(session_key), key, 16);
            Array.Copy(Encoding.UTF8.GetBytes(session_iv), iv, 16);

            SymmetricAlgorithm algorithm = Aes.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.UTF8.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        public static string Decrypt(string text)
        {
            Array.Copy(Encoding.UTF8.GetBytes(session_key), key, 16);
            Array.Copy(Encoding.UTF8.GetBytes(session_iv), iv, 16);

            SymmetricAlgorithm algorithm = Aes.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.UTF8.GetString(outputBuffer);
        }
    }



}