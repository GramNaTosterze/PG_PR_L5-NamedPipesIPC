using PG_PR_L5_IPC;
using System;
using System.Data;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Xml.Linq;

class IPC
{
    const string pipeName = "PG_PR_L5-IPC";
    const string resName = "IPC-RES";
    static void Main(string[] args)
    {
        if (args.Length == 0)
            Server();
        else
            Client(args);

    }
    static void Server()
    {
        Console.WriteLine("parent");
        string clientPath = @"PG_PR_L5-IPC.exe";

        var startInfo = new ProcessStartInfo(clientPath, pipeName + " " + resName);
        startInfo.UseShellExecute = true;
        Process childProcess = Process.Start(startInfo);

        Pipe pipe = new Pipe(new NamedPipeServerStream(pipeName + "In"), new NamedPipeServerStream(pipeName + "Out"));
        Pipe responcePipe = new Pipe(new NamedPipeServerStream(resName + "In"), new NamedPipeServerStream(resName + "Out"));
        



        Console.WriteLine("Connected");

        new UniqueList(pipe, responcePipe);

        childProcess.WaitForExit();
    }
    static void Client(string[] args)
    {
        //create streams
        Pipe pipe = new Pipe(new NamedPipeClientStream(args[0] + "Out"), new NamedPipeClientStream(args[0] + "In"));
        Pipe responcePipe = new Pipe(new NamedPipeClientStream(args[1] + "Out"), new NamedPipeClientStream(args[1] + "In"));
        Console.WriteLine("child");

        new UniqueList(pipe, responcePipe);
        
        Console.ReadLine();
    }

}