using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Test2
{
    class Program
    {
        /*
         * while loop, hvis du mister kobling break loop >
         *      Lager en ny object inni while > object sjekker app har kobling > bytter om bool verdi til true
         *
         * legg try catch rundt object?? hvis det er error?
         *
         *
         *
         */

        static async Task Main(string[] args)
        {
            var api = new ConsumeAPI();
            api.ConnectToAPI();
        }
    }
}
