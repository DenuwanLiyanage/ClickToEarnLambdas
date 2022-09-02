using System.Collections.Generic;
using WavesCS;

namespace EconomyLambda
{
    public class WavesGateway 
    {
        private readonly Node _node = new ();

        public void GetTransactionsTest()
        {
            var limit = 100;
            var address = "3MtzdBzSqCNrx169VpuVR5KYrpNPCpNTom7";

            var transactions = _node.GetTransactions(address, limit);

        }

        public Transaction GetTransaction(string txId)
        {
            return _node.GetTransactionById(txId);
        }
        
        public decimal GetAddressWavesBalance(string address)
        {
            return _node.GetBalance(address);
        }
        
        public decimal GetAddressGivenAssetBalance(string address,Asset asset)
        {
            return _node.GetBalance(address, asset);
        }

        public Dictionary<string,object> GetAddressData(string address)
        {
            return  _node.GetAddressData(address);
        }
    }
}