using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RedirectToPortal : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        RedirectToSilentLogin();
    }

    #region Silent Login to Wealth Portal   
    public string ToSilentLogin = "https://strongbrookbackoffice.secure-backoffice.net/Public/SilentLogin.aspx";

    public int NewCustomerID = Identity.Current.CustomerID;
    public string NewUserName = Identity.Current.Website.LoginName;

    public void RedirectToSilentLogin()
    {
        try
        {
            string sep = "&";
            if (!Request.RawUrl.Contains("?")) sep = "?";

            string var = sep + "confirm=" + Server.UrlEncode(Encrypt(string.Format("{0}|{1}"
                    , NewCustomerID.ToString() // 0    
                    , NewUserName // 1
                    ), "justdoit"));

            Response.Redirect(ToSilentLogin + var);
        }
        catch (Exception ex)
        {
            Response.Write(Request.RawUrl + Request.QueryString);
            string ErrorString = "Error";
            ErrorString += "Oops!<br />Sorry, but something went wrong...<br />" + ex.Message;
        }
    }

    #region Encryption/Decryption methods
    string Encrypt(string uncoded, string key)
    {
        RijndaelManaged cryptProvider = new RijndaelManaged();
        cryptProvider.KeySize = 256;
        cryptProvider.BlockSize = 256;
        cryptProvider.Mode = CipherMode.CBC;
        SHA256Managed hashSHA256 = new SHA256Managed();
        cryptProvider.Key = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
        string iv = "user";
        cryptProvider.IV = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(iv));
        byte[] plainTextByteArray = ASCIIEncoding.ASCII.GetBytes(uncoded);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, cryptProvider.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(plainTextByteArray, 0, plainTextByteArray.Length);
        cs.FlushFinalBlock();
        cs.Close();
        byte[] byt = ms.ToArray();
        return Convert.ToBase64String(byt);
    }
    string Decrypt(string coded, string key)
    {
        RijndaelManaged cryptProvider = new RijndaelManaged();
        cryptProvider.KeySize = 256;
        cryptProvider.BlockSize = 256;
        cryptProvider.Mode = CipherMode.CBC;
        SHA256Managed hashSHA256 = new SHA256Managed();
        cryptProvider.Key = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
        string iv = "user";
        cryptProvider.IV = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(iv));
        byte[] cipherTextByteArray = Convert.FromBase64String(coded);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, cryptProvider.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(cipherTextByteArray, 0, cipherTextByteArray.Length);
        cs.FlushFinalBlock();
        cs.Close();
        byte[] byt = ms.ToArray();
        return Encoding.ASCII.GetString(byt);
    }
    #endregion
    #endregion Silent Login to Wealth Portal


}