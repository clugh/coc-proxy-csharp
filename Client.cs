using System;
using System.Net;
using System.Net.Sockets;

namespace coc_proxy_csharp
{
    public class Client : ClientCrypto
    {
        public ClientState state = new ClientState();

        public Client(ServerState serverstate)
        {
            state.serverState = serverstate;
            state.clientKey = clientKey;
            state.serverKey = ClientCrypto.serverKey;
        }

        public void StartClient()
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Proxy.hostname);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, Proxy.port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.state.socket = socket;
                socket.Connect(remoteEndPoint);
                socket.BeginReceive(this.state.buffer, 0, State.BufferSize, 0, new AsyncCallback(Protocol.ReceiveCallback), this.state);

                Console.WriteLine("[INFO] Connected to {0} ...", socket.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
