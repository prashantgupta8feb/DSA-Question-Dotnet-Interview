using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Question
{

    // Abstract class BaseClass
    public abstract class BaseClass
    {

        // Abstract method 'Display()'
        public abstract void Display();

    }

    // Class Child1 inherits from BaseClass
    public class Child1 : BaseClass
    {
        // Implement abstract method Display() with override
        public override void Display()
        {
            Console.WriteLine("class Child1");
        }
    }

    // Class Child2 inherits from BaseClass
    public class Child2 : BaseClass
    {
        // Implement abstract method 'Display()' with override
        public override void Display()
        {
            Console.WriteLine("class Child2");
        }
    }
}
