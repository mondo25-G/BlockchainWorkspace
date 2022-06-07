using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BlockChain.Advanced.Library
{
    /// <summary>
    /// More rigorous implementation of a blockchain w.r.t. the ones in BlockChain.Library<br></br>
    /// where the following elements have been incorporated :<br></br>
    /// 1) Mining difficulty as a blockchain property<br></br>
    /// 2) Pending Transactions before their actual integration to the newly mined block<br></br>
    /// 2) Reward system for succesful mining (provide incentive to "participate in the chain")<br></br>
    /// Access modifiers are more "strict" here.<br></br>
    /// </summary>
    public class BlockChain
    {
        #region Fields

        //serializer options
        private static readonly JsonSerializerOptions _options;
        
        //let's define them as innate characteristic of the blockchain.
        private readonly int _powDifficulty;  //mining difficulty

        private readonly double _miningReward; // cryptocurrency.
        /// <summary>
        /// Dummy field to stress that all blockchain operations here take place in a single node. <br></br>
        /// This example does not simulate a more realistic scenario of a peer to peer network <br></br>
        /// where all nodes communicate with each other, specifically for validation of newly mined block by each and every node. <br></br> 
        /// </summary>
        private readonly bool _p2p;

        /// <summary>
        /// The new block generating process is a time-consuming process. <br></br>
        /// But, a transaction can be submitted anytime, <br></br>
        /// so we need to have a place to store transactions before they are processed. <br></br>
        /// Therefore, we added a new field, PendingTransactions, to store newly added transactions<br></br>
        /// </summary>
        private IList<Transaction> _pendingTransactions =  new List<Transaction>(); //composition, DRY for CreateGenesisBlock method

        private IList<Block> _chain =  new List<Block>(); //composition, should help in tests.
        #endregion

        #region Properties
        public static JsonSerializerOptions JsonSerializerOptions { get { return _options; } }
        public int PowDifficulty { get { return _powDifficulty; } }
        public double MiningReward { get { return _miningReward; } }

        /* I have to make PendingTransactions and the Chain properties IReadOnlyList<T>
         * Otherwise (i.e as IList<T>) I expose the blockchain to tampering via all extension methods that modify an IList<T>
         * 
         * Let's keep in mind, that , from my (limited) understanding of blockchain operations, the miners are indeed allowed
         * to modify the order of pending transactions for block formation, in order to mine the block faster, however that 
         * implies even more security considerations, I mean, you OBVIOUSLY need to check for foulplay between the pending transactions
         * before block integration, and the block's transactions after mining. That needs quite aggressive coding, and I'm not prepared to
         * do that here. So this class """feature""" is quietly ommitted.
         * 
         */
        public IReadOnlyList<Transaction> PendingTransactions { get { return (IReadOnlyList<Transaction>)_pendingTransactions; } }
        public IReadOnlyList<Block> Chain { get { return (IReadOnlyList<Block>)_chain; } }
        #endregion

        #region Constructors

        static BlockChain()
        {
            _options = new JsonSerializerOptions { WriteIndented = true };
        }

        /// <summary>
        /// Initializes Chain/adds Genesis Block,inititalizes pending transactions, sets pow difficulty/minining reward.
        /// </summary>
        public BlockChain(int proofOfWorkDifficulty, double miningReward, bool p2p = false)
        {
            _p2p = p2p;
            _powDifficulty = proofOfWorkDifficulty;
            _miningReward = miningReward;
            AddGenesisBlock();
        }
        #endregion

        #region Helper Methods 
        /// <summary>
        /// Creates the Genesis Block
        /// </summary>
        /// <returns>a Block </returns>
        private Block CreateGenesisBlock()
        {
            Block genesisBlock = new Block(DateTime.Now, _pendingTransactions);
            genesisBlock.Mine(PowDifficulty);
            _pendingTransactions = new List<Transaction>();
            return genesisBlock;
        }

        /// <summary>
        /// Adds Genesis Block to Chain.
        /// </summary>
        private void AddGenesisBlock()
        {
            _chain.Add(CreateGenesisBlock());
        }

        /// <summary>
        /// Returns latest Block.
        /// </summary>
        /// <returns></returns>
        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1] as Block;
        }

        #endregion

        #region Transactions/Balance
        /// <summary>
        /// Adds a (usual) transaction to blockchain pending transactions
        /// </summary>
        /// <param name="transaction">the transaction to add</param>
        public void AddTransaction(Transaction transaction) 
        {
            _pendingTransactions.Add(transaction);
        }
       
        /// <summary>
        /// Adds a transaction that rewards miner for successful block mining to list of pending transactions.
        /// </summary>
        /// <param name="minerAddress">the address of the miner</param>
        /// <remarks>Miner's balance updated after addintion of block</remarks>
        private void AddRewardTransaction(string minerAddress)
        {
            Transaction minerRewardTransaction = new Transaction(DateTime.Now,null, minerAddress, MiningReward);
            _pendingTransactions.Add(minerRewardTransaction);
        }

        /// <summary>
        /// Calculates each miner's balance in the blockchain, by iterating all transactions in a block,<br></br>
        /// for every block in the chain <br></br>. After succesful mining , the miner's balance will be increased by the reward.
        /// </summary>
        /// <param name="address">miner's address</param>
        /// <returns></returns>
        public double GetBalance(string address)
        {
            double balance = 0;
            foreach (Block block in Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (transaction.FromAddress == address)
                    {
                        balance -= transaction.Amount;
                    }
                    if (transaction.ToAddress == address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }
        #endregion

        #region The Actual Work
        /// <summary>
        /// Adds a new block to the end of chain
        /// </summary>
        /// <param name="block">The Block to add.</param>
        private void AddBlock(Block block)
        {
            //Mine it - get the current block's Hash.
            block.Mine(PowDifficulty);
            //Finish "decoration" of current block
            block.Index = GetLatestBlock().Index + 1;
            block.PreviousHash = GetLatestBlock().Hash;
            //Add it to the block chain.
            _chain.Add(block);
        }

        /// <summary>
        /// Processes all pending transactions.<br></br>
        /// Practically the miner mines the hash of the block that is formed by the above transactions
        /// and is awarded cryptocurrency upon integration of newly mined block to the chain 
        /// </summary>
        /// <param name="minerAddress">the miner</param>
        /// <remarks>Maybe the method name should change, but since it is one of the few public<br></br>
        /// methods of the class, I think this name succeeds in hiding every minute implementation detail <br></br>
        /// The caller only needs to know that he has to provide an address - then the new block is mined/added to chain/he gets reward<br></br>
        /// </remarks>
        public void ProcessPendingTransactions(string minerAddress)
        {
            //Create a transaction to reward miner. add it to pending transactions.
            AddRewardTransaction(minerAddress);
            //Instantiate a new block (but do not set its PreviousHash - Defer that to method AddBlock instead).
            Block block = new Block(DateTime.Now,_pendingTransactions);
            //Add it to the chain.
            AddBlock(block);
            //All pending transactions incorporated in newly created block. Initialize a new empty list of future pending trasactions.
            _pendingTransactions = new List<Transaction>();
        }
        #endregion

        #region Validity
        /// <summary>
        /// Checks the Validity of the entire blockChain (IN A SINGLE NETWORK NODE).
        /// </summary>
        /// <returns>true if valid blockchain false otherwise.</returns></returns>
        public bool IsValid(out string message)
        {

            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock =Chain[i - 1];

                //Check if there is consistency between current and previous block.
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    message = "Tampered chain";
                    return false;
                }
            }
            message = "Valid Chain";
            return true;
        }
        #endregion

        #region Useful.
        //future reference
        private string SuccessfulMining(Block block)
        {
            return $"Block {block.Index} with HASH={block.Hash} successfully mined.";
        }
        #endregion
    }
}
