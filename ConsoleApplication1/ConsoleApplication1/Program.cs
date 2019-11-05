using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            DataClasses1DataContext dataContext = new DataClasses1DataContext();


            var varList = (from r in dataContext.Results
                           select new Mytest(r.ResultID,r.ResultType)).Take(10);

            if (varList != null && varList.Count() > 0)
            {
                foreach (var i in varList)
                {
                    Console.WriteLine(i.ResultID+ i.ResultType);

                }
            }
            else
                Console.WriteLine("No data");

            Console.ReadLine();
        }


        public class Mytest
        {
            public int ResultID { get; set; }
            public string ResultType { get; set; }

            public Mytest(int id,string resultType)
            {
                this.ResultID = id;
                this.ResultType = resultType;

            }
        }
    }
}
