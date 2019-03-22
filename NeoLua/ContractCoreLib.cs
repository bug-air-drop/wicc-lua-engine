using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Neo.IronLua;
using WaykiContract.Enums;
using DBNull = System.DBNull;

namespace WaykiContract
{
    /// <summary>
    /// LUA环境合约相关API
    /// </summary>
    public class ContractCoreLib
    {
        VmRunEnv vmRunEnv = null;

        public ContractCoreLib(VmRunEnv env)
        {
            vmRunEnv = env;
        }

        public void LogPrint(dynamic log)
        {
            vmRunEnv.LuaLog.Info(log.value);
        }

        public long Int64Mul(long a, long b)
        {
            return a * b;
        }

        public long Int64Add(long a, long b)
        {
            return a + b;
        }

        public long Int64Sub(long a, long b)
        {
            return a - b;
        }

        public long Int64Div(long a, long b)
        {
            return a / b;
        }

        public LuaResult Sha256(string strData)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(strData));
            bytes = sha256.ComputeHash(bytes);

            return ToLuaResult(bytes);
        }

        public LuaResult Sha256Once(string strData)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(strData));

            return ToLuaResult(bytes);
        }

        public LuaResult Des(dynamic t)
        {
            if (t == null)
                throw new ArgumentNullException("#1");

            //LuaTable lt = (LuaTable)t;

            int dataLen = t.dataLen;
            LuaTable data = (LuaTable)t.data;
            int keyLen = t.keyLen;
            LuaTable key = (LuaTable)t.key;
            int flag = t.flag;

            byte[] inputByteArray = Int32Bytes(data).Take(dataLen).ToArray();

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Int32Bytes(key).Take(keyLen).ToArray();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;

            MemoryStream ms = new MemoryStream();

            if (flag == 1)
            {
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                if (dataLen <= 8)
                {
                    var outBytes = ms.ToArray();
                    return ToLuaResult(outBytes.Take<byte>(8).ToArray());
                }
            }
            else
            {
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
            }

            return ToLuaResult(ms.ToArray());
        }

        public void VerifySignature()
        {

        }

        public LuaResult GetTxContract(params object[] bytes)
        {
            if (bytes == null || bytes.Length != 32)
            {
                throw new Exception("ExGetTxContractFunc, para error");
            }

            //TODO 查询交易

            var contract = new byte[0];

            return ToLuaResult(contract);
        }

        public LuaResult GetTxRegID(params object[] bytes)
        {
            if (bytes == null || bytes.Length != 32)
            {
                throw new Exception("ExGetTxRegIDFunc, para error");
            }

            //TODO 获取链上交易
            var regid = "0-0";

            var outBytes = new List<byte>();
            outBytes.AddRange(BitConverter.GetBytes(int.Parse(regid.Split('-')[0])));
            outBytes.AddRange(BitConverter.GetBytes(int.Parse(regid.Split('-')[1])));

            return ToLuaResult(outBytes.ToArray());
        }

        public LuaResult GetAccountPublickey(params object[] hexs)
        {
            string address = GetAddress(hexs);

            //TODO 获取地址的公钥  33位
            var pk = vmRunEnv.MainTable.Select(address + "-PK");

            return ToLuaResult(Encoding.UTF8.GetBytes(pk));
        }

        public LuaResult QueryAccountBalance(params object[] hexs)
        {
            string address = GetAddress(hexs);

            var nowBalance = vmRunEnv.MainTable.SelectInt(address);
            return ToLuaResult(BitConverter.GetBytes(nowBalance));
        }

        /// <summary>
        /// 获取交易的确认高度
        /// </summary>
        /// <param name="hexs">交易哈希</param>
        /// <returns></returns>
        public long GetTxConfirmHeight(params object[] hexs)
        {
            if (hexs == null || hexs.Length != 32)
            {
                throw new Exception("ExGetTxConfirmHeightFunc para err1");
            }


            return 0;
        }

        public LuaResult GetBlockHash(int index)
        {
            if (index < 0 && index > vmRunEnv.ContractContext.ValidHeight)
            {
                throw new Exception("ExGetBlockHashFunc para err2");
            }

            //TODO 从链上取

            var pk = vmRunEnv.MainTable.Select(index + "-BLOCK");

            return ToLuaResult(Encoding.UTF8.GetBytes(pk));
        }

        /// <summary>
        /// 获取当前交易的hash
        /// </summary>
        /// <returns></returns>
        public LuaResult GetCurTxHash()
        {
            var table = new LuaResult(new uint256(vmRunEnv.ContractContext.TxHash));
            return table;
        }

        /// <summary>
        /// 获取当前合约运行的高度
        /// </summary>
        /// <returns></returns>
        public long GetCurRunEnvHeight()
        {
            //var bytes = BitConverter.GetBytes(DataPool.ContractContext.CurHeight);
            return vmRunEnv.ContractContext.ValidHeight;
        }

        /// <summary>
        /// 获取指定合约ID下的Key值
        /// local value={mylib.GetContractData({id=t,key='name'})}
        /// </summary>
        /// <returns></returns>
        public LuaResult GetContractData()
        {
            //TODO
            return null;
        }

        public bool WriteData(dynamic dTable)
        {
            var valueBytes = TableToList<byte>((LuaTable)dTable.value);

            vmRunEnv.AppDataTable.InsertBytes(dTable.key, valueBytes.ToArray());

            return true;
        }

        public bool DeleteData(string key)
        {
            vmRunEnv.AppDataTable.Delete(key);

            return true;
        }

        public LuaResult ReadData(string key)
        {
            var keyValue = vmRunEnv.AppDataTable.SelectBytes(key);

            if (keyValue == null || keyValue.Length == 0)
            {
                return LuaResult.Empty;
            }
            else
            {
                return ToLuaResult(keyValue);
            }
        }

        public bool ModifyData(dynamic dTable)
        {
            var valueBytes = TableToList<byte>((LuaTable)dTable.value);
            vmRunEnv.AppDataTable.InsertBytes(dTable.key, valueBytes.ToArray());

            return true;
        }

        public bool WriteOutput(dynamic dTable)
        {
            var addrType = (int)dTable.addrType;
            var operatorType = (int)dTable.operatorType;
            var outHeight = (int)dTable.outHeight;

            var money = ByteToInteger(TableToList<byte>((LuaTable)dTable.moneyTbl).ToArray());

            var address = string.Empty;

            if (addrType == (int)ADDR_TYPE.REGID)
            {
                var accountIdTbl = TableToList<byte>((LuaTable)dTable.accountIdTbl);

                var idBytes = accountIdTbl.Take(4);
                var blockId = BitConverter.ToInt32(idBytes.ToArray(), 0);
                var subId = BitConverter.ToInt16(accountIdTbl.Skip(4).ToArray(), 0);

                var regId = blockId + "-" + subId;
                address = vmRunEnv.MainTable.Select(regId);
            }

            if (operatorType == (int)OPER_TYPE.ENUM_ADD_FREE)
            {
                var nowBalance = vmRunEnv.MainTable.SelectInt(address);
                nowBalance += money;

                vmRunEnv.MainTable.InsertInt(address, nowBalance);
                vmRunEnv.LuaLog.Info($"[System_Add] balance changed {address} -> {(nowBalance / 100000000):f8}");
            }
            else if (operatorType == (int)OPER_TYPE.ENUM_MINUS_FREE)
            {
                var nowBalance = vmRunEnv.MainTable.SelectInt(address);

                if (nowBalance < money)
                {
                    return false;
                }

                nowBalance -= money;
                nowBalance = nowBalance < 0 ? 0 : nowBalance;

                vmRunEnv.MainTable.InsertInt(address, nowBalance);
                vmRunEnv.LuaLog.Info($"[System_Sub] balance changed {address} -> {(nowBalance / 100000000):f8}");
            }
            else
            {
                throw new Exception("WriteOutput addrType error");
            }

            return true;
        }

        public LuaResult GetContractRegId()
        {
            if (string.IsNullOrEmpty(vmRunEnv.ContractContext.ScriptId))
            {
                return LuaResult.Empty;
            }

            var outBytes = new List<byte>();
            outBytes.AddRange(BitConverter.GetBytes(int.Parse(vmRunEnv.ContractContext.ScriptId.Split('-')[0])));
            outBytes.AddRange(BitConverter.GetBytes(int.Parse(vmRunEnv.ContractContext.ScriptId.Split('-')[1])));

            return ToLuaResult(outBytes.ToArray());
        }

        public LuaResult GetCurTxAccount()
        {
            if (string.IsNullOrEmpty(vmRunEnv.ContractContext.FromRegId))
            {
                return LuaResult.Empty;
            }

            var outBytes = new List<byte>();
            outBytes.AddRange(BitConverter.GetBytes(int.Parse(vmRunEnv.ContractContext.FromRegId.Split('-')[0])));
            outBytes.AddRange(BitConverter.GetBytes(int.Parse(vmRunEnv.ContractContext.FromRegId.Split('-')[1])));

            return ToLuaResult(outBytes.ToArray());
        }

        public LuaResult GetCurTxPayAmount()
        {
            return ToLuaResult(BitConverter.GetBytes(vmRunEnv.ContractContext.Amount));
        }

        public LuaResult GetUserAppAccValue(dynamic idTbl)
        {
            var idValueTbl = TableToList<char>((LuaTable)idTbl.idValueTbl);
            var idValue = string.Join(string.Empty, idValueTbl);
            if (idValue.Contains("-"))
            {
                idValue = vmRunEnv.TokenTable.Select(idValue);
            }

            var nowBalance = vmRunEnv.TokenTable.SelectInt(idValue);

            return ToLuaResult(BitConverter.GetBytes(nowBalance));
        }

        public void GetUserAppAccFoudWithTag()
        {

        }

        public bool WriteOutAppOperate(dynamic dTable)
        {
            var operatorType = (int)dTable.operatorType;
            var outHeight = (int)dTable.outHeight;
            var userIdTb = TableToList<char>((LuaTable)dTable.userIdTbl);
            var userAddress = string.Join(string.Empty, userIdTb);
            var money = ByteToInteger(TableToList<byte>((LuaTable)dTable.moneyTbl).ToArray());

            //money = money / 1_0000_0000;

            if (operatorType == (int)APP_OPERATOR_TYPE.ENUM_ADD_FREE_OP)
            {
                var nowBalance = vmRunEnv.TokenTable.SelectInt(userAddress);

                nowBalance += money;

                vmRunEnv.TokenTable.InsertInt(userAddress, nowBalance);
                vmRunEnv.LuaLog.Info($"[App_Add] balance changed {userAddress} -> {(nowBalance / 100000000):f8}");
            }
            else if (operatorType == (int)APP_OPERATOR_TYPE.ENUM_SUB_FREE_OP)
            {
                var nowBalance = vmRunEnv.TokenTable.SelectInt(userAddress);

                if (nowBalance < money)
                {
                    return false;
                }

                nowBalance -= money;
                nowBalance = nowBalance < 0 ? 0 : nowBalance;

                vmRunEnv.TokenTable.InsertInt(userAddress, nowBalance);
                vmRunEnv.LuaLog.Info($"[App_Sub] balance changed {userAddress} -> {(nowBalance / 100000000):f8}");
            }
            else if (operatorType == (int)APP_OPERATOR_TYPE.ENUM_ADD_FREEZED_OP)
            {
                FreezUnlock(userAddress);

                var freez = vmRunEnv.TokenTable.SelectListLong(userAddress + "-F");
                var newFreez = freez != null ? freez.ToList() : new List<long>();

                newFreez.Add(outHeight);
                newFreez.Add(money);

                vmRunEnv.TokenTable.InsertListLong(userAddress + "-F", newFreez.ToArray());
                vmRunEnv.LuaLog.Info($"[App_Lock] {(money / 100000000):f8} will be unlocked at {outHeight}");
            }
            else
            {
                throw new Exception("WriteOutAppOperate type error");
            }

            return true;
        }

        public LuaResult GetBase58Addr(params byte[] bytes)
        {
            if (bytes == null || bytes.Length != 6)
            {
                throw new Exception("GetBase58Addr para err1");
            }

            //临时转换方案

            var idBytes = bytes.Take(4);
            var blockId = BitConverter.ToInt32(idBytes.ToArray(), 0);
            var subId = BitConverter.ToInt16(bytes.Skip(4).ToArray(), 0);

            var regId = blockId + "-" + subId;
            var address = vmRunEnv.MainTable.Select(regId);

            if (!string.IsNullOrEmpty(address))
            {
                return ToLuaResult(Encoding.UTF8.GetBytes(address));
            }
            else
            {
                return LuaResult.Empty;
            }
        }

        public long ByteToInteger(params byte[] bytes)
        {
            if (bytes == null || (bytes.Length != 1 && bytes.Length != 4 && bytes.Length != 8))
            {
                throw new Exception("ByteToInteger para err1");
            }

            var bytelst = bytes.ToList();

            if (bytes.Length == 4)
            {
                return BitConverter.ToInt32(bytelst.ToArray(), 0);
            }
            else
            {
                return BitConverter.ToInt64(bytelst.ToArray(), 0);
            }
        }

        public LuaResult IntegerToByte4(int value)
        {
            return ToLuaResult(BitConverter.GetBytes(value));
        }

        public LuaResult IntegerToByte8(long value)
        {
            return ToLuaResult(BitConverter.GetBytes(value));
        }

        public void TransferContactAsset()
        {

        }

        public void TransferSomeAsset()
        {

        }

        public long GetBlockTimestamp(long index)
        {


            return 0;
        }

        public void DebugTable(LuaType table)
        {

        }

        public void DebugArgs(params object[] args)
        {

        }

        private static List<T> TableToList<T>(LuaTable table)
        {
            var valueList = new List<T>();

            foreach (var bit in table.ArrayList)
            {
                valueList.Add((T)bit);
            }

            return valueList;
        }

        private static List<byte> Int32Bytes(LuaTable table)
        {
            var valueList = new List<byte>();

            foreach (Int32 bit in table.ArrayList)
            {
                valueList.Add(System.BitConverter.GetBytes(bit)[0]);
            }

            return valueList;
        }

        /// <summary>
        /// 检查冻结账户
        /// </summary>
        /// <param name="address"></param>
        public void FreezUnlock(string address)
        {
            var freez = vmRunEnv.TokenTable.SelectListLong(address + "-F");
            var newFreez = new List<long>();

            for (int i = 0; freez != null && i < freez.Length; i = i + 2)
            {
                if (freez[i] <= vmRunEnv.ContractContext.ValidHeight)
                {
                    var nowBalance = vmRunEnv.TokenTable.SelectInt(address);
                    nowBalance += freez[i + 1];
                    vmRunEnv.TokenTable.InsertInt(address, nowBalance);
                    vmRunEnv.LuaLog.Info($"APP asset unlock at -> height:{freez[i]} value:{freez[i + 1]}");
                }
                else
                {
                    newFreez.Add(freez[i]);
                    newFreez.Add(freez[i + 1]);
                }
            }

            vmRunEnv.TokenTable.InsertListLong(address + "-F", newFreez.ToArray());
        }

        /// <summary>
        /// 将一个byte数组展开为多个返回结果的LuaResult
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static LuaResult ToLuaResult(byte[] bytes)
        {
            var r = new object[bytes.Length];
            for (var a = 0; a < bytes.Length; a++)
                r[a] = (int)bytes[a];

            return r;
        }

        private string GetAddress(params object[] hexs)
        {
            if (hexs == null || (hexs.Length != 6 && hexs.Length != 34))
            {
                throw new Exception("para err1");
            }

            string address;

            if (hexs.Length == 6)
            {
                var idBytes = hexs.Select(x => (byte)x).ToList();
                var blockId = BitConverter.ToInt32(idBytes.Take(4).ToArray(), 0);
                var subId = BitConverter.ToInt16(idBytes.Skip(4).ToArray(), 0);

                var regId = blockId + "-" + subId;
                address = vmRunEnv.MainTable.Select(regId);
            }
            else
            {
                address = Encoding.UTF8.GetString(hexs.Select(x => (byte)x).ToArray());
            }

            return address;
        }
    }
}
