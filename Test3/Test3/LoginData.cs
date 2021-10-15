using System;

namespace Test3
{
    public class LoginData
    {
        public string apiKey { get; set; }
        public string bearertoken { get; set; }
        public DateTime sessionstart { get; set; }
        //The duration of the session in ms
        public int maxduration { get; set; }
        public string baseURI { get; set; }

    }
}