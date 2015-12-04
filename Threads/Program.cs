using System;
using System.Threading;

namespace Threads
{

    class Program
    {
        private static void Main(string[] args)
        {
            Matrix m1 = new Matrix(3, 3);
            m1.FillWithRandom(10);
            Thread.Sleep(200);
            Matrix m2 = new Matrix(3, 3);
            m2.FillWithRandom(10);
            m1.Print();
            m2.Print();
            DateTime start = DateTime.Now;
            m1 = m1 * m2;
            DateTime end = DateTime.Now;
            TimeSpan ts = end - start;
            Console.WriteLine(ts);
            m1.Print();

        }

        private static void OrderedThreads()
        {
            object o1 = new object();

            Thread t1 = new Thread(() =>
            {
                lock (o1)
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine("1");
                        Thread.Sleep(200);
                    }
            });
            Thread t2 = new Thread(() =>
            {
                lock (o1)
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine("2");

                        Thread.Sleep(200);
                    }
            });
            Thread t3 = new Thread(() =>
            {
                lock (o1)
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine("3");
                        Thread.Sleep(200);
                    }
            });
            Thread t4 = new Thread(() =>
            {
                lock (o1)
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine("4");
                        Thread.Sleep(200);
                    }
            });


            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
        }
    }
}
