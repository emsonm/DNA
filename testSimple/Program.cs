using System;

namespace testSimple
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = 10;
            Console.WriteLine($"x is {x}");

            var y = PInvokeTest.Simple.Test(x);
            Console.WriteLine($"y is {y}");
        }
    }
}
