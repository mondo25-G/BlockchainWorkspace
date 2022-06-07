
using System;

namespace BlockChain.Advanced.Library
{
    ///<summary>
    /// Transaction to participate in the blockchain. <br></br>
    /// All transactions in this implementation assumed to be valid (i.e signature exists )
    ///</summary>
    public class Transaction
    { 
        public DateTime TimeStamp { get; }
        public string FromAddress { get; }
        public string ToAddress { get; }
        public double Amount { get; }

        public Transaction(DateTime timeStamp,string fromAddress, string toAddress, double amount)
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Amount = amount;
            TimeStamp = timeStamp;            
        }
        public override string ToString()
        {
            return $"From {FromAddress} To {ToAddress}: Amount={Amount}"; 
        }
    }
}
