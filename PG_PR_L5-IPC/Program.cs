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
        var pipe = new NamedPipeServerStream(pipeName);
        Console.Write("parent");
        //start client, pass pipe ids as command line parameter 
        string clientPath = @"C:\Users\krzys\source\repos\PG_PR_L5-IPC\PG_PR_L5-IPC\bin\Debug\net6.0\PG_PR_L5-IPC.exe";

        var startInfo = new ProcessStartInfo(clientPath, pipeName);
        startInfo.UseShellExecute = true;
        Process childProcess = Process.Start(startInfo);
        pipe.WaitForConnection();
        Console.WriteLine("Connected");

        new UniqueList(pipe);

        childProcess.WaitForExit();
    }
    static void Client(string[] args)
    {
        //get pipe handle id
        Console.WriteLine("child");

        //create streams
        var pipe = new NamedPipeClientStream(args[0]);
        pipe.Connect();


        new UniqueList(pipe);
        
        Console.ReadLine();
    }

}