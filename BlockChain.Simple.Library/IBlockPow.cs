using System;
using System.Collections.Generic;
using System.Text;

namespace BlockChain.Simple.Library
{
    public interface IBlockPow:IBlock
    {
        public void Mine(int difficulty);
    }
}
