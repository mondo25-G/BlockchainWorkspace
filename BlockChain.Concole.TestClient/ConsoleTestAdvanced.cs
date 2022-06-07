using BlockChain.Advanced.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockChain.Concole.TestClient
{
    public sealed class ConsoleTestAdvanced
    {
        private List<Transaction> _transactionsInPool;
        public ConsoleTestAdvanced()
        {
            _transactionsInPool = new List<Transaction>
            {
                new Transaction(DateTime.Now,"Joe","Bob",100),
                new Transaction(DateTime.Now,"Joe","Mike",200),
                new Transaction(DateTime.Now,"Jim","Joe",300),
                new Transaction(DateTime.Now,"Bob","Nick",100),
                new Transaction(DateTime.Now,"Balin","Dualin",2),
                new Transaction(DateTime.Now,"Jack","Bob",400),
                new Transaction(DateTime.Now,"Mary","Jack",200),
                new Transaction(DateTime.Now,"Nick","Bob",100),
                new Transaction(DateTime.Now,"Nick","Mary",600),
                
            };
            TestCase(_transactionsInPool);

        }
        /// <summary>
        /// Tests mine/reward/ system in an advanced blockchain, as well as participants balance.
        /// </summary>
        /// <param name="transactions">An IList of valid transactions</param>
        public static void TestCase(IList<Transaction> transactions)
        {
            string miningMessage;
            string validityMessage;
            Advanced.Library.BlockChain blockChain = new Advanced.Library.BlockChain(proofOfWorkDifficulty:3,miningReward:12.5d);
            Console.WriteLine(blockChain.ToJson());
            Console.WriteLine($"Balin's balance is :{ blockChain.GetBalance("Balin")}");
            Console.WriteLine($"Dualin's balance is :{ blockChain.GetBalance("Dualin")}\n");


            //Add pending transactions. Balin starts mining, meanwhile more pending transactions hit the blockchain.
            blockChain.AddPendingTransaction(transactions[0]);
            blockChain.AddPendingTransaction(transactions[1]);
            Console.WriteLine("Mining block...");
            blockChain.ProcessPendingTransactions("Balin",out miningMessage);
            blockChain.AddPendingTransaction(transactions[2]);
            blockChain.AddPendingTransaction(transactions[3]);
            Console.WriteLine(miningMessage);
            Console.WriteLine(blockChain.ToJson());
            blockChain.IsValid(out validityMessage);
            Console.WriteLine(validityMessage);
            Console.WriteLine($"Balin's balance is :{ blockChain.GetBalance("Balin")}");
            Console.WriteLine($"Dualin's balance is :{ blockChain.GetBalance("Dualin")}\n");

            //Add more pending transactions , among them a miner to miner transaction
            blockChain.AddPendingTransaction(transactions[4]);
            blockChain.AddPendingTransaction(transactions[5]); //Balin to Dualin transaction.
            blockChain.AddPendingTransaction(transactions[6]);
            Console.WriteLine(blockChain.ToJson());
            blockChain.IsValid(out validityMessage);
            Console.WriteLine($"Balin's balance is :{ blockChain.GetBalance("Balin")}");
            Console.WriteLine($"Dualin's balance is :{ blockChain.GetBalance("Dualin")}\n");

            //Now Dualin mines the block.
            Console.WriteLine("Mining block...");
            blockChain.ProcessPendingTransactions("Dualin", out miningMessage);
            Console.WriteLine(blockChain.ToJson());
            blockChain.IsValid(out validityMessage);
            Console.WriteLine($"Balin's balance is :{ blockChain.GetBalance("Balin")}");
            Console.WriteLine($"Dualin's balance is :{ blockChain.GetBalance("Dualin")}\n");

        }
    }
}
