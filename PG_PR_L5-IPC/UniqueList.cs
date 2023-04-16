using System;
using System.IO.Pipes;

namespace PG_PR_L5_IPC
{
    internal class UniqueList
    {
        List<int> numbers = new List<int>();
        StreamReader reader;
        StreamWriter writer;
        enum MessageType
        {
            Number,
            Response
        }
        public UniqueList(PipeStream pipe)
        {
            reader = new StreamReader(pipe);
            writer = new StreamWriter(pipe);

            MessageType messageType = MessageType.Response;
            bool contain = false;
            int num = 12;// new Random().Next();
            bool waitForResponce = false;

            Thread read = new Thread(() =>
            {
                string res;
                (messageType, res) = ReadFromStream();

                switch (messageType)
                {
                    case MessageType.Number:
                        int number = Int32.Parse(res);
                        if (numbers.Contains(number))
                            contain = false;
                        else
                        {
                            contain = true;
                            numbers.Add(number);
                        }
                        break;

                    case MessageType.Response:
                        bool canAdd = Boolean.Parse(res);

                        if (canAdd)
                            numbers.Add(num);
                        else
                            Console.WriteLine($"cannot add {num}");
                        break;
                }
            });

            Thread write = new Thread(() =>
            {
                //send data
                if(!waitForResponce)
                    WriteToStream(MessageType.Number, num);
                else
                    WriteToStream(MessageType.Response, contain);
                //waitForResponce = !waitForResponce;
            });

            read.Start();
            write.Start();

            read.Join();
            write.Join();
        }
        private void WriteToStream(MessageType messageType, dynamic msg)
        {
            writer.WriteLineAsync($"{messageType} {msg}");
            Console.WriteLine($"Sent: {messageType} {msg}");
            writer.FlushAsync();
        }
        private (MessageType, string) ReadFromStream()
        {
            string data = null;
            while (data == null)
                data = reader.ReadLine();
            string[] dataReceived = data.Split();
            Console.WriteLine($"Received: {dataReceived[0]} {dataReceived[1]}");
            return (Enum.Parse<MessageType>(dataReceived[0]), dataReceived[1]);
        }
    }
}
