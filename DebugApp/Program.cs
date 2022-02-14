using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugApp
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Minimum size: ");
                string[] minimum = Console.ReadLine().Split(' ');
                Console.WriteLine("Maximum size: ");
                string[] maximum = Console.ReadLine().Split(' ');
                decimal.TryParse(minimum[0], out decimal minSize);
                decimal.TryParse(maximum[0], out decimal maxSize);
                string[] units = new string[]{minimum[1], maximum[1]};
                decimal[] sizes = new decimal[]{minSize, maxSize};

                Console.WriteLine("input torrent size? ");
                string[] source = Console.ReadLine().Split(' ');
                decimal.TryParse(source[0], out decimal torSize);
                string torUnit = source[1];

                switch (units[0]) //minimum size check
                {
                    case "MB":
                        if (torSize < sizes[0] && torUnit == units[0])
                            Console.WriteLine("min size. Fail 1.");
                        break;
                    case "GB":
                        if (torUnit == "MB")
                            Console.WriteLine("min size. Fail 2.");
                        if (torSize < sizes[0])
                            Console.WriteLine("min size. Fail 3.");
                        break;
                }

                switch (units[1]) //maximum size check
                {
                    case "MB":
                        if (torUnit == "GB")
                            Console.WriteLine("max size. Fail 1.");
                        if (torSize > sizes[1])
                            Console.WriteLine("max size. Fail 2.");
                        break;
                    case "GB":
                        if (torSize > sizes[1] && torUnit == units[1])
                            Console.WriteLine("max size. Fail 3.");
                        break;
                }
            } while (true);
        }
    }
}
