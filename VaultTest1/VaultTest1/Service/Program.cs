using System;
using System.Threading;
using System.Threading.Tasks;

namespace VaultTest1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string path = @"..\..\..\Model\LoginData.json";
            var vault = new VaultService(path);

            ConsoleKey ck;
            Console.WriteLine("Ziot Solutions - Vault API test console app\n");

            do
            {
                Console.WriteLine("You have the following options:");

                if (!vault.login)
                {
                    Console.WriteLine("Press L = Log into vault");
                    Console.WriteLine("Press ESC = End the app");
                    Console.WriteLine("\nPlease press a key");
                }
                else
                {
                    Console.WriteLine("Press C = Create new key");
                    //Console.WriteLine("Press E = Encrypt a message");
                    //Console.WriteLine("Press D = Decrypt a message");
                    //Console.WriteLine("Press R = Rotate a Key");
                    //Console.WriteLine("Press X = Export a Key");
                    //Console.WriteLine("Press W = Rotate a Key and warpped the new Key in the old Key");
                    Console.WriteLine("Press ESC = End the app");
                    Console.WriteLine("\nPlease press a key");
                }

                ck = Console.ReadKey().Key;
                Console.Clear();

                switch (ck)
                {
                    case ConsoleKey.L:
                        await Login();
                        break;

                    case ConsoleKey.K:
                        await CreateNewKey();
                        break;

                    default:
                        break;
                }


            } while (ck != ConsoleKey.Escape);

            async Task Login()
            {
                if (vault._sessionData == null)
                {
                    var sessionData = await vault.Login();
                    if (sessionData.auth.lease_duration != null)
                    {
                        double seconds = sessionData.auth.lease_duration;
                        var timespan = TimeSpan.FromSeconds(seconds).ToString();

                        Console.WriteLine("Logged in");
                        Console.WriteLine("You will be logged out in " + timespan +"\n");
                    }
                    else
                    {
                        Console.WriteLine("Something went wrong..try again later");
                    }                 
                }
                else
                {
                    Console.WriteLine("Already logged in");
                }
            }

            async Task CreateNewKey()
            {
                Console.WriteLine("Please write down a name or a id for the new key");
                string keyName = Console.ReadLine();
                var key = await vault.CreateNewKey(keyName);
                if (key != null)
                {
                    Console.WriteLine("Key named " + keyName + "was created");
                }
                else
                {
                    Console.WriteLine("Error - No key was created");
                }
            }
        }
    }
}
