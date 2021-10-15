using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Test3
{
    class API
    {

        private static HttpClient _client;
        private readonly string _filepath;
        private LoginData _logindata;
        private static SessionData _sessionData;
        private List<Key> _keylist;

        public API(string filepath)
        {
            _filepath = filepath;
            _logindata = GetLoginDatafromFile(filepath);
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_logindata.baseURI);
            _sessionData = new SessionData();

        }

        private LoginData GetLoginDatafromFile(string path)
        {
            string jsonString = System.IO.File.ReadAllText(path);
            //Console.WriteLine(jsonString);
            LoginData loginData = JsonSerializer.Deserialize<LoginData>(jsonString);
            //Console.WriteLine(loginData.baseURI);

            return loginData;

        }

        public async Task GetAPIStatus()
        {
            string returnstring;
            try
            {
                HttpResponseMessage response = await _client.GetAsync("https://eu.smartkey.io/sys/v1/health");
                returnstring = "The server is handling requests.";
            }
            catch (HttpRequestException e)
            {
                returnstring = "\n The api has Problems!" + ("Message : {0} ", e.Message);
            }

            Console.WriteLine(returnstring);
        }

        public async Task StartSession()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _logindata.apiKey);
            try
            {

                HttpResponseMessage response = await _client.PostAsync("/sys/v1/session/auth", null);
                string responseBody = await response.Content.ReadAsStringAsync();
                _sessionData = JsonSerializer.Deserialize<SessionData>(responseBody);
                Console.WriteLine(responseBody);
                Console.WriteLine(_sessionData.expires_in);
                Console.WriteLine(_sessionData.sessionstart);
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
            int res = _sessionData.sessionend.CompareTo(DateTime.Now);
            if (res >= 0)
            {
                Console.WriteLine("Session active until:" + _sessionData.sessionend);
                return true;

            }
            else
            {
                Console.WriteLine("Session expired: " + _sessionData.sessionend);
                return false;
            }
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
            HttpResponseMessage response = await _client.GetAsync("/crypto/v1/keys");
            string responseString = await response.Content.ReadAsStringAsync();
            _keylist = JsonSerializer.Deserialize<List<Key>>(responseString);
            Console.WriteLine("Number of keys found: " + _keylist.Count);

            Console.WriteLine("--------------------------");
        }

        //public async Task<Key> GetKeyByName(string keyname)
        //{
        //    Key returnkey = new Key();
        //    foreach (var key in _keylist)
        //    {
        //        if (key.name == keyname)
        //        {
        //            returnkey = await GetKeyByID(key.kid);
        //        }
        //    }

        //    return returnkey;

        //}

        //public async Task<Key> GetKeyByID(string kid)
        //{
        //    Key returnkey = new Key();
        //    try
        //    {
        //        HttpResponseMessage response = await _client.GetAsync("/crypto/v1/keys/" + kid);
        //        string responseBody = await response.Content.ReadAsStringAsync();
        //        returnkey = JsonSerializer.Deserialize<Key>(responseBody);
        //        Console.WriteLine(String.Format("The key with the name {0} has been found", returnkey.name));

        //    }
        //    catch (HttpRequestException e)
        //    {
        //        Console.WriteLine("\nException Caught!");
        //        Console.WriteLine("Message :{0} ", e.Message);

        //    }

        //}

        //public async Task<string> Encrypt(string plaintext)
        //{

        //}




    }

    public class EncryptionCall
    {
        public string key { get; set; }
        public string alg { get; set; }
        public string plain { get; set; }
    }
}