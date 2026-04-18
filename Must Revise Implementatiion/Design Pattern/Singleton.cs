using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    //singleton design pattern implementation

    public class Singleton
    {
        private static Singleton Instance = null;
        private static readonly object padlock = new object();

        private Singleton()
        {
            Console.WriteLine("Inside private constructor");
        }

        public static Singleton GetInstance()
        {
            if (Instance == null)       //first check, avoids lock overhead
            {
                lock (padlock)          //ensures only one thread enters at a time
                {
                    if (Instance == null)       //second check, thread safety
                    {
                        Instance = new Singleton();
                    }
                }
            }
            Instance.Show();
            return Instance;
        }

        public void Show()
        {
            Console.WriteLine("Singleton object created");
        }
    }

}
