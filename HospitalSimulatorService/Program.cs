using System;
using Microsoft.Owin.Hosting;
using HospitalSimulatorService.Properties;

namespace HospitalSimulatorService
{
    public class Program
    {
        public static void Main()
        {
            // Start OWIN - Let the Console window stay open until user clicks a key
            using (WebApp.Start<Startup>(url: Resources.BaseUrl))
            {
                Console.WriteLine("Hospital Simulator Service Started at URL: " + Resources.BaseUrl);
                DataRepository.ReadSeedJson();
                Console.WriteLine("\n\nPress any key to stop the service");
                Console.ReadKey();
            }
        }
    }
}
