using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WaykiContract
{
    public static class EncryptLib
    {
        public static byte[] Sha256(string strData)
        {
            var bytValue = System.Text.Encoding.UTF8.GetBytes(strData);

            try
            {
                SHA256 sha256 = new SHA256CryptoServiceProvider();
                return sha256.ComputeHash(bytValue);
            }
            catch (Exception ex)
            {
                throw new Exception("GetSHA256HashFromString() fail,error:" + ex.Message);
            }
        }
    }
}
