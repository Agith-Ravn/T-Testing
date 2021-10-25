using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test5.Model
{
    //Temporary class
    class AllClasses
    {
    }

    public class LoginData
    {
        public string apiKey { get; set; }
        public string baseURI { get; set; }
    }

    public class SessionData
    {
        public int _expires_in;
        public int expires_in
        {
            get { return _expires_in; }
            set
            {
                _expires_in = value;
                sessionStart = DateTime.Now;
                sessionEnd = sessionStart.AddSeconds(value);
            }
        }
        public string access_token { get; set; }
        public string entity_id { get; set; }

        public DateTime sessionStart { get; set; }
        public DateTime sessionEnd { get; set; }
    }

    public class Key
    {
        public string Data { get; set; }
        public string DeviceID { get; set; }

        public Key()
        {

        }
    }

    public class Payload
    {
        public string Data { get; set; }
        public string DeviceID { get; set; }
    }
}
