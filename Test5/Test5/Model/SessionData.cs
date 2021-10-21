using System;

namespace Test5.M
{
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
}