using Sodium;

namespace coc_proxy_csharp
{
    public class ClientState : State
    {
        public ServerState serverState;

        public KeyPair clientKey;
        public byte[] serverKey, nonce;
    }
}
