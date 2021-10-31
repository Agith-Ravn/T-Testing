using System;
using System.Threading.Tasks;

namespace study1
{
    class Program
    {
        // Device Id is coded into this class. Needs to be changed so that it can be entered manually
        //IV for encryption is coded into the fortanixService class. may needs to be changed later. 

        static async Task Main(string[] args)
        {
            var api = new FortanixService();
            string keyName = "MorningTest2 (rotated at 2021-10-22 07:13:15)";
            ConsoleKey ck;

            Console.WriteLine("Ziot Solutions - Fortanix API test console app\n");

            do
            {
                Console.WriteLine("You have the following options:");
                Console.WriteLine("Press N = Create new key");
                Console.WriteLine("Press E = Encrypt a message");
                Console.WriteLine("Press D = Decrypt a message");
                Console.WriteLine("Press R = Rotate a Key");
                Console.WriteLine("Press X = Export a Key");
                Console.WriteLine("Press W = Rotate a Key and warpped the new Key in the old Key");
                Console.WriteLine("Press ESC = End the app");
                Console.WriteLine("\nPlease press a key");
                ck = Console.ReadKey().Key;
                Console.Clear();

                switch (ck)
                {
                    case ConsoleKey.N:
                        await CreateNewKeyHandler();
                        break;
                    case ConsoleKey.E:
                        await EncryptionHandler();
                        break;
                    case ConsoleKey.D:
                        await DecryptionHandler();
                        break;
                    case ConsoleKey.R:
                        await RotateKeyHandler();
                        break;
                    case ConsoleKey.X:
                        await ExportKeyHandler();
                        break;
                    case ConsoleKey.W:
                        await RotateWrapKeyHandler();
                        break;
                    default:
                        KeyUnknown();
                        break;
                }


            } while (ck != ConsoleKey.Escape);

            async Task RotateWrapKeyHandler()
            {
                Console.WriteLine("Please write down name for the key that you want to rotate:");
                var oldKeyName = Console.ReadLine();
                Console.Clear();
                var wrappedNewKey = await api.RotateKeyWrapped(oldKeyName);
                Console.WriteLine("The new Key value:\n");
                Console.WriteLine(wrappedNewKey.wrapped_key);

            }



            static void KeyUnknown()
            {
                Console.WriteLine("Please enter a valid key or end the App by pressing ESC.");
            }

            async Task CreateNewKeyHandler()
            {
                Console.WriteLine("Please write down name for the key");
                string newKeyName = Console.ReadLine();
                var newKey = await api.CreateNewKey(newKeyName);
                Console.Clear();
                if (newKey.name != null)
                {
                    Console.WriteLine("New key info: ");
                    Console.WriteLine("Key name: " + newKey.name);
                    Console.WriteLine("Key id: " + newKey.kid + "\n");
                }
                else
                {
                    Console.WriteLine("Error.. New key was't created.");
                }

                keyName = newKey.name;
            }

            async Task EncryptionHandler()
            {
                Console.WriteLine("Please write your plain text:");
                string data = Console.ReadLine();

                Payload payload = new Payload(keyName, data);

                var encryptedChicken = await api.Encrypt(payload);
                Console.WriteLine($"Your cipher text is: {encryptedChicken.Data} \n");
            }

            async Task DecryptionHandler()
            {
                Console.WriteLine("Please write your cipher text");
                string cipher = Console.ReadLine();

                Payload payload = new Payload(keyName, cipher);

                var decryptedChicken = await api.Decrypt(payload);
                Console.WriteLine($"Your decrypted text is: {decryptedChicken.Data}");
            }

            async Task RotateKeyHandler()
            {
                Console.WriteLine("Please enter the name of the key that should rotate:");
                var keyname = Console.ReadLine();
                var newkey = await api.RotateKey(keyname);
                Console.WriteLine($"\nKeyname: {newkey.name}\nNew Key-Id: {newkey.kid}\n");
            }

            async Task ExportKeyHandler()
            {
                Console.WriteLine("Please enter the name of the key that you want to export:");
                var keyname = Console.ReadLine();
                var key = await api.ExportKey(keyname);
                Console.WriteLine($"\nKeyname: {key.name}\nKey-Id: {key.kid}\nKey-Value: {key.value}\n");
            }
        }


    }
}
