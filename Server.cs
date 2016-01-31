using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace coc_proxy_csharp
{
    public class Server : ServerCrypto
    {
        private int port;
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        public Server(int port)
        {
            this.port = port;
        }

        public void StartServer()
        {
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
                Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(100);

                Console.WriteLine("[INFO] Started Listener on Port {0} ...", port);

                while (true)
                {
                    Server.allDone.Reset();
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket socket = listener.EndAccept(ar);

                ServerState state = new ServerState();
                state.socket = socket;
                state.serverKey = ServerCrypto.serverKey;

                Console.WriteLine("[INFO] Connection from {0} ...", socket.RemoteEndPoint.ToString());

                Client client = new Client(state);
                client.StartClient();
                client.state.serverState = state;
                state.clientState = client.state;

                socket.BeginReceive(state.buffer, 0, State.BufferSize, 0, new AsyncCallback(Protocol.ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
