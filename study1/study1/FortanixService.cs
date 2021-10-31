using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace study1
{
    class FortanixService
    {
        private static HttpClient _client;
        private readonly LoginData _loginData;
        private static SessionData _sessionData = null;
        private readonly string _rgbIv = "0000000000000000";

        /*
         * The fíle "logindata.json" needs to be present in the apps directory containing the API-Key and BaseUri
         */
        public FortanixService()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            _loginData = GetLoginDatafromFile();
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri(_loginData.BaseUri);



        }

        private LoginData GetLoginDatafromFile()
        {
            var path = Path.Join(Directory.GetCurrentDirectory(), "\\logindata.json");
            var jsonString = File.ReadAllText(path);
            var loginData = JsonSerializer.Deserialize<LoginData>(jsonString);
            return loginData;

        }

        public async Task<Payload> Encrypt(Payload payload)
        {
            //is session active
            await EnsureActiveSession();

            //Get the kid
            Key key = await GetKey(payload.KeyName);

            //pack data from payload into right format
            var plaintext64 = Base64Encode(payload.Data);
            var iv = Base64Encode(_rgbIv);


            var keyDescriptor = new SObjectDescriptor(key.kid);

            var content = new EncryptionCall(keyDescriptor, plaintext64, iv);

            // send post request

            var jsonContent = JsonSerializer.Serialize(content);
            HttpContent contentObject = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/crypto/v1/encrypt", contentObject);
            if (response.IsSuccessStatusCode)
            {
                EncryptionResponse encResponse = await response.Content.ReadFromJsonAsync<EncryptionResponse>();
                // pack response into payload adn return payload
                //string cipher = Base64Decode(encResponse.cipher);
                return new Payload(payload.KeyName, encResponse.cipher);

            }

            var errormessage = await response.Content.ReadAsStringAsync();
            var errorcode = response.StatusCode;
            Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            Console.ReadLine();
            return new Payload(payload.KeyName, "");

        }

        public async Task<Key> GetKey(string keyName)
        {

            {
                var response = await _client.GetAsync("/crypto/v1/keys?name=" + keyName);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    List<Key> keylist = JsonSerializer.Deserialize<List<Key>>(responseBody);
                    if (keylist != null && keylist.Count == 1)
                    {
                        return keylist[0];
                    }
                    else
                    {
                        Console.WriteLine("There is no Key for this keyName!");
                        Console.ReadLine();
                        return new Key();

                        // Needs a path to go back to the main menu.
                    }
                }

                var errormessage = await response.Content.ReadAsStringAsync();
                var errorcode = response.StatusCode;
                Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
                return new Key();


            }


        }

        private Task EnsureActiveSession()
        {
            if (_sessionData == null) return StartNewSession();

            int resultCompare = DateTime.Compare(_sessionData.sessionend, DateTime.Now);
            var diff = (_sessionData.sessionend - DateTime.Now).TotalSeconds;

            if (resultCompare > 0 && diff <= 10.0) return RenewSession();

            return StartNewSession();


        }

        private async Task RenewSession()
        {
            try
            {
                var response = await _client.PostAsync("/sys/v1/session/refresh", null);
                _sessionData.sessionstart = DateTime.Now;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                Console.ReadLine();
            }
        }

        public async Task StartNewSession()
        {

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _loginData.ApiKey);

            var response = await _client.PostAsync("/sys/v1/session/auth", null);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _sessionData = JsonSerializer.Deserialize<SessionData>(responseBody);
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _sessionData.access_token);
                return;
            }

            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", await response.Content.ReadAsStringAsync());
            Console.ReadLine();


        }

        public async Task<Payload> Decrypt(Payload payload)
        {
            await EnsureActiveSession();

            Key key = await GetKey(payload.KeyName);
            var iv = Base64Encode(_rgbIv);
            var keyDescriptor = new SObjectDescriptor(key.kid);
            var content = new DecryptionCall(keyDescriptor, payload.Data, iv);
            var jsonContent = JsonSerializer.Serialize(content);
            HttpContent contentObject = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/crypto/v1/decrypt", contentObject);
            if (response.IsSuccessStatusCode)
            {
                DecryptionResponse decResponse = await response.Content.ReadFromJsonAsync<DecryptionResponse>();
                string plain = Base64Decode(decResponse.plain);
                return new Payload(payload.KeyName, plain);
            }
            var errormessage = await response.Content.ReadAsStringAsync();
            var errorcode = response.StatusCode;
            Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            return new Payload(payload.KeyName, "");

        }

        public async Task<Key> RotateKey(string keyName)
        {
            await EnsureActiveSession();
            var oldKey = await GetKey(keyName);
            var content = new RotateKeyCall(keyName);
            var jsonContent = JsonSerializer.Serialize(content);
            HttpContent contentObject = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/crypto/v1/keys/rekey", contentObject);
            if (response.IsSuccessStatusCode)
            {
                var newkey = await response.Content.ReadFromJsonAsync<Key>();
                return newkey;
            }
            var errormessage = await response.Content.ReadAsStringAsync();
            var errorcode = response.StatusCode;
            Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            return new Key();

        }

        public async Task<Key> CreateNewKey(string keyName)
        {
            await EnsureActiveSession();

            //name and key size
            Key KeyRequest = new Key(keyName, 256, true);

            string KeyToCreate = JsonSerializer.Serialize(KeyRequest);

            //For testing error
            //var request = new HttpRequestMessage(HttpMethod.Post, "");

            var request = new HttpRequestMessage(HttpMethod.Post, "/crypto/v1/keys");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(KeyToCreate);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Key newKey = JsonSerializer.Deserialize<Key>(content);
                return newKey;
            }
            var errormessage = await response.Content.ReadAsStringAsync();
            var errorcode = response.StatusCode;
            Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            return new Key();
        }

        public async Task<Key> ExportKey(string keyname)
        {
            await EnsureActiveSession();
            var key = await GetKey(keyname);
            var content = new SObjectDescriptor(key.kid);
            var jsonContent = JsonSerializer.Serialize(content);
            HttpContent contentObject = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/crypto/v1/keys/export", contentObject);
            if (response.IsSuccessStatusCode)
            {
                var newkey = await response.Content.ReadFromJsonAsync<Key>();
                return newkey;
            }
            var errormessage = await response.Content.ReadAsStringAsync();
            var errorcode = response.StatusCode;
            Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            return new Key();

        }

        public async Task<WrappedKey> RotateKeyWrapped(string oldKeyName)
        {
            await EnsureActiveSession();
            var oldKey = await GetKey(oldKeyName);
            var newKey = await RotateKey(oldKeyName);
            var iv = Base64Encode(_rgbIv);
            var content = new WrapKeyRequest(newKey.kid, iv);
            var jsonContent = JsonSerializer.Serialize(content);
            HttpContent contentObject = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"/crypto/v1/keys/{oldKey.kid}/wrapkey", contentObject);
            if (response.IsSuccessStatusCode)
            {
                var wrappedKey = await response.Content.ReadFromJsonAsync<WrappedKey>();
                return wrappedKey;
            }
            var errormessage = await response.Content.ReadAsStringAsync();
            var errorcode = response.StatusCode;
            Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
            return new WrappedKey();



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
}
