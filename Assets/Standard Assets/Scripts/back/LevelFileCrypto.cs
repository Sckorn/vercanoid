using UnityEngine;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.Xml;

public static class LevelFileCrypto {

    [System.Runtime.InteropServices.DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
    public static extern bool ZeroMemory(ref string Destination, int length);

    static private string permanentKey = "fp910DX!";//"fkmvfyfp910DX!";

    static public string PermanentKey
    {
        get { return LevelFileCrypto.permanentKey; }
    }
    
    static public string GenerateSessionCryptoKey()
    {
        DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
        //Debug.LogWarning(ASCIIEncoding.ASCII.GetString(desCrypto.Key));
        return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
    }

    static public void EncryptFile(string sInputFilePath, string sOutputFilePath, string sKey)
    {
        if (sKey.Equals(string.Empty)) sKey = LevelFileCrypto.PermanentKey;

        FileStream fsInput = new FileStream(sInputFilePath, FileMode.Open, FileAccess.Read);
        FileStream fsEncrypted = new FileStream(sOutputFilePath, FileMode.Create, FileAccess.Write);

        DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
        DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
        DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

        ICryptoTransform desencrypt = DES.CreateEncryptor();
        CryptoStream cryptoStream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write);

        byte[] byteArrayInput = new byte[fsInput.Length - 1];
        fsInput.Read(byteArrayInput, 0, byteArrayInput.Length);
        cryptoStream.Write(byteArrayInput, 0, byteArrayInput.Length);

        cryptoStream.Close();
        fsEncrypted.Close();
        fsInput.Close();
    }

    static public void DecryptFile(string sInputFilePath, string sOutputFilePath, string sKey)
    {
        if (sKey.Equals(string.Empty)) sKey = LevelFileCrypto.PermanentKey;

        DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
        DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
        DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

        FileStream fsRead = new FileStream(sInputFilePath, FileMode.Open, FileAccess.Read);

        ICryptoTransform desDecrypt = DES.CreateDecryptor();
        CryptoStream cryptoStreamDecr = new CryptoStream(fsRead, desDecrypt, CryptoStreamMode.Read);

        StreamWriter fsDecrypter = new StreamWriter(sOutputFilePath);
        fsDecrypter.Write(new StreamReader(cryptoStreamDecr).ReadToEnd());
        fsDecrypter.Flush();
        fsDecrypter.Close();
    }

    static public string DecryptFile(string sInputFilePath, string sKey)
    {
        string result = string.Empty;

        if (sKey.Equals(string.Empty)) sKey = LevelFileCrypto.PermanentKey;

        DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
        DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
        DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

        FileStream fsRead = new FileStream(sInputFilePath, FileMode.Open, FileAccess.Read);

        ICryptoTransform desDecrypt = DES.CreateDecryptor();
        CryptoStream cryptoStreamDecr = new CryptoStream(fsRead, desDecrypt, CryptoStreamMode.Read);

        return new StreamReader(cryptoStreamDecr).ReadToEnd();
    }

    static public void WriteLevelsXml(string xmlFilePath)
    { 
        
    }
}
