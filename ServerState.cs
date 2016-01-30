using Sodium;

namespace coc_proxy_csharp
{
    public class ServerState : State
    {
        public ClientState clientState;

        public KeyPair serverKey;
        public byte[] clientKey, nonce, sessionKey, sharedKey;
    }
}
