﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace server_programs
{
    class chat_server
    {
        int HeaderSize=10;
        Dictionary<int, string> users = new Dictionary<int, string>();
        Dictionary<int, TcpClient> userClient = new Dictionary<int, TcpClient>();
        int userCount = 0;
        int NumberToAssine =0;
        public void start(int _port)
        {
            int port = _port;
            TcpListener server = new TcpListener(IPAddress.Any,port);
            server.Start();
            Console.WriteLine("ready to reseve conections");
            while (true)
            {
                server.BeginAcceptTcpClient(handdleConections, server);
            }
     
        }
        void handdleConections(IAsyncResult result)
        {
            TcpListener server = (TcpListener)result.AsyncState;
            TcpClient client = server.EndAcceptTcpClient(result);
            packetReader reader = new packetReader();
            Byte[] data = new Byte[256];
            IPAddress clinetIp = IPAddress.Parse(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

            Console.WriteLine("incomint conection");
            NetworkStream stream = client.GetStream();
            stream.Read(data, 0, data.Length);
            if (reader.readVerification(data))
            {
                comandInterpriter(reader.readCMD(data),data,client);
            }
            Thread receveMSG = new Thread(() => receveMessages(client));
            receveMSG.Start();
            
        }
        void comandInterpriter(int cmd, Byte[] pac, TcpClient client)
        {
            //Console.WriteLine("entering the comand interpriter not enter safety mode yet");
            if (cmd <= 3)
            {
              //  Console.WriteLine("entered the cmd structue");
                switch (cmd)
                {
                    case 0:
                        Console.WriteLine("starting setion ");
                        startSetion(pac,client);
                        break;
                    case 1:
                        //Change username
                        Console.WriteLine("change username comand called");
                        break;
                    case 2:
                        Send(pac);
                        Console.WriteLine("send message comand called");
                        break;
                    case 3:
                        Console.WriteLine("disconect comand called");
                        disconect(pac);
                        break;
                }
            } else
            {
                Console.WriteLine("invaled comand");
            }
            
        }
        void startSetion(Byte[] pac,TcpClient client)
        {
            int AssiningNum=0;
            Console.WriteLine("start setion comand called");
            packetReader reader = new packetReader();
            packetCreator creator = new packetCreator();
            if (NumberToAssine == 0)
            {
              userCount += 1;
                AssiningNum = userCount;
            } else
            {
                AssiningNum = NumberToAssine;
                NumberToAssine = 0;
            }

            users.Add(AssiningNum, reader.ReadMessage(pac, reader.readHeader(pac)));
            userClient.Add(AssiningNum, client);
            foreach(KeyValuePair<int,String> kvp in users)
            {
                Console.WriteLine("key = {0}, value ={1}",kvp.Key,kvp.Value);
            }
            foreach (KeyValuePair<int, TcpClient> kvp in userClient)
            {
              //writes the client ID and IPaddress
                Console.WriteLine("key = {0}, value ={1}", kvp.Key,((IPEndPoint)kvp.Value.Client.RemoteEndPoint).Address.ToString());
            }
            Byte[] newUserPac = creator.createPacet(AssiningNum, 0, users[AssiningNum]);
            NetworkStream stream = client.GetStream();
            stream.Write(newUserPac, 0, newUserPac.Length);
            stream.Flush();
            Send(newUserPac);
             foreach (KeyValuePair<int, string> kvp in users)
             {
                if (kvp.Key != userCount)
                {
                    newUserPac = creator.createPacet(kvp.Key,0,kvp.Value);
                    stream.Write(newUserPac);
                    stream.Flush();
                }
             }
        }
        void Send(Byte[] pac)
        {
            packetReader reader = new packetReader();
            foreach (KeyValuePair<int,TcpClient> kvp in userClient)
            {
                Console.WriteLine("entering foreach");
                if(kvp.Key != reader.ReadUserID(pac) )
                {
                    Console.WriteLine("should be sending to other clients");
                    TcpClient client = kvp.Value;
                    NetworkStream stream = client.GetStream();
                    stream.Write(pac, 0, pac.Length);
                }
            }
            
        }
        void disconect(Byte[] pac)
        {
            packetReader reader = new packetReader();
            users.Remove(reader.ReadUserID(pac));
            userClient.Remove(reader.ReadUserID(pac));
            NumberToAssine = reader.ReadUserID(pac);
        }
        //this is the infint lissen loop
        void receveMessages(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            packetReader reader = new packetReader();
            while(true)
            {
                Console.WriteLine(" in the receve loop");
                Byte[] buffer = new Byte[256];
                try
                {
                    stream.Read(buffer, 0, buffer.Length);
                    Console.WriteLine("the user id is " + reader.ReadUserID(buffer));
                    comandInterpriter(reader.readCMD(buffer), buffer, client);
                    Console.WriteLine(reader.ReadMessage(buffer, reader.readHeader(buffer)));
                    if (reader.readCMD(buffer)==3)
                    {
                        Thread.CurrentThread.Join();
                    }
                } catch(Exception e)
                {
                    Console.WriteLine("this is throwing an exceion " + e);
                }
            } 
        }
    }
}
