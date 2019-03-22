using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WaykiContract
{
    public class ContractContext
    {
        public string ContractHex;
        [JsonIgnore]
        public byte[] ContractBytes;

        public int Fee;
        public int Amount;
        public int TxHeight;
        public string TxRaw;
        public string TxHash;
        public string TxAccount;
        public string FromAddress;
        public string FromRegId;
        public string ScriptId = "0-0";
        public string ScriptAddress;
        public string Signature;

        /// <summary>
        /// 确认高度
        /// </summary>
        public int ValidHeight;
    }
}
