using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Testfortanixapi
{
    class Program
    {
        
        static async Task Main()
        {

            //API apicall = new API(filepath: "C:/Users/mgran/Desktop/KMS/apps/Testfortanixapi/logindata.json");
            //API apicall = new API(filepath: "C:/Users/mgran/Source/Repos/ziot-fortanix-spike/logindata.json");
            API apicall = new API(filepath: "C:/Users/Visjon/Documents/GitHub/ziot-fortanix-spike/logindata.json");

            Console.WriteLine("Fortanix Test Console Application");
            await apicall.GetAPIStatus();
            await apicall.StartSession();
            ConsoleKeyInfo cki;

            do
            {
                Console.WriteLine("You have the following options:");
                Console.WriteLine("Press K = Get all the existing keys");
                Console.WriteLine("Press S = Check the status of the current session");
                Console.WriteLine("Press G = Choose a key");
                Console.WriteLine("Press E = Encrypt a message");
                Console.WriteLine("Press D = Decrypt a message");
                Console.WriteLine("Press ESC = End the app");
                Console.WriteLine("\nPlease press a key");
                cki = Console.ReadKey();
                Console.Clear();

                if (cki.Key == ConsoleKey.K)
                {
                    await apicall.GetAllKeys();
                }

                if (cki.Key == ConsoleKey.S)
                {
                    apicall.IsSessionActive();
                }

                if (cki.Key == ConsoleKey.G)
                {
                    Console.WriteLine("Please enter the keyname or Key ID.");
                    string keyname = Console.ReadLine();
                    Console.WriteLine("");
                    await apicall.GetAllKeys();
                    await apicall.GetKeyByName(keyname);
                    if (!apicall.isactiveKey)
                    {
                        await apicall.GetKeyByID(keyname);
                    };
                    if (apicall.isactiveKey)
                    {
                        Key activeKey = apicall.getactiveKey;
                        Console.WriteLine($"The Key with the name {activeKey.name} and the KID: {activeKey.kid} has been chosen.\n");
                    }
                    else
                    {
                        Console.WriteLine("The key could not be found.\n");
                    }
                }


                if (cki.Key == ConsoleKey.E && !apicall.isactiveKey)
                {
                    Console.WriteLine("Please choose a key first by entering G\n");
                }

                if (cki.Key == ConsoleKey.E && apicall.isactiveKey)
                {
                    Console.WriteLine("Please write your plain text:");
                    string plaintext = Console.ReadLine();
                    Console.WriteLine(""); //For extra space
                    string cipher = apicall.Encrypt(plaintext).Result;
                    Console.WriteLine("Your "+cipher + "\n");

                }

                if (cki.Key == ConsoleKey.D && apicall.isactiveKey)
                {
                    Console.WriteLine("Please enter cipher text:");
                    string cipher = Console.ReadLine();
                    Console.WriteLine(""); //For extra space
                    Console.WriteLine("Please enter the IV.");
                    string iv = Console.ReadLine();
                    Console.WriteLine(""); //For extra space
                    string plain = apicall.Decrypt(cipher,iv).Result;
                    Console.WriteLine("Your Plaintext is: " + plain + "\n");

                }


            } while (cki.Key != ConsoleKey.Escape);
        }

        
        
    }
}
