using System;
using NewLife.IoT.Drivers;
using NewLife.Log;

namespace Test
{
    class Program
    {
        static void Main(String[] args)
        {
            XTrace.UseConsole();

            try
            {
                Test1();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }

            Console.WriteLine("OK!");
            Console.ReadKey();
        }

        static void Test1()
        {
            DriverFactory.ScanAll();

            DriverFactory.ScanAll();

            foreach (var item in DriverFactory.Drivers)
            {
                XTrace.WriteLine(item.DefaultParameter?.Trim());
            }
        }
    }
}