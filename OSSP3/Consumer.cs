using System;
using System.Collections.Concurrent;

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
                // doSmth(filename);
                // Console.WriteLine(filename);
            }
        }
    }
}

