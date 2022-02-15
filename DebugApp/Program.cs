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

                if (sizes[0] > 0) //only execute min size check if it's not a 0 value
                {
                    switch (units[0]) //minimum size check
                    {
                        case "MB":
                            if (torUnit == "KB")
                                Console.WriteLine("KB, min size is MB");
                            if (torSize < sizes[0] && torUnit == units[0])
                                Console.WriteLine("Min size MB, smaller than min size");
                            break;
                        case "GB":
                            if (torUnit == "MB")
                                Console.WriteLine("Min size is GB and torrent is MB");
                            if (torSize < sizes[0])
                                Console.WriteLine("Min size is GB, smaller than Min Size");
                            break;
                    }
                }

                switch (units[1]) //maximum size check
                {
                    case "MB":
                        if (torUnit == "GB")
                            Console.WriteLine("Max size is MB and torrent is GB");
                        if (torSize > sizes[1] && torUnit != "KB")
                            Console.WriteLine("Max size is MB, torrent is larger than max size and is not KB");
                        break;
                    case "GB":
                        if (torSize > sizes[1] && torUnit == units[1])
                            Console.WriteLine("Max size is GB. Torrent is larger than max size.");
                        break;
                }
            } while (true);
        }
    }
}
