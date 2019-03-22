using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DBreeze;

namespace WaykiContract
{
    public class DBreezeDb : IAppData
    {
        public DBreezeEngine DBreezeEngine = null;
        public string TableName = "default";

        public static DBreezeDb CreateDatabase(string scriptId)
        {
            DBreezeDb db = new DBreezeDb();
            if (db.DBreezeEngine == null)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    db.DBreezeEngine = new DBreezeEngine(Environment.CurrentDirectory + "/DB/" + scriptId);
                }
                else
                {
                    db.DBreezeEngine = new DBreezeEngine(Environment.CurrentDirectory + "\\DB\\" + scriptId);
                }
            }

            return db;
        }

        public IAppData GetAppTable(string table)
        {
            return new DBreezeDb()
            {
                DBreezeEngine = this.DBreezeEngine,
                TableName = table
            };
        }

        public void Insert(string key, string value)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                tran.Insert(TableName, key, value);
                tran.Commit();
            }
        }

        public void InsertInt(string key, long value)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                tran.Insert<string, long>(TableName, key, value);
                tran.Commit();
            }
        }

        public void InsertBytes(string key, byte[] value)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                tran.Insert<string, byte[]>(TableName, key, value);
                tran.Commit();
            }
        }

        public string Select(string key)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                var row = tran.Select<string, string>(TableName, key);

                if (row.Exists)
                {
                    return row.Value;
                }
            }

            return null;
        }

        public byte[] SelectBytes(string key)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                var row = tran.Select<string, byte[]>(TableName, key);

                if (row.Exists)
                {
                    return row.Value;
                }
            }

            return null;
        }

        public long SelectInt(string key)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                var row = tran.Select<string, long>(TableName, key);

                if (row.Exists)
                {
                    return row.Value;
                }
            }

            return 0;
        }

        public long[] SelectListLong(string key)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                var row = tran.Select<string, string>(TableName, key);

                if (row.Exists && !string.IsNullOrEmpty(row.Value))
                {
                    var cells = row.Value.Split(',').Select(long.Parse).ToArray();

                    return cells;
                }
            }

            return null;
        }

        public void InsertListLong(string key, long[] value)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                tran.Insert<string, string>(TableName, key, string.Join(",", value.Select(x => x.ToString())));
                tran.Commit();
            }
        }

        public bool Delete(string key)
        {
            using (var tran = DBreezeEngine.GetTransaction())
            {
                tran.RemoveKey(TableName, key);
                tran.Commit();
            }

            return true;
        }

    }
}
