using System;
using Test5.M;

namespace Test5
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new API();

            Console.WriteLine("Ziot Solutions COOP \n");
            ConsoleKeyInfo cki;

            do
            {
                Console.WriteLine("You have the following options:");
                Console.WriteLine("Press N = Create new key");
                Console.WriteLine("Press E = Encrypt a message");
                Console.WriteLine("Press D = Decrypt a message");
                Console.WriteLine("Press ESC = End the app");
                Console.WriteLine("\nPlease press a key");
                cki = Console.ReadKey();
                Console.Clear();


                if (cki.Key == ConsoleKey.E)
                {
                    //How to get device id?
                    string deviceID = "12345";
                    Console.WriteLine("Please write your plain text:");
                    string data = Console.ReadLine();

                    Chicken chicken = new Chicken()
                    {
                        Data = data,
                        DeviceID = deviceID
                    };

                    var encryptedChicken = api.Encrypt(chicken);
                    Console.WriteLine("Your cipher text is" + encyptedChicken.Data + "\n");

                }

                if (cki.Key == ConsoleKey.D)
                {
                    string deviceID = "12345";
                    Console.WriteLine("Please write your cipher text");
                    string cipher = Console.ReadLine();

                    Chicken chicken = new Chicken()
                    {
                        Data = cipher,
                        DeviceID = deviceID
                    };

                    var decryptedChicken = api.Decrypt(chicken);
                    Console.WriteLine("Your decrypted text is" + decryptedChicken.Data);
                }

            } while (cki.Key != ConsoleKey.Escape);
        }
    }
}
