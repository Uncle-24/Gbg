using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testUsing
{
    class Program
    {
        static void Main(string[] args)
        {
            MyCommand command = null;
            try { command = new MyCommand(); command.Fun1(); } catch { }

            try { command.Fun1(); } catch { }
            Console.Read();

        }
    }

    public class MyConnection : IDisposable
    {
        public void Open()
        {
            Console.WriteLine("MyConnection open");
        }
        public void Dispose()
        {
            Console.WriteLine("MyConnection dispose");
        }
    }


    public class MyCommand
    {
        public void Fun1()
        {
            using (MyConnection conn = new MyConnection())
            {
                conn.Open(); throw new Exception("");
            }
        }
    }

}
