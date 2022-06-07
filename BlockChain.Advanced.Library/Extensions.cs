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
        #endregion

        #region Block extensions
        /// <summary>
        /// Returns a chain's block to Json given its chain index.
        /// </summary>
        /// <param name="blockChain">the blockchain</param>
        /// <param name="index">the block's index in the chain</param>
        /// <returns>the block in JSON format</returns>
        /// <remarks>This method could prove useful to check the validity of a block in the future.</remarks>
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
