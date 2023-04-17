using System;
using System.IO.Pipes;

namespace PG_PR_L5_IPC
{
    internal class UniqueList
    {
        List<int> numbers = new List<int>();
        StreamReader reader;
        StreamWriter writer;
        Thread read;
        Thread write;
        enum MessageType
        {
            Number,
            Response,
            Skip
        }
        public UniqueList(PipeStream pipeIn, PipeStream pipeOut)
        {
            reader = new StreamReader(pipeIn);
            writer = new StreamWriter(pipeOut);

            MessageType messageType = MessageType.Response;
            bool contain = false;
            


            Semaphore sem = new Semaphore(0, 1);
            Semaphore sem2 = new Semaphore(0, 1);
            int num = 0;
            int LOOP = 1000000;

            read = new Thread(() =>
            {
                for(int i = 0; i < LOOP; i++)
                {
                    string res;
                    (messageType, res) = ReadFromStream();
                    sem2.WaitOne();    
                    switch (messageType)
                    {
                        case MessageType.Number:
                            lock (numbers)
                            {
                                int number = Int32.Parse(res);
                                if (numbers.Contains(number))
                                    contain = true;
                                else
                                {
                                    contain = false;
                                    lock (numbers)
                                        numbers.Add(number);
                                    Console.WriteLine($"Rec: {number}");
                                }
                            }
                            break;

                        case MessageType.Response:
                            bool canAdd = Boolean.Parse(res);

                            if (canAdd && !numbers.Contains(num))
                            {
                                Console.WriteLine($"SentSucc: {num}");
                            }
                            else
                                Console.WriteLine($"cannot add");
                            break;
                        case MessageType.Skip:
                            break;
                    }
                    sem.Release();
                }
            });

            write = new Thread(() =>
            {
                sem.Release();
                for(int i = 0; i < LOOP ; i++)
                {
                    num =  new Random().Next();
                    sem.WaitOne(); ;
                    //send data
                    if (messageType != MessageType.Number)
                    {
                        lock(numbers)
                        {
                            if (!numbers.Contains(num))
                            {
                                WriteToStream(MessageType.Number, num);
                                pipeOut.WaitForPipeDrain();
                            }
                            else
                            {
                                Console.WriteLine($"cannot add2");
                                WriteToStream(MessageType.Skip, 0);
                            }
                        }
                    }
                    else
                    {
                        WriteToStream(MessageType.Response, !contain);
                        pipeOut.WaitForPipeDrain();
                    }
                    sem2.Release();
                }
            });

            read.Start();
            write.Start();

            read.Join();
            write.Join();

            Console.Write(numbers.ToString);
        }
        private void WriteToStream(MessageType messageType, dynamic msg)
        {
            writer.WriteLine($"{messageType} {msg}");
            writer.Flush();
        }
        private (MessageType, string) ReadFromStream()
        {
            string data = null;
            while (data == null)
                data = reader.ReadLine();
            string[] dataReceived = data.Split();
            return (Enum.Parse<MessageType>(dataReceived[0]), dataReceived[1]);
        }
    }
}
