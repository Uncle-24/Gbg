using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace StartSulphur
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //获得服务集合
                var serviceControllers = ServiceController.GetServices();
                //遍历服务集合，打印服务名和服务状态
                foreach (var service in serviceControllers)
                {
                    Console.WriteLine("ServiceName:{0}\t\tServiceStatus:{1}", service.ServiceName, service.Status);
                }


                ServiceController sc = serviceControllers.FirstOrDefault(service => service.ServiceName == "Themes");
                
                while (true)
                {

                    if (sc.Status == ServiceControllerStatus.Stopped)
                        sc.Start();

                }
            }
            catch (Exception e)
            {
                throw e;

            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
