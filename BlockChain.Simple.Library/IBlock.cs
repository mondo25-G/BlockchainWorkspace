﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlockChain.Simple.Library
{
    public interface IBlock
    {
        public int Index { get; set; }
        public int Nonce { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public string Data { get; set; }
        public string CalculateHash();
    }
}
