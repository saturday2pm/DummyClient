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
        public static void SendJoinQueue()
        {
            Console.WriteLine(currentPlayerId);
            var packet = new JoinQueue() { senderId = currentPlayerId };

            SendPacket(packet);
        }
    }
}
