using System;
using System.Linq;
using System.Net.Sockets;
using Sodium;

namespace coc_proxy_csharp
{
    public class ClientCrypto : Protocol
    {
        protected KeyPair clientKey = PublicKeyBox.GenerateKeyPair();
        protected static byte[] serverKey = Utilities.HexToBinary("01c98c143a840d92ee656996dad5af41de5d1b8ebb289081368b5cfda9bd4a30");

        public static void DecryptPacket(Socket socket, ClientState state, byte[] packet)
        {
            int messageId = BitConverter.ToInt32(new byte[2].Concat(packet.Take(2)).Reverse().ToArray(), 0);
            int payloadLength = BitConverter.ToInt32(new byte[1].Concat(packet.Skip(2).Take(3)).Reverse().ToArray(), 0);
            int unknown = BitConverter.ToInt32(new byte[2].Concat(packet.Skip(2).Skip(3).Take(2)).Reverse().ToArray(), 0);
            byte[] cipherText = packet.Skip(2).Skip(3).Skip(2).ToArray();
            byte[] plainText;

            if (messageId == 20100)
            {
                plainText = cipherText;
            }
            else if (messageId == 20104)
            {
                byte[] nonce = GenericHash.Hash(state.nonce.Concat(state.clientKey.PublicKey).Concat(state.serverKey).ToArray(), null, 24);
                plainText = PublicKeyBox.Open(cipherText, nonce, state.clientKey.PrivateKey, state.serverKey);
                state.serverState.nonce = plainText.Take(24).ToArray();
                state.serverState.sharedKey = plainText.Skip(24).Take(32).ToArray();
                plainText = plainText.Skip(24).Skip(32).ToArray();
            }
            else
            {
                state.serverState.nonce = Utilities.Increment(Utilities.Increment(state.serverState.nonce));
                plainText = SecretBox.Open(new byte[16].Concat(cipherText).ToArray(), state.serverState.nonce, state.serverState.sharedKey);
            }
            Console.WriteLine("{0} {1}", messageId, Utilities.BinaryToHex(BitConverter.GetBytes(messageId).Reverse().Skip(2).Concat(BitConverter.GetBytes(plainText.Length).Reverse().Skip(1)).Concat(BitConverter.GetBytes(unknown).Reverse().Skip(2)).Concat(plainText).ToArray()));
            ServerCrypto.EncryptPacket(state.serverState.socket, state.serverState, messageId, unknown, plainText);
        }

        public static void EncryptPacket(Socket socket, ClientState state, int messageId, int unknown, byte[] plainText)
        {
            byte[] cipherText;
            if (messageId == 10100)
            {
                cipherText = plainText;
            }
            else if (messageId == 10101)
            {
                byte[] nonce = GenericHash.Hash(state.clientKey.PublicKey.Concat(state.serverKey).ToArray(), null, 24);
                plainText = state.serverState.sessionKey.Concat(state.nonce).Concat(plainText).ToArray();
                cipherText = PublicKeyBox.Create(plainText, nonce, state.clientKey.PrivateKey, state.serverKey);
                cipherText = state.clientKey.PublicKey.Concat(cipherText).ToArray();
            }
            else
            {
                // nonce was already incremented in ServerCrypto.DecryptPacket
                cipherText = SecretBox.Create(plainText, state.nonce, state.serverState.sharedKey).Skip(16).ToArray();
            }
            byte[] packet = BitConverter.GetBytes(messageId).Reverse().Skip(2).Concat(BitConverter.GetBytes(cipherText.Length).Reverse().Skip(1)).Concat(BitConverter.GetBytes(unknown).Reverse().Skip(2)).Concat(cipherText).ToArray();
            socket.BeginSend(packet, 0, packet.Length, 0, new AsyncCallback(SendCallback), state);
        }
    }
}
