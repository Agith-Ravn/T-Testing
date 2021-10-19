using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Testfortanixapi
{
    internal class API
    {
        private static HttpClient _client;
        private static SessionData _sessionData;
        private readonly string _filepath;
        private List<Key> _keylist;
        private readonly LoginData _logindata;

        public API(string filepath)
        {
            _filepath = filepath;
            _logindata = GetLoginDatafromFile(filepath);
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_logindata.baseURI);
            _sessionData = new SessionData();
        }

        public bool isactiveKey
        {
            get
            {
                if (getactiveKey != null)
                    return true;
                return false;
            }
        }

        public Key getactiveKey { get; private set; }

        private LoginData GetLoginDatafromFile(string path)
        {
            var jsonString = File.ReadAllText(path);
            //Console.WriteLine(jsonString);
            var loginData = JsonSerializer.Deserialize<LoginData>(jsonString);
            //Console.WriteLine(loginData.baseURI);

            return loginData;
        }

        public async Task GetAPIStatus()
        {
            string returnstring;
            try
            {
                var response = await _client.GetAsync("https://eu.smartkey.io/sys/v1/health");
                returnstring = "The server is handling requests.\n";
            }
            catch (HttpRequestException e)
            {
                returnstring = "\n The api has Problems!" + ("Message : {0} ", e.Message + "\n");
            }

            Console.WriteLine(returnstring);
        }

        public async Task StartSession()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _logindata.apiKey);
            try
            {
                var response = await _client.PostAsync("/sys/v1/session/auth", null);
                var responseBody = await response.Content.ReadAsStringAsync();
                _sessionData = JsonSerializer.Deserialize<SessionData>(responseBody);
                //Console.WriteLine(responseBody);
                //Console.WriteLine(_sessionData.expires_in);
                //Console.WriteLine(_sessionData.sessionstart);
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _sessionData.access_token);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public bool IsSessionActive()
        {
            var res = _sessionData.sessionend.CompareTo(DateTime.Now);
            if (res >= 0)
            {
                Console.WriteLine("Session active until:" + _sessionData.sessionend + "\n");
                return true;
            }

            Console.WriteLine("Session expired: " + _sessionData.sessionend + "\n");
            return false;
        }

        public async Task RefreshSession()
        {
            try
            {
                await _client.PostAsync("/sys/v1/session/refresh", null);
                _sessionData.sessionstart = DateTime.Now;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public async Task TerminateSession()
        {
            try
            {
                await _client.PostAsync("/sys/v1/session/terminate", null);
                _logindata.bearertoken = "";
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public async Task GetAllKeys()
        {
            var response = await _client.GetAsync("/crypto/v1/keys");
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                _keylist = JsonSerializer.Deserialize<List<Key>>(responseString);
                Console.WriteLine("Number of keys found: " + _keylist.Count);

                foreach (var key in _keylist)
                {
                    Console.WriteLine("Test: " + key.name);
                }

                Console.WriteLine("--------------------------\n");
            }
            else
            {
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            }
        }

        public async Task GetKeyByName(string keyname)
        {
            Key returnkey = null;
            foreach (var key in _keylist)
                if (key.name == keyname)
                {
                    await GetKeyByID(key.kid);
                    return;
                }

            getactiveKey = returnkey;
        }

        public async Task GetKeyByID(string kid)
        {
            Key returnkey = null;
            if (kid.Length == 32 || kid.Length == 36)
            {
                var response = await _client.GetAsync("/crypto/v1/keys/" + kid);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    returnkey = JsonSerializer.Deserialize<Key>(responseBody);
                    Console.WriteLine(string.Format($"The key with the name {returnkey.name} has been found"));
                }
                else
                {
                    var errormessage = await response.Content.ReadAsStringAsync();
                    var errorcode = response.StatusCode;
                    Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
                }
            }

            getactiveKey = returnkey;
        }

        public async Task<string> Encrypt(string plaintext)
        {
            var plaintext64 = Base64Encode(plaintext);

            var keyDescriptor = new SobjectDescriptor
            {
                kid = getactiveKey.kid
            };

            var content = new EncryptionCall
            {
                key = keyDescriptor,
                alg = "AES",
                plain = plaintext64,
                mode = "CBC"
            };
            EncryptionResponse encResponse = null;
            var jsonContent = JsonSerializer.Serialize(content);
            HttpContent contentObject = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/crypto/v1/encrypt", contentObject);
            //HttpResponseMessage response = await _client.PostAsJsonAsync("/crypto/v1/encrypt", content);
            if (response.IsSuccessStatusCode)
            {
                encResponse = await response.Content.ReadFromJsonAsync<EncryptionResponse>();
                return $"Cipher: {encResponse.cipher}\nIV: {encResponse.iv}";
            }

            var errormessage = await response.Content.ReadAsStringAsync();
            var errorcode = response.StatusCode;
            return $"Error {(int)errorcode}: \nMessage: {errormessage}";
        }

        public async Task<string> Decrypt(string cipher, string iv)
        {
            var cipher64 = Base64Encode(cipher);
            var keyDescriptor = new SobjectDescriptor
            {
                kid = getactiveKey.kid
            };
            var content = new DecryptionCall
            {
                key = keyDescriptor,
                alg = "AES",
                cipher = cipher,
                mode = "CBC",
                iv= iv,
            };
            DecryptionResponse decResponse = null;
            var jsonContent = JsonSerializer.Serialize(content);
            HttpContent contentObject = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/crypto/v1/decrypt", contentObject);
            if (response.IsSuccessStatusCode)
            {
                decResponse = await response.Content.ReadFromJsonAsync<DecryptionResponse>(); 
                return Base64Decode(decResponse.plain);


            }
            else
            {
                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                return $"Error {(int)errorcode}: \nMessage: {errormessage}";
            }

            
        }

        private string Base64Encode(string plain)
        {
            var plaintextbytes = Encoding.UTF8.GetBytes(plain);
            return Convert.ToBase64String(plaintextbytes);
        }

        private string Base64Decode(string code)
        {
            var base64EncodedBytes = Convert.FromBase64String(code);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public class EncryptionCall
    {
        public SobjectDescriptor key { get; set; }
        public string alg { get; set; }
        public string plain { get; set; }
        public string mode { get; set; }
    }

    public class DecryptionCall
    {
        public SobjectDescriptor key { get; set; }
        public string alg { get; set; }
        public string cipher { get; set; }
        public string mode { get; set; }
        public string iv { get; set; }
    }

    public class EncryptionResponse
    {
        public string kid { get; set; }
        public string cipher { get; set; }
        public string iv { get; set; }
        public string tag { get; set; }
    }

    public class DecryptionResponse
    {
        public string kid { get; set; }
        public string plain { get; set; }
    }

    public class SobjectDescriptor
    {
        public string kid { get; set; }
    }
}