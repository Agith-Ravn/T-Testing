using System;
using System.Threading;
using System.Threading.Tasks;
using VaultTest1.Model;

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
                    Console.WriteLine("Press E = Encrypt a message");
                    Console.WriteLine("Press D = Decrypt a message");
                    //Console.WriteLine("Press R = Rotate a Key");
                    //Console.WriteLine("Press X = Make a key exportable";
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

                    case ConsoleKey.C:
                        await CreateNewKey();
                        break;

                    case ConsoleKey.E:
                        await EncryptMessage();
                        break;

                    case ConsoleKey.D:
                        await DecryptMessage();
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

            //Making a key with a already existing name/id will return status code 204 as well.. need to find a solution for this
            //Cannot make a new key with name/id that existed before deletion
            async Task CreateNewKey()
            {
                Console.WriteLine("Please write down a name or a id for the new key");
                string keyName = Console.ReadLine().ToLower();
                int statusCode = await vault.CreateNewKey(keyName);
                if (statusCode == 204)
                {
                    Console.Clear();
                    Console.WriteLine("Key named " + keyName + " was created\n");
                }
                else
                {
                    Console.WriteLine("Error - No key was created\n");
                    Thread.Sleep(300);
                    Console.Clear();
                }
            }

            //Use key1 to encrypt
            async Task EncryptMessage()
            {
                Console.WriteLine("Please write down the key name/id (This key will be used to encrypt message)");
                string keyName = Console.ReadLine().ToLower();
                Console.WriteLine("Please write a message to encrypt)");
                string message = Console.ReadLine();

                Payload payload = new Payload(message,"aes256-gcm96");
                var encryptedPayload = await vault.EncryptMessage(keyName, payload);

                if (encryptedPayload.plaintext != null)
                {
                    Console.Clear();
                    Console.WriteLine("Your ciphertext is : " + encryptedPayload.plaintext);
                    Console.WriteLine("Make sure to save this a place!\n");
                    return;
                }

                Console.WriteLine("Error - message was not encrypted\n");
                Thread.Sleep(300);
                Console.Clear();
            }

            //Use key1 to decrypt
            async Task DecryptMessage()
            {
                Console.WriteLine("Please write down the key name/id (This key will be used to decrypt message)");
                string keyName = Console.ReadLine().ToLower();

                Console.WriteLine("Please write down ciphertext)");
                string ciphertext = Console.ReadLine();

                Data payload = new Data(ciphertext);

                var decryptedPayload = await vault.DecryptMessage(keyName, payload);

                if (decryptedPayload.plaintext != null)
                {
                    Console.Clear();
                    Console.WriteLine("Your plaintext is : " + decryptedPayload.plaintext + "\n");
                    return;
                }

                Console.WriteLine("Error - message was not decrypted\n");
                Thread.Sleep(300);
                Console.Clear();
            }
        }
    }
}
