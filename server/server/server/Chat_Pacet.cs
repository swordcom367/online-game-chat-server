﻿using System;
using System.Collections.Generic;
using System.Text;

namespace server_programs
{
    class packetCreator
    {
        List<Byte> buffer;
        int headerSize = 10;
        int userIDLength = 2;
       public packetCreator()
        {
            buffer = new List<byte>();
        }
       public Byte[] createPacet(int userID,string Message)
        {
            writeUserID(userID);
            write(Message);
            Byte[] packet;
            int messagelength = buffer.Count - userIDLength;
            string messageHeader = messagelength.ToString().PadRight(headerSize);
            Console.WriteLine(messageHeader);
            buffer.InsertRange(0, Encoding.ASCII.GetBytes(messageHeader));
            packet = buffer.ToArray();
            reset();
            return packet;
        }
       
         void write(Byte[] data)
        {
            buffer.AddRange(data);
        }
        void write(string s)
        {
            buffer.AddRange(Encoding.ASCII.GetBytes(s));
        }
        void writeUserID(int num)
        {
            if (num.ToString().Length <= 2)
            {
                buffer.AddRange(Encoding.ASCII.GetBytes(num.ToString().PadRight(userIDLength)));
            } else
            {
                Console.WriteLine("user Id out of scope");
                buffer.AddRange(Encoding.ASCII.GetBytes(0.ToString().PadRight(userIDLength)));
            }
        }
        void reset()
        {
            buffer.Clear();
        }
    }
    public class packetReader
    {
        int headerSize = 10;
        int userIDLength = 2;
        int Mesagelength;
        public packetReader()
        {

        }
        public void readPacet(Byte[] pac)
        {
           Mesagelength = readHeader(pac);
           Console.WriteLine("Message Length:" + Mesagelength);
           Console.WriteLine("UserID:" + ReadUserID(pac));
            Console.WriteLine("Message:" +ReadMessage(pac, Mesagelength));

        } 
        private int readHeader(Byte[] pac)
        {
            Byte[] headerByteArray = new Byte[headerSize];
            String headerString;
            int messageLegth=0;
            for (int i = 0; i < headerSize; i++)
            {
                headerByteArray[i] = pac[i];
            }
            headerString = Encoding.ASCII.GetString(headerByteArray);
            headerString.Trim();
            try
            {
                messageLegth = Int32.Parse(headerString);   
            } catch
            {
                Console.WriteLine("the header is coropted");
            }
            return messageLegth;
        }
        private int ReadUserID(Byte[] pac)
        {
            Byte[] userIDBytes = new Byte[userIDLength];
            String userIDString = "";
            int userID=0;
            for (int i = 0; i < userIDLength; i++)
            {
                userIDBytes[i] = pac[headerSize + i];
            }
            userIDString = Encoding.ASCII.GetString(userIDBytes);
            userIDString.Trim();
            try
            {
                userID = Int32.Parse(userIDString);
            }
            catch
            {
                Console.WriteLine("the header is coropted");
            }
            return userID;
        }
        private string ReadMessage(Byte[] pac,int messageLength)
        {
            Byte[] messageBytes = new Byte[messageLength];
            String Message;
            for (int i = 0; i < messageLength; i++)
            {
                messageBytes[i] = pac[headerSize + userIDLength + i];
            }
            Message = Encoding.ASCII.GetString(messageBytes);
            return Message;
        }
    }
}