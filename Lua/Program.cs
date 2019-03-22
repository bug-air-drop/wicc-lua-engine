using NBitcoin;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using WaykiContract;

namespace Lua
{
    class Program
    {
        static void Main(string[] args)
        {
            // Step 1.Create configuration object

            //var scriptAddress = new BitcoinPubKeyAddress("wgsGSfuznCXjfGTyo7d14eznNFX5BqZEws", NBitcoin.Wicc.Wicc.Instance.Testnet);

            //var ss = scriptAddress.ScriptPubKey.WitHash.ScriptPubKey.ToBytes();


            LoggingConfiguration config = new LoggingConfiguration();


            // Step 2. Create targets and add them to the configuration 

            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            // Step 3. Set target properties 

            consoleTarget.Layout = "${date:format=HH:MM:ss} ${logger} ${message}";

            // Step 4. Define rules 

            LoggingRule rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            // Step 5. Activate the configuration 

            LogManager.Configuration = config;

            var env = new WaykiContract.VmRunEnv();

            env.ExecuteContract(File.ReadAllText("d:\\lua.txt"), new WaykiContract.ContractContext() { TxHeight = 123 });
        }
    }
}
