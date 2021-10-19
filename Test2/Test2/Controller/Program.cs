using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Test2
{
    class Program
    {


        static async Task Main(string[] args)
        {
            string loginDataPath = "C:/Users/Visjon/Documents/GitHub/Testing/Test2/Test2/Models/LoginData.json";
            var api = new ConsumeAPI(loginDataPath);
            await api.CheckAPIStatus();
            await api.ConnectToAPI();
            ConsoleKeyInfo cki;

            while (true)
            {
                Console.WriteLine("You have the following options:");
                Console.WriteLine("Press K = Get all the existing keys");
                Console.WriteLine("Press S = Check the status of the current session");
                Console.WriteLine("Press G = Choose a key");
                Console.WriteLine("Press E = Encrypt a message");
                //Console.WriteLine("Press D = Decrypt a message");
                Console.WriteLine("Press Q = End the app");
                Console.WriteLine("\nPlease press a key");
                cki = Console.ReadKey(); //Slik at while loop stopper her
                Console.Clear();

                if (cki.Key == ConsoleKey.K)
                {
                    await api.GetAllKeys();
                }

                if (cki.Key == ConsoleKey.S)
                {
                    api.CheckSession();
                }

                if (cki.Key == ConsoleKey.G)
                {
                    Console.WriteLine("Please enter the keyname or Key ID.");
                    string keyname = Console.ReadLine();
                    Console.WriteLine(""); //extra space?
                    await api.GetAllKeys();
                    //await api.GetKeyByName(keyname);

                }

                if (cki.Key == ConsoleKey.E)
                {

                }

                if (cki.Key == ConsoleKey.Q)
                {
                    break;
                }

            }

        }
    }
}
