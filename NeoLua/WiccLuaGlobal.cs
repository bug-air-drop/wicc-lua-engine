using System;
using System.Collections.Generic;
using System.IO;
using Neo.IronLua;

namespace WaykiContract
{
    #region -- class LuaGlobal ----------------------------------------------------------

    ///////////////////////////////////////////////////////////////////////////////
    /// <summary></summary>
    public class WiccLuaGlobal : LuaGlobal
    {
        public VmRunEnv VmRunEnv = null;
        private ContractCoreLib mylib = null;

        #region -- Ctor/Dtor --------------------------------------------------------------

        /// <summary>Create a new environment for the lua script manager.</summary>
        /// <param name="lua"></param>
        public WiccLuaGlobal(Lua lua, VmRunEnv env)
            : base(lua)
        {
            VmRunEnv = env;
        } // ctor

        #endregion

        #region -- Basic Functions --------------------------------------------------------

        /// <summary></summary>
        /// <param name="sText"></param>
        protected override void OnPrint(string sText)
        {
            VmRunEnv.LuaLog.Info("[Print] " + sText);
        } // proc OnPrint

        [LuaMember("require")]
        public override LuaResult LuaRequire(object modname)
        {
            if (modname == null)
                throw new ArgumentNullException();

            // check if the modul is loaded in this global
            if (loaded.ContainsKey(modname))
                return new LuaResult(loaded[modname]);

            if (modname.ToString() == "mylib")
            {
                if (mylib == null)
                    mylib = new ContractCoreLib(VmRunEnv);

                return new LuaResult(mylib);
            }

            // check if the modul is loaded in a different global
            var chunk = ((LuaLibraryPackage)LuaPackage).LuaRequire(this, modname as string);
            if (chunk != null)
                return new LuaResult(loaded[modname] = DoChunk(chunk)[0]);
            else
                return LuaResult.Empty;
        } // func LuaRequire

        #endregion


    } // class LuaGlobal

    #endregion
}
