using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Advanced.Library
{
    /// <summary>
    /// Block (representing a number of transactions) that participates in a blockchain,<br></br>
    /// with full proof of work implementations (Nonce/Mining with given PoW difficulty),<br></br>
    /// Access modifiers are more "strict" here.<br></br>
    /// </summary>
    public class Block
    {
        //Index of block in chain.
        private int _index;
        public int Index { get { return _index; } internal set { _index = value; } }

        //Time of block creation (to have a history).
        private readonly DateTime _timeStamp;
        public DateTime TimeStamp { get { return _timeStamp; } }
        //number of leading zeroes in hash.
        private long _nonce;
        public long Nonce { get { return _nonce; } }

        //contains the hash of the previous block in the chain.
        private string _previousHash;
        public string PreviousHash { get { return _previousHash; } internal set { _previousHash = value; } }

        //hash of the block calculated based on all the properties of the block (change detection).
        private string _hash;
        public string Hash { get { return _hash; } internal set { _hash = value; } }

        //Transactions Data 
        private IList<Transaction> _transactions;
        public IList<Transaction> Transactions { get { return _transactions; } internal set { _transactions = value; } }

        public Block(DateTime timeStamp, IList<Transaction> transactions,string previousHash="")
        {
            Index = 0;
            _nonce = 0;
            _timeStamp = timeStamp;
            PreviousHash = previousHash;
            Transactions = transactions;
        }
        /// <summary>
        /// Calculates the Block's Hash.
        /// </summary>
        /// <returns>the Block's hash</returns>
        private string CalculateHash()
        {
            using (SHA256 sha256 = SHA256.Create()) //define a scope.
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash ?? ""}-{JsonSerializer.Serialize(Transactions)}--{Nonce}");
                byte[] outputBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(outputBytes);
            }            
        }

        /// <summary>
        /// Mine method tries to find a hash that matches with difficulty.<br></br>
        /// If a generated hash doesn’t meet the difficulty, then it increases nonce to generate a new one.<br></br>
        /// The process will be ended when a qualified hash is found.<br></br>
        /// </summary>
        /// <param name="PoWdifficulty">an integer that indicates the number of leading zeros required for a generated hash.</param>
        /// <remarks>This is the work that takes place in the block</remarks>
        public void Mine(int PoWdifficulty)
        {
            var leadingZeros = new string('0', PoWdifficulty);
            while (Hash == null || Hash.Substring(0, PoWdifficulty) != leadingZeros)
            {
                _nonce++;
                Hash = CalculateHash();
            }
            
        }

    }
}
