using System;
using System.Threading.Tasks;

namespace Test3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            {

                API apicall = new API(filepath: "C:\\Users\\Visjon\\Documents\\GitHub\\Testing\\Test3\\Test3\\json1.json");
                await apicall.GetAPIStatus();
                await apicall.StartSession();
                ConsoleKeyInfo cki;

                do
                {
                    Console.WriteLine("You have the following options:");
                    Console.WriteLine("Press k to get all the existing keys");
                    Console.WriteLine("Press s to check the status of the current session");
                    Console.WriteLine("Press e to encrypt a message");
                    Console.WriteLine("Press d to decrypt a message");

                    Console.WriteLine("Press ESC to end the app");
                    Console.WriteLine("Please press a key");
                    cki = Console.ReadKey();
                    Console.WriteLine();
                    if (cki.Key == ConsoleKey.K)
                    {
                        await apicall.GetAllKeys();
                    }

                    if (cki.Key == ConsoleKey.S)
                    {
                        apicall.IsSessionActive();
                    }
                    if (cki.Key == ConsoleKey.E)
                    {

                    }


                } while (cki.Key != ConsoleKey.Escape);
            }



            //static byte[] EncryptStringToByte_Aes(string plainText, byte[] Key, byte[] IV)
            //{
            //    //Check Arguments.
            //    if (plainText == null || plainText.Length <= 0)
            //        throw new ArgumentNullException("plainText");
            //    if (Key == null || Key.Length <= 0)
            //        throw new ArgumentNullException("Key");
            //    if (IV == null || IV.Length <= 0)
            //        throw new ArgumentNullException("IV");
            //    byte[] encrypted;

            //    //Create an AesManaged object
            //    //with the specified Key and IV.
            //     using(AesManaged aesAlg = new AesManaged())
            //    {
            //        aesAlg.Key = Key;
            //        aesAlg.IV = IV;

            //        //Create an enryptor to perform the stream transform.
            //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            //        //Create the streams used for encryption.
            //        using (MemoryStream msEncrypt = new MemoryStream())
            //        {
            //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            //            {
            //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            //                {
            //                    //Write all data to the stream.
            //                    swEncrypt.Write(plainText);
            //                }
            //                encrypted = msEncrypt.ToArray();
            //            }
            //        }

            //    }
            //    //Return the encrypted bytes from the memory stream.
            //    return encrypted;

            //}
            //static string DecryptStringFormBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        }
    }
}
