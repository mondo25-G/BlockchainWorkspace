using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BlockChain.Advanced.Library
{
    public static class Extensions
    {
       
        #region BlockChain extensions
        // 
        /// <summary>
        /// Checks the Validity of the entire blockChain (IN A SINGLE NETWORK NODE).
        /// </summary>
        /// <param name="blockChain">The blockchain to validate</param>
        /// <returns>true if valid blockchain false otherwise.</returns></returns>
        public static bool IsValid(this BlockChain blockChain, out string message)
        {

            for (int i = 1; i < blockChain.Chain.Count; i++)
            {
                Block currentBlock = blockChain.Chain[i];
                Block previousBlock = blockChain.Chain[i - 1];

                //Check if there is consistency between current and previous block.
                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    message = "tampered chain";
                    return false;
                }
            }
            message = "all clear";
            return true;
        }

        /// <summary>
        /// returns the entire blockChain in JSON
        /// </summary>
        /// <param name="blockChain">the blockchain to convert</param>
        /// <returns>the blockchain JSON</returns>
        public static string ToJson(this BlockChain blockChain)
        {           
            try
            {
                return JsonSerializer.Serialize(blockChain, BlockChain.JsonSerializerOptions);
            }
            catch (NullReferenceException e)
            {
                return e.Message;
            }
        }

       /// <summary>
       /// Returns a chain's block to Json given its chain index.
       /// </summary>
       /// <param name="blockChain">the blockchain</param>
       /// <param name="index">the block's index in the chain</param>
       /// <returns>the block in JSON format</returns>
        public static string BlockToJson(this BlockChain blockChain,int index)
        {            
            try
            {
                return JsonSerializer.Serialize(blockChain.Chain[index], BlockChain.JsonSerializerOptions);
            }
            catch (IndexOutOfRangeException e)
            {
                return e.Message;
            }
        }
        #endregion
    }
}
