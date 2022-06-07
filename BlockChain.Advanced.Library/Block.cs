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
        #region Fields
        //Index of block in chain.
        private int _index;
        //Time of block creation (to have a history).
        private readonly DateTime _timeStamp;
        //"seed" number to mine the hash accounting for chain's difficulty.
        private long _nonce;
        //contains the hash of the previous block in the chain.
        private string _previousHash;
        //hash of the block calculated based on all the properties of the block (change detection).
        private string _hash;
        //block transaction data
        private IList<Transaction> _transactions;
        #endregion


        #region Properties
        //You can access the properties anywhere but only via the blockchain 
        //or if you decouple the block from the chain in another block instance
        public int Index { get { return _index; } internal set { _index = value; } }       
        public DateTime TimeStamp { get { return _timeStamp; } }        
        public long Nonce { get { return _nonce; } }      
        public string PreviousHash { get { return _previousHash; } internal set { _previousHash = value; } }       
        public string Hash { get { return _hash; } internal set { _hash = value; } }

        #region Transaction properties
        //What the library manipulates
        internal IList<Transaction> Transactions { get { return _transactions; } set { _transactions = value; } }
        //what the client sees
        public IReadOnlyList<Transaction> BlockTransactions { get { return (IReadOnlyList<Transaction>) Transactions; } }

        #endregion

        #endregion

        #region Constructor
        //you can never call the constructor outside this library,
        internal Block(DateTime timeStamp, IList<Transaction> transactions,string previousHash="")
        {
            Index = 0;
            _nonce = 0;
            _timeStamp = timeStamp;
            PreviousHash = previousHash;
            Transactions = transactions;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculates the Block's Hash.
        /// </summary>
        /// <returns>the Block's hash</returns>
        ///<remarks>If I could get it somehow to work for validation of a block given nonce/difficulty I'd make it public<br></br>
        /// as it currently stands, it's private.</remarks>
        private string CalculateHash()
        {
            //A very simple implementation.  
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
        internal void Mine(int PoWdifficulty)
        {
            var leadingZeros = new string('0', PoWdifficulty);
            while (Hash == null || Hash.Substring(0, PoWdifficulty) != leadingZeros)
            {
                _nonce++;
                Hash = CalculateHash();
            }           
        }
        #endregion

    }
}
