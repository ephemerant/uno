﻿using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualBasic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Windows.Threading;
using System.Collections.Generic;

namespace UNO
{
    public class UDP
    {
        public string name = "";
        public MainWindow window;

        public UdpClient udpResponse = new UdpClient(42424, AddressFamily.InterNetwork);

        public String getName()
        {
            String name = "";
            while (name.Length == 0)
                name = Interaction.InputBox("What is your name?").Trim();
            return name;
        }

        public String getIP()
        {
            String IP = "";
            while (IP.Length == 0)
                IP = Interaction.InputBox("What is the IP?").Trim();
            return IP;
        }

        public void SendMessage(object message)
        {
            // send via UDP
            UdpClient client = new UdpClient(24242, AddressFamily.InterNetwork);
            client.EnableBroadcast = true;

            IPEndPoint groupEp = new IPEndPoint(IPAddress.Broadcast, 42424);
            client.Connect(groupEp);

            var data = Encoding.ASCII.GetBytes(message.ToString());
            client.Send(data, data.Length);

            client.Close();
        }

        //alternative sendMessage with IP address
        public void SendMessage(string message, string inputIP)
        {
            // send via UDP
            UdpClient client = new UdpClient(24242, AddressFamily.InterNetwork);
            client.EnableBroadcast = true;

            IPEndPoint groupEp = new IPEndPoint(IPAddress.Parse(inputIP), 42424);
            client.Connect(groupEp);

            var data = Encoding.ASCII.GetBytes(message);

            client.Send(data, data.Length);
            client.Close();
        }

        public void ReceiveMessage()
        {
            try
            {
                while (true)
                {
                    IPEndPoint recvEp = new IPEndPoint(IPAddress.Any, 0);

                    var text = Encoding.ASCII.GetString(udpResponse.Receive(ref recvEp));

                    var message = new Message(text);

                }
            }
            catch (Exception ex)
            {
                // did we force close the connection?
                if (ex.Message != "A blocking operation was interrupted by a call to WSACancelBlockingCall")
                    throw ex;
            }
        }

        private void EndReceive(object sender, CancelEventArgs e)
        {
            udpResponse.Close();
        }

        public void BroadcastHost()
        {
            while (window.currentScreen == window.lobby)
            {
                var players = new List<Player>();

                foreach (var player in window.playerList)
                {
                    players.Add(new Player { name = player.name, IP = player.IP, isComputer = player.isComputer });
                }

                SendMessage(new Message { HostID = window.HostID, Action = "hosting", PlayerName = window.lobby.clientName, PlayerList = players });
                Thread.Sleep(5000);
            }
        }

        public void ListenForClients()
        {
            try
            {
                while (window.currentScreen == window.lobby)
                {
                    IPEndPoint recvEp = new IPEndPoint(IPAddress.Any, 0);

                    var text = Encoding.ASCII.GetString(udpResponse.Receive(ref recvEp));

                    var message = new Message(text);

                    if (message.HostID == window.HostID && message.Action == "join")
                    {
                        SendMessage(new Message { HostID = window.HostID, Action = "joinAck", PlayerID = message.PlayerID, PlayerName = message.PlayerName });
                    }
                    if (message.HostID == window.HostID && message.Action == "joinAckAck")
                    {
                        window.lobby.addClient(message);
                    }
                }
            }
            catch (Exception ex)
            {
                // did we force close the connection?
                if (ex.Message != "A blocking operation was interrupted by a call to WSACancelBlockingCall")
                    throw ex;
            }
        }

        public void ListenForHosts()
        {
            try
            {
                while (window.currentScreen == window.lobby)
                {
                    IPEndPoint recvEp = new IPEndPoint(IPAddress.Any, 0);

                    var text = Encoding.ASCII.GetString(udpResponse.Receive(ref recvEp));

                    var message = new Message(text);
                    Application.Current.Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        if (message.Action.Equals("hosting") && !window.lobby.hosts.ContainsKey(message.HostID))
                        {
                            window.lobby.hosts.Add(message.HostID, message.PlayerName);
                            window.lobby.reloadHostList();
                        }
                        if (message.Action.Equals("joinAck") && window.lobby.hosts.ContainsKey(message.HostID) && message.PlayerID == window.HostID)
                        {
                            SendMessage(new Message { HostID = window.HostID, Action = "joinAckAck", PlayerName = window.lobby.clientName });
                        }

                    }));

                }
            }
            catch (Exception ex)
            {
                // did we force close the connection?
                if (ex.Message != "A blocking operation was interrupted by a call to WSACancelBlockingCall")
                    throw ex;
            }
        }

        public void ListenForMoves()
        {
            try
            {
                while (window.currentScreen == window.game)
                {
                    IPEndPoint recvEp = new IPEndPoint(IPAddress.Any, 0);

                    var text = Encoding.ASCII.GetString(udpResponse.Receive(ref recvEp));

                    var message = new Message(text);
                }
            }
            catch (Exception ex)
            {
                // did we force close the connection?
                if (ex.Message != "A blocking operation was interrupted by a call to WSACancelBlockingCall")
                    throw ex;
            }
        }
    }

    public class Message
    {
        public string HostID { get; set; }
        public string PlayerID { get; set; }
        public string Action { get; set; }
        public string Extra { get; set; }
        public string PlayerName { get; set; }
        public List<Player> PlayerList { get; set; }
        public Card Card { get; set; }

        public Message()
        {

        }

        public Message(string data)
        {
            Console.WriteLine(data);

            var message = JsonConvert.DeserializeObject<Message>(data);

            HostID = message.HostID;
            PlayerID = message.PlayerID;
            Action = message.Action;
            PlayerName = message.PlayerName;
            PlayerList = message.PlayerList;
            Card = message.Card;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
