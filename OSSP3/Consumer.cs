using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.IO;

namespace OSSP3
{
    public class Consumer
    {
        BlockingCollection<string> fileQueue;

        public Consumer(BlockingCollection<string> queue)
        {
            fileQueue = queue;
        }

        public void Run()
        {
            string filename;
            while(fileQueue.TryTake(out filename))
            {
                int mzs = CountMZs(filename);
                if (mzs > 0)
                {
                    string md5 = BitConverter.ToString(GetMD5(filename)).Replace("-","").ToLower();
                    Console.WriteLine("{0}\n  MD5: {1} MZs: {2} ", filename, md5, mzs);
                }
            }
        }

        byte[] GetMD5(string file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        int CountMZs(string file)
        {
            int count = 0;

            const int chunkSize = 65536;
            byte[] chunk = new byte[chunkSize];
            int offset = 0;
            using (var stream = File.OpenRead(file))
            {
                while (stream.Read(chunk, offset, chunkSize) > 0)
                {
                    string s = System.Text.Encoding.ASCII.GetString(chunk);
                    if (s.Contains("MZ"))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}

