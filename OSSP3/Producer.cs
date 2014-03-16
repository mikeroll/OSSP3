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

        void TraverseFS(string root, Action<string> action)
        {
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
                catch (UnauthorizedAccessException) {}
                catch (DirectoryNotFoundException) {}

                foreach (var dir in subDirs)
                {
                    dirs.Push(dir);
                }

                try 
                {
                    files = Directory.GetFiles(currentDir);
                }
                catch (UnauthorizedAccessException) {}
                catch (DirectoryNotFoundException) {}
                catch (IOException) {}

                foreach (var file in files)
                {
                    action(file);
                }
            }
        }
    }
}

