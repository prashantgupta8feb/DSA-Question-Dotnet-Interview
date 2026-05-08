using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public class Multithreading
    {
        public static void CallMultithreading()
        {
            // 1. Start a new thread (via the ThreadPool)
            Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("Background Thread working...");
                    Thread.Sleep(500); // Wait half a second
                }
            });

            // 2. This runs on the Main thread at the same time
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Main Thread working...");
                Thread.Sleep(500);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
