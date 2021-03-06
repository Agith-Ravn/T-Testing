using System;

namespace Test3
{
    public class SessionData
    {
        public string access_token { get; set; }
        public string entity_id { get; set; }
        private double _expires_in;
        public DateTime sessionend { get; set; }
        public DateTime sessionstart { get; set; }

        public double expires_in
        {
            get => _expires_in;
            set
            {
                _expires_in = value;
                sessionstart = DateTime.Now;
                sessionend = sessionstart.AddSeconds(value);
            }
        }

    }
}