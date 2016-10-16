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
        static WebSocket wsMatchMaking { get; set; }
        static WebSocket wsGame { get; set; }
        static int currentPlayerId { get; set; }

        static string matchToken { get; set; }

        static bool processGame { get; set; }
        static bool frameReceived { get; set; }

        static void Main(string[] args)
        {
            Serializer.senderId = currentPlayerId = new Random().Next(10000);

            Console.WriteLine("DummyClient");
            Console.Title = "DummyClient - " + currentPlayerId.ToString();

            wsMatchMaking = new WebSocket("ws://localhost:9916/mmaker?version=1.0.0");
            wsGame = new WebSocket("ws://localhost:9916/game?version=1.0.0");

            wsMatchMaking.ConnectAsync();
            wsGame.ConnectAsync();

            wsMatchMaking.OnOpen += Ws_OnOpen;
            wsMatchMaking.OnMessage += Ws_OnMessage;
            wsMatchMaking.OnClose += Ws_OnClose;
            wsGame.OnOpen += Ws_OnOpen;
            wsGame.OnMessage += Ws_OnMessage;
            wsGame.OnClose += Ws_OnClose;

            while (wsMatchMaking.ReadyState != WebSocketState.Open)
                ;
            while (wsGame.ReadyState != WebSocketState.Open)
                ;

            SendJoinQueue();

            while (true)
            {
                var ch = Console.ReadLine();

                if (ch == "join") SendJoinQueue();
                if (ch == "joinGame") SendJoinGame();

                if (processGame && frameReceived)
                {
                    SendEmptyFrame();
                    frameReceived = false;
                }
            }

            Console.Read();
        }

        static void SendPacket(WebSocket target, PacketBase packet)
        {
            var json = Serializer.ToJson(packet);

            Console.WriteLine("Send : " + json);
            target.Send(json);
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

            if (packet is MatchSuccess)
            {
                var matchSuccess = (MatchSuccess)packet;
                matchToken = matchSuccess.matchToken;

                SendJoinGame();
            }
            if (packet is StartGame)
            {
                var startGame = (StartGame)packet;

                Console.WriteLine("StartGae");
                Console.WriteLine("  - Players : " + startGame.players.Length.ToString());

                foreach (var player in startGame.players)
                    Console.WriteLine("    " + player.id);

                processGame = true;
                frameReceived = true;

                SendEmptyFrame();
            }
            if (packet is Frame)
            {
                var frame = (Frame)packet;

                frameReceived = true;

                SendEmptyFrame();
            }
        }
    }
}
