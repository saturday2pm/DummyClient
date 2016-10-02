using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtocolCS;
using WebSocketSharp;

namespace DummyClient
{
    partial class Program
    {
        static WebSocket ws { get; set; }
        static int currentPlayerId { get; set; }

        static void Main(string[] args)
        {
            Serializer.senderId = currentPlayerId = new Random().Next(10000);

            Console.WriteLine("DummyClient");
            Console.Title = "DummyClient - " + currentPlayerId.ToString();

            ws = new WebSocket("ws://localhost/mmaker?version=1.0.0");
            

            ws.ConnectAsync();

            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.OnClose += Ws_OnClose;

            while (ws.ReadyState != WebSocketState.Open)
                ;
            
            while (true)
            {
                Console.ReadKey();

                SendJoinQueue();
            }

            Console.Read();
        }

        static void SendPacket(PacketBase packet)
        {
            var json = Serializer.ToJson(packet);

            Console.WriteLine("Send : " + json);
            ws.Send(json);
        }

        private static void Ws_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("OnOpen");
        }

        private static void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine($"OnClose : {e.Code}, {e.Reason}");
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine($"OnMessage : {e.Data}");

            var packet = Serializer.ToObject(e.Data);

            Console.WriteLine($"  Packet : {packet.GetType()}");
        }
    }
}
