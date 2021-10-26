using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Test5
{
    class Program
    {
        // Device Id is coded into this class. Needs to be changed so that it can be entered manually
        // IV for encryption is coded into the fortanixService class. may needs to be changed later. 

        static async Task Main(string[] args)
        {
            //string path = "C:/Users/AgithRavn/Documents/GitHub/Testing/Test5_v2/Test5/Model/LoginData.json";
            string path = @"..\..\..\Model\LoginData.json";
            var api = new FortanixService(path);

            //????
            //string keyName = "Test13"; 

            ConsoleKey ck;
            Console.WriteLine("Ziot Solutions - Fortanix API test console app\n");

            do
            {
                Console.WriteLine("You have the following options:");
                Console.WriteLine("Press K = Create new key");
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
                    case ConsoleKey.K:
                        await CreateNewKeyHandler();
                        break;
                    //case E:
                    //    await EncryptionHandler();
                    //    break;
                    //case D:
                    //    await DecryptionHandler();
                    //    break;
                    //case R:
                    //    await RotateKeyHandler();
                    //    break;
                    //case X:
                    //    await ExportKeyHandler();
                    //    break;
                    //case W:
                    //    await RotateWrapKeyHandler();
                    //    break;
                    default:
                        KeyUnknown();
                        break;
                }


            } while (ck != ConsoleKey.Escape);



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

                //Hva er poenget med denne?
                //keyName = newKey.name;

            }

            //async Task EncryptionHandler()
            //{
            //    Console.WriteLine("Please write your plain text:");
            //    string data = Console.ReadLine();

            //    Payload payload = new Payload(keyName, data);

            //    var encryptedChicken = await api.Encrypt(payload);
            //    Console.WriteLine($"Your cipher text is: {encryptedChicken.Data} \n");

            //}

            //async Task DecryptionHandler()
            //{
            //    Console.WriteLine("Please write your cipher text");
            //    string cipher = Console.ReadLine();

            //    Payload payload = new Payload(keyName, cipher);

            //    var decryptedChicken = await api.Decrypt(payload);
            //    Console.WriteLine($"Your decrypted text is: {decryptedChicken.Data}");

            //}

            //async Task RotateKeyHandler()
            //{
            //    Console.WriteLine("Please enter the name of the key that should rotate:");
            //    var keyname = Console.ReadLine();
            //    var newkey = await api.RotateKey(keyname);
            //    Console.WriteLine($"\nKeyname: {newkey.name}\nNew Key-Id: {newkey.kid}\n");

            //}

            //async Task ExportKeyHandler()
            //{
            //    Console.WriteLine("Please enter the name of the key that you want to export:");
            //    var keyname = Console.ReadLine();
            //    var key = await api.ExportKey(keyname);
            //    Console.WriteLine($"\nKeyname: {key.name}\nKey-Id: {key.kid}\nKey-Value: {key.value}\n");

            //}

            //async Task RotateWrapKeyHandler()
            //{
            //    Console.WriteLine("Please write down name for the key that you want to rotate:");
            //    var oldKeyName = Console.ReadLine();
            //    Console.Clear();
            //    var wrappedNewKey = await api.RotateKeyWrapped(oldKeyName);
            //    Console.WriteLine("The new Key value:\n");
            //    Console.WriteLine(wrappedNewKey.wrapped_key);

            //}
        }
    }
}
