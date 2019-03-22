using System;
using System.Collections.Generic;
using System.Text;

namespace WaykiContract
{
    public interface IAppData
    {
        void Insert(string key, string value);

        void InsertInt(string key, long value);

        void InsertBytes(string key, byte[] value);

        string Select(string key);

        byte[] SelectBytes(string key);

        long SelectInt(string key);

        long[] SelectListLong(string key);

        void InsertListLong(string key, long[] value);

        bool Delete(string key);
    }
}
