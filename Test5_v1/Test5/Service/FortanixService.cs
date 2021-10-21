using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Test5.M;
using Test5.Model;

namespace Test5
{
    public class API
    {
        public bool _loggedIn { get; set; }
        private LoginData _loginData { get; set; }

        private HttpClient _httpClient;
        private SessionData _sessionData { get; set; }

        private List<SecurityObject> _securityObjectList { get; set; }

        //SessionData
        //LoginData

        public API(string filepath)
        {
            _loginData = GetLoginDataFromFile(filepath);
            _loggedIn = false;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_loginData.baseURL);
            _sessionData = new SessionData();
        }

        private LoginData GetLoginDataFromFile(string filepath)
        {
            var jsonString = File.ReadAllText(filepath);
            var loginData = JsonSerializer.Deserialize<LoginData>(jsonString);
            return loginData;
        }

        public async Task connectToAPI()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _loginData.apiKey);

            var request = new HttpRequestMessage(HttpMethod.Post, "sys/v1/session/auth");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            _sessionData = JsonSerializer.Deserialize<SessionData>(content,
                  new JsonSerializerOptions
                  {
                      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                  });

            _loggedIn = true;

        }

        public async Task RenewConnectionToAPI()
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionData.access_token);

                var request = new HttpRequestMessage(HttpMethod.Post, "sys/v1/session/refresh");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);

                //This one needs to be fixed... API.. dosen't return anything..
                //How long will session last after refresh? 600 seconds?
                _sessionData.sessionStart = DateTime.Now;
                _sessionData.sessionEnd = _sessionData.sessionStart.AddSeconds(600);

                _loggedIn = true;

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                Console.ReadLine();
            }

        }

        public async Task GetAllSecurityObjects()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sessionData.access_token);

            var request = new HttpRequestMessage(HttpMethod.Get, "crypto/v1/keys");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            _securityObjectList = JsonSerializer.Deserialize<List<SecurityObject>>(content);

            foreach (var securityObject in _securityObjectList)
            {
                Console.WriteLine(securityObject.name);
            }
            Console.WriteLine("\n");
        }

        private async Task<SecurityObjectData> GetKeyId(string deviceID)
        {
            SecurityObjectData returnkey = null;
            {
                var response = await _httpClient.GetAsync("/crypto/v1/keys?name=" + deviceID);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    List<SecurityObjectData> keylist = JsonSerializer.Deserialize<List<SecurityObjectData>>(responseBody);
                    if (keylist != null && keylist.Count == 1)
                    {
                        returnkey = keylist[1];
                    }
                    else
                    {
                        Console.WriteLine("There is no Key for this DeviceId!");
                        Console.ReadLine();

                        // Needs a path to go back to the main menu.
                    }
                }
                else
                {
                    var errormessage = await response.Content.ReadAsStringAsync();
                    var errorcode = response.StatusCode;
                    Console.WriteLine($"Error {(int)errorcode}: \nMessage: {errormessage}");
                }
            }

            return returnkey;
        }

        //Encrypt
        public async Task Encrypt(SecurityObjectData SOD)
        {
            //await IsSessionActive(); //FIX!

            SecurityObjectData keyId = GetKeyId(SOD.DeviceID).Result;


        }

        private async Task IsSessionActive()
        {

            int resultCompare = DateTime.Compare(_sessionData.sessionEnd, DateTime.Now);

            //hvis sessionEnd er før tiden nå eller lik 0
            if (resultCompare <= 0)
            {
                await connectToAPI();
                Console.WriteLine("Connecting to API again");
            }
            //else
            //{
            //    await RenewConnectionToAPI();
            //    Console.WriteLine("Renewing connection");
            //}


        }

        //Decrypt
        public async Task Decrypt(SecurityObjectData SOD)
        {


        }
    }

    class SecurityObject
    {
        public string name { get; set; }
        public string description { get; set; }
        public string acct_id { get; set; }
        public CreatorType creator { get; set; }

        public string kid { get; set; }
        
        //obj_tyoe

        public string origin { get; set; }

        //Trenger sikkert flere fields..
    }

    class CreatorType
    {
        public string app { get; set; }
        public string user { get; set; }
    }

    public class SObjectDescriptor
    {
        public string Kid { get; set; }
    }
    public class EncryptionCall
    {
        public SObjectDescriptor key { get; set; }
        public string alg { get; set; }
        public string plain { get; set; }
        public string mode { get; set; }
        public string iv { get; set; }
    }

    public class DecryptionCall
    {
        public SObjectDescriptor key { get; set; }
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
}