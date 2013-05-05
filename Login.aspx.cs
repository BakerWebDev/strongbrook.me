using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // This is to handle silently logging in from the Portal.
            if (Request.QueryString["ReturnUrl"] != null)
            {
                SilentLogin();           
            }

            // Auto-populate the username from the cookie
            if (Request.Cookies[UsernameCookieName] != null)
            {
                chkRememberMe.Checked = true;
                LoginName = Request.Cookies[UsernameCookieName].Value;
                txtPassword.Focus();
            }
            else
            {
                txtLoginName.Focus();
            }


            // Auto-fill some test credentials if we are running this website on our local machines. This is just for convenience.
            if(Request.IsLocal || Request.Url.AbsoluteUri.Contains("sample.exigo.com"))
            {
                txtLoginName.Text = GlobalSettings.LocalSettings.TestLoginName;
                txtPassword.Attributes.Add("value", GlobalSettings.LocalSettings.TestPassword);
            }
        }
    }
    #endregion

    #region Properties
    public string ErrorString { get; set; }

    public string LoginName
    {
        get { return txtLoginName.Text; }
        set { txtLoginName.Text = value; }
    }
    public string Password
    {
        get { return txtPassword.Text; }
        set { txtPassword.Text = value; }
    }
    public bool RememberMe
    {
        get { return chkRememberMe.Checked; }
        set { chkRememberMe.Checked = value; }
    }

    public string UsernameCookieName = "Username";
    #endregion

    #region Helper Methods
    public void SaveUsernameCookie()
    {
        var cookie = Request.Cookies[UsernameCookieName] ?? new HttpCookie(UsernameCookieName);
        cookie.Value = LoginName;
        cookie.Expires = DateTime.Now.AddDays(30);
        Response.Cookies.Add(cookie);
    }
    public void DeleteUsernameCookie()
    {
        HttpCookie cookie = Request.Cookies[UsernameCookieName];
        if (cookie != null)
        {
            cookie = new HttpCookie(UsernameCookieName);
            cookie.Value = string.Empty;
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);
        }
    }
    #endregion

    #region Event Handlers
    public void SignIn_Click(object sender, EventArgs e)
    {
        // If they want to be remembered, let's save their username to a cookie. If not, let's kill any cookies that might already exist.
        if (RememberMe) SaveUsernameCookie();
        else DeleteUsernameCookie();

            var svc = new IdentityAuthenticationService();
            if (svc.SignIn(LoginName, Password))
            {
                if (Request.QueryString["ReturnUrl"] != null)
                {
                    Response.Redirect(Request.QueryString["ReturnUrl"], false);
                }
                else
                {
                    Response.Redirect("~/Home.aspx", false);
                }
            }
            else
            {
                ErrorString = "Invalid username/password. Please try again.";
            }

    }
    #endregion

    #region Silent Login
    public int _NewCustomerID { get; set; }
    protected int NewCustomerID
    {
        get
        {
            return _NewCustomerID;
        }
        set { _NewCustomerID = value; }
    }
    public string _NewUsername { get; set; }
    protected string NewUsername
    {
        get
        {
            return _NewUsername;
        }
        set { _NewUsername = value; }
    }

    #region Data population
    public void DecryptQueryString()
    {
        string[] Decrypted = Server.UrlDecode(Decrypt(Request.QueryString["confirm"], "justdoit")).Split('|');
        _NewCustomerID = Convert.ToInt32(Decrypted[0]);
        _NewUsername = (Decrypted[1]);
    }
    #endregion

    #region Decryption Method
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

    public void SilentLogin()
    {
            int customerID = NewCustomerID;
            string loginName = NewUsername;

            var svc = new IdentityAuthenticationService();
            if (svc.SilentLogin(customerID, loginName))
            {
                if (Request.QueryString["ReturnUrl"] != null)
                {
                    Response.Redirect(Request.QueryString["ReturnUrl"], false);
                }
                else
                {
                    Response.Redirect("~/Home.aspx", false);
                }
            }
            else
            {
                ErrorString = "Invalid username/password. Please try again.";
            }

    }
    #endregion

}
