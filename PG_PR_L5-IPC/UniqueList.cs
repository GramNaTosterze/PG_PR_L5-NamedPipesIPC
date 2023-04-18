using System;
using System.IO.Pipes;
using static IPC;

namespace PG_PR_L5_IPC
{
    internal class UniqueList
    {
        List<int> numbers = new List<int>();
        Thread read;
        Thread write;
        Pipe mainPipe;
        Pipe responcePipe;
        public enum MessageType
        {
            Number,
            Response,
            Skip
        }
        public UniqueList(Pipe _mainPipe, Pipe _responcePipe)
        {
            mainPipe = _mainPipe;
            responcePipe = _responcePipe;

            MessageType messageType = MessageType.Response;
            bool contain = false;
            


            Semaphore sendNum = new Semaphore(1, 1);
            Semaphore readNum = new Semaphore(0, 1);
            int LOOP = 100000;

            write = new Thread(() =>
            {
                for(int i = 0; i < LOOP; i++)
                {
                    //sendNum.WaitOne();
                    lock (numbers)
                    {
                        int num = new Random(100).Next();
                        if (!numbers.Contains(num))
                        {
                            //send number
                            mainPipe.Write(MessageType.Number, num);
                            Console.WriteLine($"Sent: {num}");

                            //read responce
                            string res;
                            //(messageType, res) = responcePipe.Read();
                            //bool canAdd = Boolean.Parse(res);

                            //if (canAdd && !numbers.Contains(num))
                            //{
                            //    Console.WriteLine($"SentSucc: {num}");
                            //}
                            //else
                            //    Console.WriteLine($"cannot add");
                        }
                    }
                    //readNum.Release();
                }
            });

            read = new Thread(() =>
            {
                for(int i = 0; i < LOOP; i++)
                {
                    //readNum.WaitOne();
                    //read number
                    lock (numbers)
                    {
                        string res;
                        (messageType, res) = mainPipe.Read();
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

                    //send responce
                    //responcePipe.Write(MessageType.Response, !contain);

                    //sendNum.Release();
                }
            });

            read.Start();
            write.Start();

            read.Join();
            write.Join();

        }
        
    }
}
