using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            DataClasses1DataContext DataContext = new DataClasses1DataContext();
            var list = (from r in DataContext.UIContents
                        select new Result(r.ContentID)).Take(10);

        }
    }
    class Result {
        public int contentid { get; set; }
        public Result(int contentid) {
            this.contentid = contentid;
        }
    }
}
