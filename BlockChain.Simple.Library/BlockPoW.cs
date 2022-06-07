using System;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Simple.Library
{
    /// <summary>
    /// Simple Block that takes into account a single transaction (Data) and incorporates Proof of Work (Nonce,Mining).
    /// Access modifiers are "lax" here. Emphasis is on blockchain concepts
    /// </summary>
    public class BlockPoW : Block, IBlockPow
    {
        /*Proof of Work is a scheme used in generating a new block for a blockchain. 
         * It requires a significant amount of work, a.k.a. computing time, to generate a piece of information for a new block.
         * The generated information must be simple and verifiable. 
         * Therefore, it can be easily verified by any nodes in the network. T
         * he generated information is proof that this block is valid and some work has been done to generate it. 
         * The amount of work required for generating a new block is determined 
         * from the average computing time on generating a new block in the entire Blockchain network. 
         * The algorithm picked to implement Proof of Work scheme is a hash algorithm. 
         * The most widely used hash algorithm in Proof of Work is SHA-256. 


        /*To generate a hash that has a specific format, like a number of leading zeros, 
         * is very computing intensive and time-consuming.There is no shortcut for the generating process.
         * However, verifying the source data that matches with the generated hash is trivial.
         * Because a specific piece of data can only get a specific hash, 
         * the source data must be changed to generate a different hash.
         * This is solved by introducing "NONCE" in the data structure. (number only once)
         * The nonce is an integer. By increasing the nonce, the hash algorithm can generate a different hash. 
         * This process will be ended until the generated hash meets the requirement, we call it DIFFICULTY.
         */

        public BlockPoW(DateTime timeStamp, string previousHash, string data, int difficulty) : base(timeStamp, previousHash, data)
        {
            Nonce = 0;
            Mine(difficulty);
        }
        public override string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();
            //Note that here we have implemented the first step of PoW via the Nonce
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash ?? ""}-{Data}-{Nonce}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        // The (full) implementation of Proof of Work is called mining.

        /// <summary>
        /// Mine method tries to find a hash that matches with difficulty.<br></br>
        /// If a generated hash doesn’t meet the difficulty, then it increases nonce to generate a new one.<br></br>
        /// The process will be ended when a qualified hash is found.<br></br>
        /// </summary>
        /// <param name="difficulty">an integer that indicates the number of leading zeros required for a generated hash.</param>
        public void Mine(int difficulty)
        {
            var leadingZeros = new string('0', difficulty);
            while (Hash == null || Hash.Substring(0, difficulty) != leadingZeros)
            {
                Nonce++;
                Hash = CalculateHash();
            }
        }
    }
}
