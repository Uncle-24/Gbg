using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testWCF.ServiceReference1;

namespace testWCF
{
    class Program
    {
        static void Main(string[] args)
        {
            //需要先启动wcf服务项目：WcfServiceLibrary2
            using (var proxy = new Service1Client())
            {
                Console.WriteLine(proxy.HelloWorld("world"));
                CompositeType t = new CompositeType();
                t.StringValue = "world";
                Console.WriteLine(proxy.GetDataUsingDataContract(t).StringValue);
                Console.ReadKey();
            }

            Service1Client client = new Service1Client();
            Console.WriteLine(client.HelloWorld("123"));
            
            Console.ReadKey();
        }
    }
}
