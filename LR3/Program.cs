using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // Метод для демонстрації роботи з Thread - обробка даних
    static void ThreadMethod()
    {
        Console.WriteLine("Thread Method: Processing data started.");
        int[] data = new int[1000];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = i * 2; // Симуляція обробки даних
            if (i % 100 == 0)
                Console.WriteLine($"Thread Method: Processed {i} items.");
        }
        Console.WriteLine("Thread Method: Data processing completed.");
    }

    // Метод для демонстрації async-await - симуляція завантаження даних з мережі
    static async Task AsyncMethod1()
    {
        Console.WriteLine("Async Method 1: Starting network operation...");
        await Task.Delay(2000); // Симуляція затримки завантаження
        Console.WriteLine("Async Method 1: Data fetched from the network.");
    }

    // Метод для демонстрації async-await - швидке обчислення
    static async Task AsyncMethod2()
    {
        Console.WriteLine("Async Method 2: Performing quick calculation.");
        await Task.Yield(); // Швидка задача без затримки
        int result = 0;
        for (int i = 0; i < 100; i++)
        {
            result += i; // Проста операція обчислення
        }
        Console.WriteLine($"Async Method 2: Calculation result = {result}.");
    }

    static void Main(string[] args)
    {
        // Виконання методу з класом Thread
        Thread thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();

        // Виклик асинхронних методів
        Task task1 = AsyncMethod1();
        Task task2 = AsyncMethod2();

        // Очікування завершення асинхронних методів
        Task.WaitAll(task1, task2);

        Console.WriteLine("Main method completed.");
    }
}
