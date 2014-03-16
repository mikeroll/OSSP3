using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OSSP3
{
    public class Producer
    {
        BlockingCollection<string> fileQueue;

        public Producer(BlockingCollection<string> queue)
        {
            fileQueue = queue;
        }

        public void Run(string root)
        {
            TraverseFS(root, GetNextFileName);
        }

        void GetNextFileName(string name)
        {
            fileQueue.Add(name);
        } 

        static void TraverseFS(string root, Action<string> action)
        {
            int processors = System.Environment.ProcessorCount;
            var dirs = new Stack<string>();

            if (!Directory.Exists(root))
            {
                throw new ArgumentException("The specified path does not exist.");
            }

            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs = {};
                string[] files = {};

                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }
                // Just continue silently - read what is readable
                catch (UnauthorizedAccessException) {}
                catch (DirectoryNotFoundException) {}

                try 
                {
                    files = Directory.GetFiles(currentDir);
                }
                catch (UnauthorizedAccessException) {}
                catch (DirectoryNotFoundException) {}
                catch (IOException) {}

                foreach (var dir in subDirs)
                {
                    dirs.Push(dir);
                }

                try
                {
                    // Few files - execute sequentially
                    if (files.Length < processors)
                    {
                        foreach (var file in files)
                        {
                            action(file);
                        }
                    }
                    // Many files - go parallel
                    else
                    {
                        Parallel.ForEach(files, (file) =>
                        {
                            action(file);
                        });
                    }
                }
                // Peace and quiet.
                catch (AggregateException) {}
            }
        }
    }
}

