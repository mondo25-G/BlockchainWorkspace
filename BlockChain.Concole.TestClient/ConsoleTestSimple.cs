using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using BlockChain.Simple.Library;

namespace BlockChain.Concole.TestClient
{
    public sealed class ConsoleTestSimple
    {
        public ConsoleTestSimple()
        {
            var outputData = new Dictionary<string, string>(2);
            TestCases<Simple.Library.BlockChain>(ref outputData);
            TestCases<BlockChainPoW>(ref outputData);
            foreach (var kvp in outputData)
            {
                Console.WriteLine(kvp);
            }

            Console.WriteLine("\n---------TAMPERING--------------\n");

            HackBlockChain();
            
        }
        /// <summary>
        /// Tests two cases of valid blockchains, one without proof of work and one with, and illustrates the
        /// cost (in time) of block mining in the latter case.
        /// </summary>
        /// <typeparam name="T">an IBlockChain</typeparam>
        /// <param name="outputData">the performance data</param>
        public static void TestCases<T>(ref Dictionary<string,string> outputData) where T : IBlockChain, new()
        { 
            T blockchain = new T();
            Type blockchainType = typeof(T);
            var startTime = DateTime.Now;

            if (blockchainType.Equals(typeof(Simple.Library.BlockChain)))
            {
                Console.WriteLine("Calculate amount of time spent adding blocks, NO Proof of Work");
                blockchain.AddBlock(new Block(DateTime.Now, null, "{sender:Jeff,receiver:Walter,amount:10}"));
                blockchain.AddBlock(new Block(DateTime.Now, null, "{sender:Walter,receiver:Jeff,amount:5}"));
                blockchain.AddBlock(new Block(DateTime.Now, null, "{sender:Walter,receiver:Jeff,amount:5}"));
                
            }

            if (blockchainType.Equals(typeof(BlockChainPoW)))
            {
                Console.WriteLine("Calculate amount of time spent adding blocks, WITH Proof of Work");
                //I know this is problematic (allowing the client to set the difficulty and mess things up but this is not
                //a strict implementation. it is meant to have faults/security risks and such, so that we can see and learn. 
                blockchain.AddBlock(new BlockPoW(DateTime.Now, null, "{sender:Jeff,receiver:Walter,amount:10}",2)); 
                blockchain.AddBlock(new BlockPoW(DateTime.Now, null, "{sender:Walter,receiver:Jeff,amount:5}",2));
                blockchain.AddBlock(new BlockPoW(DateTime.Now, null, "{sender:Walter,receiver:Jeff,amount:5}",2));
            }
           
            var endTime = DateTime.Now;
            outputData.Add($"{blockchainType.Name}:   ", $"Duration: {endTime - startTime}");
            Console.WriteLine(JsonSerializer.Serialize(blockchain, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"Is {blockchainType.Name} valid?:   {blockchain.IsValid()}");

        }
        /// <summary>
        /// Illustrates the hacking of a simple blockchain without POW, and the vulnerability of a centralized system.
        /// </summary>
        public static void HackBlockChain()
        {
            Simple.Library.BlockChain blockchain = new Simple.Library.BlockChain();
            blockchain.AddBlock(new Block(DateTime.Now, null, "{sender:Jeff,receiver:Walter,amount:10}"));
            blockchain.AddBlock(new Block(DateTime.Now, null, "{sender:Walter,receiver:Jeff,amount:5}"));
            blockchain.AddBlock(new Block(DateTime.Now, null, "{sender:Walter,receiver:Jeff,amount:5}"));
            Console.WriteLine("\nValidity tests.\n");

            Console.WriteLine($"No tampering.");
            Console.WriteLine($"Is Chain Valid: {blockchain.IsValid()}\n");

            Console.WriteLine($"Attacker picks block.");
            Console.WriteLine(JsonSerializer.Serialize(blockchain.Chain[1], new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"Attacker tampers data of a block.");
            blockchain.Chain[1].Data = "{sender:Jeff,receiver:Wallter,amount:1000}";
            Console.WriteLine(JsonSerializer.Serialize(blockchain.Chain[1], new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"Is Chain Valid: {blockchain.IsValid()}\n");

            Console.WriteLine($"Attacker recalculates hash of the block");
            blockchain.Chain[1].Hash = blockchain.Chain[1].CalculateHash();
            Console.WriteLine(JsonSerializer.Serialize(blockchain.Chain[1], new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"Is Chain Valid: {blockchain.IsValid()}\n");

            Console.WriteLine($"Attacker updates entire chain");
            blockchain.Chain[2].PreviousHash = blockchain.Chain[1].Hash;
            blockchain.Chain[2].Hash = blockchain.Chain[2].CalculateHash();
            blockchain.Chain[3].PreviousHash = blockchain.Chain[2].Hash;
            blockchain.Chain[3].Hash = blockchain.Chain[3].CalculateHash();
            Console.WriteLine(JsonSerializer.Serialize(blockchain, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"Is Chain Valid: {blockchain.IsValid()}");

            Console.WriteLine("This is only passed on one node because Blockchain is a decentralized system.\n" +
                "Tampering with one node could be easy but tampering with all the nodes in the system is impossible.\n");

            Console.WriteLine("The situation could become worse in the real world because re-calculation can be done \n" +
                "in a short period of time with a modern computer. We must come up with a solution to stop attackers \n" +
                "from tampering with a blockchain. \nThe solution is the so called prrof of work. \n");

        }
    }

    
}
