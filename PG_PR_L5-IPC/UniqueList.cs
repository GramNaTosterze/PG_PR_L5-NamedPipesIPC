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
            
            


            int LOOP = 100;

            write = new Thread(() =>
            {
                for(int i = 0; i < LOOP; i++)
                {
                    //lock (numbers)
                    {
                        int num = new Random().Next(100);
                        if (!numbers.Contains(num))
                        {
                            //send number
                            mainPipe.Write(MessageType.Number, num);

                            //read responce
                            string res;
                            (messageType, res) = responcePipe.Read();
                            bool canAdd = Boolean.Parse(res);

                            if (canAdd && !numbers.Contains(num))
                            {
                                Console.WriteLine($"{num}");
                            }
                            else
                                Console.WriteLine($"been in other proces");

                        }
                        else
                        {
                            Console.WriteLine($"been here");
                            mainPipe.Write(MessageType.Skip, num);
                        }
                        mainPipe.WaitForDrain();
                    }
                }
            });

            read = new Thread(() =>
            {
                for(int i = 0; i < LOOP; i++)
                {
                    //read number
                    bool contain;
                    lock (numbers)
                    {
                        string res;
                        (messageType, res) = mainPipe.Read();
                        if (messageType == MessageType.Skip)
                            continue;

                        int number = Int32.Parse(res);
                        if (numbers.Contains(number))
                            contain = true;
                        else
                        {
                            contain = false;
                            numbers.Add(number);
                            Console.WriteLine($" {number}");
                        }
                    }
                    //send responce
                    responcePipe.Write(MessageType.Response, !contain);
                    responcePipe.WaitForDrain();
                }
            });

            read.Start();
            write.Start();

            read.Join();
            write.Join();

        }
        
    }
}
