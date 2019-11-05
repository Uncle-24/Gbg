using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Csharp
{
    class Program
    {
        public static void Main()
        {

            DateTime july28 = new DateTime(2009, 7, 28, 5, 23, 15, 16);

            string[] july28Formats = july28.GetDateTimeFormats();
            string [] s = DateTime.Now.GetDateTimeFormats();

            DateTime t = DateTime.MaxValue;
            t = DateTime.MinValue;
            t = DateTime.Now;
            t = DateTime.Today;
            t = DateTime.UtcNow;



            DateTime date1 = new DateTime(2010, 8, 18, 16, 32, 0, DateTimeKind.Local);
            Console.WriteLine("{0} {1}", date1, date1.Kind);
        }
    }
}
