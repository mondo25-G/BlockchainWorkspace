using System;
using System.Collections.Generic;

namespace BlockChain.Concole.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BlockChain Playground.\n");
            DashBoard();
        }


        static void DashBoard()
        {
            string choice;
            do
            {
                
                Console.WriteLine("To test simple blockchain library press 1.");
                Console.WriteLine("To test advanced blockchain library press 2.");
                Console.WriteLine("to run other press 3.");
                Console.WriteLine("To exit press 4."); ;
                choice = Console.ReadLine();

            } while (choice != "1" && choice != "2" && choice != "3" && choice != "4");

            if (choice == "1")
            {
                var testSimple = new ConsoleTestSimple();
                DashBoard();
            }
            if (choice == "2")
            {
                var testAdvanced = new ConsoleTestAdvanced();
                DashBoard();
            }
            if (choice == "3")
            {
                DashBoard();
            }
            if (choice == "4")
            {
                Environment.Exit(0);
            }
            
        }
    }
}
