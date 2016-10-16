using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtocolCS;

namespace DummyClient
{
    partial class Program
    {
        public static void SendJoinGame()
        {
            var packet = new JoinGame()
            {
                matchToken = matchToken
            };

            SendPacket(wsGame, packet);
        }

        public static void SendEmptyFrame()
        {
            var packet = new Frame()
            {
                events = new IngameEvent[] { }
            };

            SendPacket(wsGame, packet);
        }
    }
}
