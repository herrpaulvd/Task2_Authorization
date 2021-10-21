using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Task2_Authorization
{
    static class Encoder
    {
        private static Random random = new();
        private static int random2 = random.Next();

        public static void FeedByteArray(byte[] bytes)
        {
            for(int i = 0; i < bytes.Length; i++)
            {
                int offset = random.Next(32 - 8);
                random2 ^= (bytes[i] << offset);
            }
        }

        public static void FeedInt(int i)
        {
            random2 += i * i;
        }

        public static byte[] EncodeObject<T>(T obj)
        {
            string json = JsonSerializer.Serialize(obj);
            byte[] rawBytes = Encoding.UTF8.GetBytes(json);
            FeedByteArray(rawBytes);
            int seed = random.Next() ^ random2;
            Random local = new (seed);
            for (int i = 0; i < rawBytes.Length; i++)
                rawBytes[i] = (byte) (rawBytes[i] ^ (local.Next(256) + i * i));
            return new byte[] { (byte)(seed >> 0), (byte)(seed >> 8), (byte)(seed >> 16), (byte)(seed >> 24) }.Concat(rawBytes).ToArray();
        }

        public static T DecodeObject<T>(byte[] bytes)
        {
            int n = bytes.Length - 4;
            int seed = bytes[0] + (bytes[1] << 8) + (bytes[2] << 16) + (bytes[3] << 24);
            Random local = new(seed);
            byte[] rawBytes = new byte[n];
            for (int i = 0; i < n; i++)
                rawBytes[i] = (byte)(bytes[i + 4] ^ (local.Next(256) + i * i));
            string json = Encoding.UTF8.GetString(rawBytes);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
