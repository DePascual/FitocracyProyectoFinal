using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FitocracyProyectoFinal.Models
{
    public class EncriptacionClass
    {
        byte[] _key;

        byte[] _iv;

        public EncriptacionClass()
        {
            _key = Encoding.UTF8.GetBytes("12EstaClave34es56dificil489ssswf");
            _iv = Encoding.UTF8.GetBytes("AulaJose11.37hAES");
        }


        public string Encrit(string inputText)
        {

            byte[] inputBytes = Encoding.UTF8.GetBytes(inputText);
            byte[] encripted;
            string textoLimpio = String.Empty;

            RijndaelManaged cripto = new RijndaelManaged();

            try
            {
                using (MemoryStream ms = new MemoryStream(inputBytes.Length))
                {
                    using (CryptoStream objCryptoStream =
                        new CryptoStream(ms, cripto.CreateEncryptor(_key, _iv), CryptoStreamMode.Write))
                    {
                        objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                        objCryptoStream.FlushFinalBlock();
                        objCryptoStream.Close();
                    }
                    encripted = ms.ToArray();
                }
                return Convert.ToBase64String(encripted);
            }
            catch (Exception e)
            {
                String ex = e.ToString();
                return textoLimpio = "";
            }
        }



        public string Desencrit(string inputText)
        {

            byte[] inputBytes = Convert.FromBase64String(inputText);
            byte[] resultBytes = new byte[inputBytes.Length];
            string textoLimpio = String.Empty;

            RijndaelManaged cripto = new RijndaelManaged();

            try
            {
                using (MemoryStream ms = new MemoryStream(inputBytes))
                {

                    using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(_key, _iv),
                        CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(objCryptoStream, true))
                        {
                            textoLimpio = sr.ReadToEnd();
                        }
                    }
                }
                return textoLimpio;
            }
            catch (Exception)
            {

                return textoLimpio = "";
            }
        }
    }
}