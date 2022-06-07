using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BlockChain.Simple.Library
{
    /// <summary>
    /// Simple Block that takes into account a single transaction (Data) and ignores Proof of Work. 
    /// </summary>
    public class Block : IBlock
    {
        //Index of block in chain.
        public int Index { get; set; }

        //Ignore nonce for now.
        public int Nonce { get; set; } = 0;

        //Time of block creation (to have a history).
        public DateTime TimeStamp { get; set; }

        //contains the hash of the previous block in the chain.
        public string PreviousHash { get; set; }
        //hash of the block calculated based on all the properties of the block (change detection).
        public string Hash { get; set; }

        //Transaction Data
        //For simplicity assume only one transaction here whose Data we serialize
        public string Data { get; set; }

        //In the constructor of the class, we initialize all the properties 
        //and using the CreateHash method we calculate the hash of the block 
        //based on all properties of the block. This way, if anything will 
        //change inside the block, it will have a different hash, so a change can be easily detected.
        public Block(DateTime timeStamp, string previousHash, string data)
        {
            Index = 0;
            TimeStamp = timeStamp;
            PreviousHash = previousHash;
            Data = data;
            Hash = CalculateHash();
        }
        /// <summary>
        /// Calculates the Block's Hash.
        /// </summary>
        /// <returns>the Block's hash</returns>
        /// <remarks>Can be overriden in derived class (BlockPoW.cs) to incorporate Nonce into hashing.</remarks>
        public virtual string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();
            //Note: Null coalescing operator takes care of genesis block hash
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash ?? ""}-{Data}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }
    }
}
