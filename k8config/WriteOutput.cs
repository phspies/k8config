using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config
{
    public class WriteOutput
    {
        static public void WriteNormalLine(string _line)
        {
            Console.WriteLine(_line);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        static public void WriteInformationLine(string _line)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(_line);
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        static public void WriteErrorLine(string _line)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(_line);
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }
    }
}
