using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Test2
{
    class Program
    {


        static async Task Main(string[] args)
        {
            string loginDataPath = "C:/Users/Visjon/Documents/GitHub/Testing/Test2/Test2/Models/LoginData.json";
            var api = new ConsumeAPI(loginDataPath);
            ConsoleKeyInfo cki;

            while (true)
            {
                api.CheckAPIStatus();
                api.ConnectToAPI();

                cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Q)
                {
                    break;
                }
            }

        }
    }
}
