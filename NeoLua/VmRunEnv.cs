using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.IronLua;
using NLog;

namespace WaykiContract
{
    public class VmRunEnv
    {
        //public List<>

        public Logger LuaLog = NLog.LogManager.GetLogger("LUA");
        public static Logger MainLog = NLog.LogManager.GetLogger("SYS");
        public NBitcoin.Network Network = null;
        public ContractContext ContractContext = null;

        public DBreezeDb ScriptData;
        public IAppData TokenTable;
        public IAppData AppDataTable;
        public IAppData MainTable;

        public static Lua GetLua()
        {
            var lua = new Lua();
            dynamic dg = lua.CreateEnvironment<WiccLuaGlobal>();
            return lua;
        }

        public void ExecuteContract(string luaScript, ContractContext context)
        {
            try
            {
                Initialize(context);

                var contractTable = new LuaTable();

                if (!string.IsNullOrEmpty(ContractContext.ContractHex))
                {
                    if (ContractContext.ContractHex.Length % 2 != 0)
                    {
                        throw new Exception("ContractHex lenght error");
                    }

                    ContractContext.ContractBytes = HexStringToByteArray(ContractContext.ContractHex);

                    foreach (var bit in ContractContext.ContractBytes)
                    {
                        contractTable.Add(bit);
                    }
                }

                using (Lua l = new Lua())
                {
                    var global = new WiccLuaGlobal(l, this) { ["contract"] = contractTable };

                    //dg.mylib = LuaType.GetType(typeof(ContractCore));
                    global.DoChunk(luaScript, "DEBUG");
                }
            }
            catch (Exception exception)
            {
                LuaLog.Error("[Error] " + exception.Message);
            }

            MainLog.Info("The script is executed");
        }

        private void Initialize(ContractContext context)
        {
            ContractContext = context;
            Network = NBitcoin.Wicc.Wicc.Instance.Testnet;
            ScriptData = DBreezeDb.CreateDatabase(context.ScriptId);
            AppDataTable = ScriptData.GetAppTable("AppDataTable");
            TokenTable = ScriptData.GetAppTable("TokenTable");
            MainTable = ScriptData.GetAppTable("MainTable");
        }

        private static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
            var buffer = new byte[s.Length / 2];
            for (var i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
    }
}
