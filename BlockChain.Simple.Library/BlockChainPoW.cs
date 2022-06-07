using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BlockChain.Simple.Library
{
    /// <summary>
    /// Simple BlockChain comprised of blocks as descibed in BlockPoW.cs(i.e. 1 block= 1 transaction, PoW taken into account.)
    /// Access modifiers are "lax" here. Emphasis is on blockchain concepts
    /// </summary>   
    public class BlockChainPoW : BlockChain
    {
        public int Difficulty { set; get; } = 2; //Increase difficulty i.e. number of leading zeroes in hash
                                                 //and see the cost of mining firsthand.
        private protected override IBlock CreateGenesisBlock()
        {
            return new BlockPoW(DateTime.Now, null, "{}",Difficulty);
        }

        private protected override void DecorateBlock(IBlock block)
        {
            IBlock latestBlock = GetLatestBlock();
            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;
            IBlockPow blockPoW = (IBlockPow)block; //Minimal performance hit due to cast. This is simply a result of my approach 
            blockPoW.Mine(Difficulty);             //to create two examples (for learning purposes) :one for proof of work and one without it.
        }

        /// <summary>
        /// Checks the Validity of the entire blockChain in ONE NODE.
        /// </summary>
        /// <returns>true if valid blockchain false otherwise.</returns>
        public override bool IsValid()
        {
            //Run through the (quite similar to but definetely not identical to) linked list and check its validity.
            for (int i = 1; i < Chain.Count; i++)
            {
                IBlock currentBlock = Chain[i];
                IBlock previousBlock = Chain[i - 1];

                //When mining/nonce is involved they must have some other means of recalculating the hash to check its
                //validity (that I am not aware of). 
                //a) Recalculating the hash via the base method CalculateHash() does not 
                //   give correct results since the nonce is involved in the bruteforce procedure to mine the valid hash.
                //b) They can't possibly RE-mine the hash to verify it. So obviously I am missing something here.
                //So this method remains incomplete, and in a more advanced implementation I will restrict access to all
                //security risk properties/methods so that the following condition is enough to verify the validity of a chain
                //in one node of the P2P network.

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
