using System;

namespace coc_proxy_csharp
{
    class Proxy
    {
        public static string hostname = "gamea.clashofclans.com";
        public static int port = 9339;

        private static void Main(string[] args)
        {
            try
            {
                Server server = new Server(Proxy.port);
                server.StartServer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
