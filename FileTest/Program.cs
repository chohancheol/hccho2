using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTest
{
    //https://zetcode.com/csharp

    internal class Program
    {
        static void Main(string[] args)
        {
            // ReadLine
            var path = "words.txt";

            var lines = File.ReadLines(path);

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }


            string[] lines2 = File.ReadAllLines(path);

            foreach (var line2 in lines2)
            {
                Console.WriteLine(line2);
            }

            string readText = File.ReadAllText(path);
            Console.WriteLine(readText);
        }
    }
}
