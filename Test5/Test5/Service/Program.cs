using System;
using System.Threading;
using System.Threading.Tasks;
using Test5.M;

namespace Test5
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*
             *  x Hente alle sucurit object (oversikt for deg + øvelse) denne funksjonen kan sikkert fjernes..
             *  - Lage nye security object
             *  - Encrypte data + ha med device ID
             *  - Decrypte data + ha med device ID
             *  - Key rotations
             *
             */



            string filepath = "C:/Users/Visjon/Documents/GitHub/Testing/Test5/Test5/Model/LoginData.json";
            var api = new API(filepath);

            Console.WriteLine("Ziot Solutions - Fortanix & Console application test \n");
            ConsoleKeyInfo cki = default;

            do
            {
                Console.WriteLine("You have the following options:");
                if (!api._loggedIn)
                {
                    Console.WriteLine("Press L = Login");
                    Console.WriteLine("\nPlease press a key");
                    cki = Console.ReadKey();
                    Console.Clear();
                }

                if (api._loggedIn)
                {
                    Console.WriteLine("Press G = Get a list of all security objects");
                    Console.WriteLine("Press N = Create new key");
                    Console.WriteLine("Press E = Encrypt a message");
                    Console.WriteLine("Press D = Decrypt a message");
                    Console.WriteLine("Press ESC = End the app");
                    Console.WriteLine("\nPlease press a key");
                    cki = Console.ReadKey();
                    Console.Clear();

                    if (cki.Key == ConsoleKey.G)
                    {
                        await api.GetAllSecurityObjects();
                    }

                    //if (cki.Key == ConsoleKey.N)
                    //{

                    //}

                    if (cki.Key == ConsoleKey.E)
                    {
                        //How to get device id?
                        string deviceID = "12345";
                        Console.WriteLine("Please write your plain text:");
                        string data = Console.ReadLine();

                        //SOD = SecurityObjectData
                        SecurityObjectData SOD = new SecurityObjectData()
                        {
                            Data = data,
                            DeviceID = deviceID
                        };

                        var encryptedSOD = api.Encrypt(SOD);
                        //Console.WriteLine("Your cipher text is" + encryptedSOD.Data + "\n");

                    }

                    //if (cki.Key == ConsoleKey.D)
                    //{
                    //    string deviceID = "12345";
                    //    Console.WriteLine("Please write your cipher text");
                    //    string cipher = Console.ReadLine();

                    //    //SOD = SecurityObjectData
                    //    SecurityObjectData SOD = new SecurityObjectData()
                    //    {
                    //        Data = cipher,
                    //        DeviceID = deviceID
                    //    };

                    //    var decryptedSOD = api.Decrypt(SOD);
                    //    Console.WriteLine("Your decrypted text is" + decryptedSOD.Data);
                    //}
                }


                if (cki.Key == ConsoleKey.L)
                {
                    if (api._loggedIn)
                    {
                        Console.WriteLine("You are already logged in");
                    }
                    else
                    {
                        Console.WriteLine("Logging in..\n");
                        Thread.Sleep(300);
                        await api.connectToAPI();
                        Console.Clear();
                        Console.WriteLine("Login successful!\n");
                    }
                }



            } while (cki.Key != ConsoleKey.Escape);
        }
    }
}
