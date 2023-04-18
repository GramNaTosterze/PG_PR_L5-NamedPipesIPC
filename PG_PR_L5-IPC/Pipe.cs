using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace PG_PR_L5_IPC
{
    class Pipe
    {
        PipeStream In { get; }
        PipeStream Out { get; }
        StreamReader Reader { get; }
        StreamWriter Writer { get; }
        public Pipe(NamedPipeServerStream _in, NamedPipeServerStream _out)
        {
            In = _in;
            Out = _out;
            _in.WaitForConnection();
            _out.WaitForConnection();

            Reader = new StreamReader(In);
            Writer = new StreamWriter(Out);
        }
        public Pipe(NamedPipeClientStream _in, NamedPipeClientStream _out)
        {
            In = _in;
            Out = _out;
            _in.Connect();
            _out.Connect();

            Reader = new StreamReader(In);
            Writer = new StreamWriter(Out);
        }
        public void Write(UniqueList.MessageType messageType, dynamic msg)
        {
            Writer.WriteLine($"{messageType} {msg}");
            Writer.Flush();
        }
        public (UniqueList.MessageType, string) Read()
        {
            string data = null;
            while (data == null)
                data = Reader.ReadLine();
            string[] dataReceived = data.Split();
            return (Enum.Parse<UniqueList.MessageType>(dataReceived[0]), dataReceived[1]);
        }
    }
}
