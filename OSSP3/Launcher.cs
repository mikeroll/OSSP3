using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OSSP3
{
    public class Launcher
    {
        public delegate void AsyncProducerCaller(string root);
        public delegate void AsyncConsumerCaller();

        public static void Main(string[] args)
        {
            int consumersCount = 0;
            if (args.Length != 2 || !(Int32.TryParse(args[0], out consumersCount)))
            {
                var usage = @"Usage: OSSP3.exe <number of consumers> <root catalog>";
                Console.WriteLine(usage);
                return;
            }
            string root = args[1];

            var queue = new BlockingCollection<string>();

            var p = new Producer(queue);
            var prodCaller = new AsyncProducerCaller(p.Run);
            IAsyncResult prodResult = prodCaller.BeginInvoke(root, null, null);

            var consCallers = new List<AsyncConsumerCaller>();
            var asyncResults = new List<IAsyncResult>();
            for (int i = 0; i < consumersCount; i++)
            {
                var consumer = new Consumer(queue);
                var consCaller = new AsyncConsumerCaller(consumer.Run);
                consCallers.Add(consCaller);
                asyncResults.Add(consCaller.BeginInvoke(null, null));
            }

            for (int i = 0; i < consumersCount; i++)
            {
                consCallers[i].EndInvoke(asyncResults[i]);
            }

            prodCaller.EndInvoke(prodResult);
        }
    }
}

