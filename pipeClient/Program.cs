using System;
using System.IO.Pipes;
using System.Reflection;

class PipeClient
{
    static void Main(string[] args)
    {
        string parentSenderID;
        string parentReceiverID;

        //get pipe handle id
        //parentSenderID = args[0];
        //parentReceiverID = args[1];
        Console.WriteLine("parent: " + args[0]);

        //create streams
        //var receiver = new AnonymousPipeClientStream(PipeDirection.In, parentSenderID);
        //var sender = new AnonymousPipeClientStream(PipeDirection.Out, parentReceiverID);
        var child = new NamedPipeClientStream(args[0]);
        child.Connect();


        //read data
        int dataReceive = child.ReadByte();
        Console.WriteLine("Client receive: " + dataReceive.ToString());

        //write data
        byte dataSend = 24;
        child.WriteByte(dataSend);
        Console.WriteLine("Client send: " + dataSend.ToString());
        /*
        //read data
        int dataReceive = receiver.ReadByte();
        Console.WriteLine("Client receive: " + dataReceive.ToString());
        
        //write data
        byte dataSend = 24;
        sender.WriteByte(dataSend); 
        Console.WriteLine("Client send: " + dataSend.ToString());
        */
        Console.ReadLine();
    }
}