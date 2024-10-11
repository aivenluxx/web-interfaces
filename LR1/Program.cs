using System;
using System.IO;

namespace ConsoleMenuApp
{
    class Program
    {
        static void Main()
        {
            bool showMenu = true;

            while (showMenu)
            {
                ShowMenu();
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        DisplayWordCount();
                        break;
                    case "2":
                        PerformMathOperation();
                        break;
                    case "q":
                        showMenu = false;
                        break;
                    default:
                        Console.WriteLine("Unknown option of menu. Please, try again :)");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("Choose option:");
            Console.WriteLine("1. Show amount of words in \"Lorem ipsum\"");
            Console.WriteLine("2. Do math operation and show result");
            Console.WriteLine("Q. Quit program");
        }

        static void DisplayWordCount()
        {
            string text = File.ReadAllText("lorem.txt");
            string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            int wordCount = words.Length;
            Console.WriteLine($"Amount of words in text \"Lorem ipsum\": {wordCount}");
        }

        static void PerformMathOperation()
        {
            Console.WriteLine("Enter math operation:");
            string expression = Console.ReadLine();

            try
            {
                double result = Convert.ToDouble(new System.Data.DataTable().Compute(expression, ""));
                Console.WriteLine("Result: " + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
