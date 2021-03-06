﻿using System;

namespace chatClientNetworking
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            Chat_client client = new Chat_client("10.1.1.48", 1234);
            packetCreator PacketHandler = new packetCreator();
            Console.Write("enter username ");
            client.Start(Console.ReadLine());
            while(true)
            {
                Console.WriteLine("enter message or command");
                string clientInput = Console.ReadLine();
                if (clientInput != "quit")
                {
                    if (clientInput != "change usernames")
                    {
                        client.sendMessage(PacketHandler.createPacet(client.UserID, 2, clientInput));
                    } else
                    {
                        client.sendMessage(PacketHandler.createPacet(client.UserID, 1, clientInput));
                    }
                }
                else
                {
                    client.disconectSelf(client.client);
                }
            }
                
        }
    }
}
