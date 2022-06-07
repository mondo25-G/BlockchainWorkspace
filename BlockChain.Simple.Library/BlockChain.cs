using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BlockChain.Simple.Library
{
    /// <summary>
    /// Simple BlockChain comprised of blocks as descibed in Block.cs (i.e. 1 block= 1 transaction, no PoW taken into account.)
    /// </summary>
    public class BlockChain:IBlockChain
    {

        public IList<IBlock> Chain { private set; get; }
        /// <summary>
        /// Initializes Chain, adds Genesis Block.
        /// </summary>
        public BlockChain()
        {
            InitializeChain();
            AddGenesisBlock();
        }

        /// <summary>
        /// Initializes the chain
        /// </summary>
        private void InitializeChain()
        {
            Chain = new List<IBlock>();
        }
        /// <summary>
        /// Creates the Genesis Block
        /// </summary>
        /// <returns>an IBlock </returns>
        /// <remarks> overriden in derived class to return a Block (BlockPoW) that implements PoW</remarks>
        private protected virtual IBlock CreateGenesisBlock()
        {
            return new Block(DateTime.Now, null, "{}");
        }
        /// <summary>
        /// Adds Genesis Block to Chain.
        /// </summary>
        private void AddGenesisBlock()
        {
            Chain.Add(CreateGenesisBlock());
        }

        /// <summary>
        /// Returns latest Block.
        /// </summary>
        /// <returns></returns>
        public IBlock GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }

        /// <summary>
        /// Adds a block to the chain
        /// </summary>
        /// <param name="block">the IBlock to add</param>
        public void AddBlock(IBlock block)
        {
            DecorateBlock(block);
            Chain.Add(block);
        }

        /// <summary>
        /// Sets the properties of a block before adding to the chain.
        /// </summary>
        /// <param name="block">The IBlock to add.</param>
        /// <remarks>overriden in derived class to apply Mining/Difficulty (PoW) </remarks>
        private protected virtual void DecorateBlock(IBlock block)
        {
            IBlock latestBlock = GetLatestBlock();
            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;
            block.Hash = block.CalculateHash();

        }


        /// <summary>
        /// Checks the Validity of the entire blockChain.
        /// </summary>
        /// <returns>true if valid blockchain false otherwise.</returns>
        public virtual bool IsValid()
        {
            //Run through the (quite similar to but definetely not identical to) linked list and check its validity.
            for (int i = 1; i < Chain.Count; i++)
            {
                IBlock currentBlock = Chain[i];
                IBlock previousBlock = Chain[i - 1];

                //Check if current block has been tampered with. This makes sense when there is no mining involved
                
                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }
                //Check if there is consistency between current and previous block.
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
