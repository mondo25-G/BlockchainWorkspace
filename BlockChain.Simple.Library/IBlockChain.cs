using System;
using System.Collections.Generic;
using System.Text;

namespace BlockChain.Simple.Library
{
    public interface IBlockChain
    {
        public IList<IBlock> Chain { get; }
        public IBlock GetLatestBlock();
        public void AddBlock(IBlock block);

        public bool IsValid();

    }
}
