using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuppyDbConsole
{
    public class OutMessage
    {
        public static void Warning(string message)
        {
            Console.WriteLine($@"
!----------------!
| WARNING  
!----------------!
{message}");
        }

        public static void Error(string message)
        {
            Console.WriteLine($@"
!----------------!
| ERROR  
!----------------!
{message}");
        }
        public static void Success(string message)
        {
            Console.WriteLine($@"
| SUCCESS : {message}");

        }
    }
}
