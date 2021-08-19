using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace k8config
{
    public class WriteOutput
    {

        static public void WriteInformationHighlightLine(string _line, ConsoleColor color)
        {
            var pieces = Regex.Split(_line, @"(\[[^\]]*\])");
            Console.ForegroundColor = ConsoleColor.Blue;

            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];

                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                Console.Write(piece);
                Console.ResetColor();
            }

            Console.WriteLine();
        }
        static public void WriteNormalLine(string _line)
        {
            Console.WriteLine(_line);
        }
        static public void WriteInformationLine(string _line)
        {
            Console.WriteLine(_line);
            Console.ResetColor();
        }
        static public void WriteErrorLine(string _line)
        {
            Console.WriteLine(_line);
            Console.ResetColor();
        }
    }
}
