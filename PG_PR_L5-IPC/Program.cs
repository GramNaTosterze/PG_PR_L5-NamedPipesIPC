using PG_PR_L5_IPC;
using System;
using System.Data;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;

class IPC
{
    const string pipeName = "PG_PR_L5-IPC";
    static void Main(string[] args)
    {
        if (args.Length == 0)
            Server();
        else
            Client(args);

    }
    static void Server()
    {
        var pipeIn = new NamedPipeServerStream(pipeName+"In");
        var pipeOut = new NamedPipeServerStream(pipeName+"Out");
        Console.WriteLine("parent");

        string clientPath = @"C:\Users\krzys\source\repos\PG_PR_L5-IPC\PG_PR_L5-IPC\bin\Debug\net6.0\PG_PR_L5-IPC.exe";

        var startInfo = new ProcessStartInfo(clientPath, pipeName);
        startInfo.UseShellExecute = true;
        Process childProcess = Process.Start(startInfo);
        pipeIn.WaitForConnection();
        pipeOut.WaitForConnection();
        Console.WriteLine("Connected");

        new UniqueList(pipeIn, pipeOut);

        childProcess.WaitForExit();
    }
    static void Client(string[] args)
    {
        //get pipe handle id
        Console.WriteLine("child");

        //create streams
        var pipeIn = new NamedPipeClientStream(args[0]+"Out");
        var pipeOut = new NamedPipeClientStream(args[0]+"In");
        pipeIn.Connect();
        pipeOut.Connect();


        new UniqueList(pipeIn, pipeOut);
        
        Console.ReadLine();
    }

}