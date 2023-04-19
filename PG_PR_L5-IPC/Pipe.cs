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
        StreamReader reader;
        StreamWriter writer;
        public Pipe(NamedPipeServerStream _in, NamedPipeServerStream _out)
        {
            In = _in;
            Out = _out;
            _in.WaitForConnection();
            _out.WaitForConnection();
            CreateStreams();
        }
        public Pipe(NamedPipeClientStream _in, NamedPipeClientStream _out)
        {
            In = _in;
            Out = _out;
            _in.Connect();
            _out.Connect();
            CreateStreams();
        }
        public void Write(UniqueList.MessageType messageType, dynamic msg)
        {
            writer.WriteLine($"{messageType} {msg}");
            writer.Flush();
        }
        public (UniqueList.MessageType, string) Read()
        {
            string data = reader.ReadLine();
            string[] dataReceived = data.Split();
            return (Enum.Parse<UniqueList.MessageType>(dataReceived[0]), dataReceived[1]);
        }
        private void CreateStreams()
        {
            reader = new StreamReader(In);
            writer = new StreamWriter(Out);
        }
        public void WaitForDrain()
        {
            Out.WaitForPipeDrain();
        }
    }
}
