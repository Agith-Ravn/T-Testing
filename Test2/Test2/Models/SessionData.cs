using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2.Client.Models
{
    class SessionData
    {
        public int _expires_in;
        public int expires_in //Verdien vil gå hit pga navnet (når du skal deserialize verdi | string > object)
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
}
