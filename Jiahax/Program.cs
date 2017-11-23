using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jiahax
{
    class Program
    {
        private static readonly object lockObj = new object();
        private static SemaphoreSlim slim = new SemaphoreSlim(2);
        private static bool IsComplete { get; set; }

        static void Main(string[] args)
        {
            Say();

            Task.Run((Action)Test_lock);
            Task.Run((Action)Test_lock);
            Task.Run((Action)Test_lock);
            Task.Run((Action)Test_lock);
            Task.Run((Action)Test_lock);

            for (int i = 0; i < 5; i++)
            {
                ThreadPool.QueueUserWorkItem(Test_Slim, i);
            }

            Console.ReadLine();
        }

        private async static void Say()
        {
            var t = TestAsync();
            Thread.Sleep(1100);
            Console.WriteLine("Main Thread");
            Console.WriteLine(await t);
        }

        private async static Task<string> TestAsync()
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(1000);
                return "Hello World";
            });
        }

        private static void Test_lock()
        {
            // 线程锁会让多线程访问的时候，一次只允许一个线程进入
            lock (lockObj)
            {
                if (!IsComplete)
                {
                    // todo other
                    Thread.Sleep(500);
                    Console.WriteLine("Completed");
                    IsComplete = true;
                }
            }
        }

        private async static void Test_Slim(object i)
        {
            // 信号量会让同一块代码指定多个线程进入
            Console.WriteLine("Ready" + i);
            await slim.WaitAsync();
            Console.WriteLine("Start" + i);

            // todo other
            await Task.Delay(1000);

            Console.WriteLine("End" + i);
            slim.Release();
        }
    }
}
