using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Exigo.OData;
//using Exigo.API;
//using Exigo.Settings;
//using Exigo.Data;
using Exigo.WebService;
using System.Security.Cryptography;
using System.IO;

public partial class SilentLogin : System.Web.UI.Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        // Allows cookies to be passed through iFrames.
        HttpContext.Current.Response.AddHeader("p3p", "CP=\"IDC DSP COR CURa ADMa PHY DEVi TAIi PSA PSD IVAi ONL IVDi CONi HIS OUR IND CNT COM STA\"");


        if (!IsPostBack)
        {
            if (Request.QueryString["confirm"] != null)
            {
                DecryptQueryString();
            }
        }



        //if (AuthenticateSessionID()) return;
        //if (AutenticateSilentLogin()) return;
        IfAllisWellRedirect();
    }

    //ExigoApiServices Api = new ExigoApiServices();
    #endregion

    public string ToPortal = "https://strongbrookbackoffice.secure-backoffice.net/Public/SilentLogin.aspx";
    public string ToPortal2 = "http://strongbrookdirect.com/Secure/WealthClub.aspx";
    public string ToPortal3 = "Home.aspx";

    public void IfAllisWellRedirect()
    {
        try
        {



            if (AutenticateSilentLogin())
            {
                if (AuthenticateUserInfo())
                {
                    Response.Redirect(ToPortal3);
                }
                else
                {
                    Response.Write("There was a problem. Plesae double check your values and try again.");
                }
            }
            else
            {
                Response.Redirect("../TrySilentLoginTestAgain.aspx");
            }
        }
        catch
        {
            Response.Write("Your user information is incorrect or invalid. Please double check your values and try again.");
        }
    }



    
    #region Authentication

    public bool AuthenticateUserInfo()
    {
        // Go ahead and validate the request
        try
        {
            // Decrypt the 'token' query string
                //var decryptedString = Decrypt(Request.QueryString["token"], WebSettingsContext.SilentLogins.EncryptionKey);

            // Split it up by pipes
            //string[] args = decryptedString.Split('|');

            // Set some local variables so we don't have to keep accessing the array by index
            int customerID = NewCustomerID; // Convert.ToInt32(args[0]);
            string loginName = NewUsername; // args[1];
            DateTime tokenCreatedDate = DateTime.Now; // Convert.ToDateTime(args[2]);

            // If we got here, let's create the FormsAuthentication cookie and move them to the default page.
            var svc = new IdentityAuthenticationService();
            if (svc.SilentLogin(customerID, loginName))
            {
                //SaveUserCookie(loginName);
                return true;
            }
            else
            {
                Response.Write("Your user information is incorrect or invalid. Please double check your values and try again.");
                return false;
            }
        }

        // If ANYTHING goes wrong, let's just return a generic message so as not to frighten the users.
        catch
        {
            Response.Write("Your user information is incorrect or invalid. Please double check your values and try again.");
            return false;
        }
    }

    //public bool AutenticateSilentLogin()
    //{
    //    // Are we even allowed to silently login? If not, let's stop right here.
    //    if (!WebSettingsContext.SilentLogins.AllowSilentLogins)
    //    {
    //        Response.Write("Silent logins are not permitted at this time.");
    //        return false;
    //    }

    //    // Validate that we have a query string request called 'token'
    //    if (Request.QueryString["token"] == null)
    //    {
    //        Response.Write("Missing token.");
    //        return false;
    //    }

    //    // Go ahead and validate the request
    //    try
    //    {
    //        // Decrypt the 'token' query string
    //        var decryptedString = Decrypt(Request.QueryString["token"], WebSettingsContext.SilentLogins.EncryptionKey, WebSettingsContext.SilentLogins.IVKey);


    //        // Split it up by pipes
    //        string[] args = decryptedString.Split('|');


    //        // Set some local variables so we don't have to keep accessing the array by index
    //        int customerID = Convert.ToInt32(args[0]);
    //        string loginName = args[1];
    //        DateTime tokenCreatedDate = Convert.ToDateTime(args[2]);


    //        // Check to ensure that the token has not expire. By default, we expire the tokens 15 minutes after they have been created.
    //        //if (WebSettingsContext.SilentLogins.TokenLifeSpan > 0)
    //        //{
    //        //    TimeSpan timeDifference = DateTime.Now.ToUniversalTime().Subtract(tokenCreatedDate);
    //        //    if (timeDifference.Minutes > WebSettingsContext.SilentLogins.TokenLifeSpan)
    //        //    {
    //        //        Response.Write("Your token has expired. Please request a new token and try again.");
    //        //        return false;
    //        //    }
    //        //}


    //        // If we got here, let's create the FormsAuthentication cookie and move them to the default page.
    //        var svc = new AuthenticationService();
    //        if (svc.SilentLogin(customerID, loginName))
    //        {
    //            SaveUserCookie(loginName);
                 
    //            // Did they request a specific page through the query string? Let's look at the 'page' variable to see if we have one.
    //            // If so, let's redirect them there.
    //            if (Request.QueryString["page"] != null)
    //            {
    //                //Response.Redirect("../Secure/" + Server.UrlDecode(Request.QueryString["page"]), false);
    //                Response.Redirect("../Secure/" + Server.UrlDecode(Request.QueryString["page"]));

    //                return true;
    //            }
    //            else
    //            {
    //                Response.Redirect("../Secure/Default.aspx");
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            Response.Write("Your token contains invalid credentials. Plesae request a new token and try again.");
    //            return false;
    //        }
    //    }

    //    // If ANYTHING goes wrong, let's just return a generic message so as not to frighten the users.
    //    catch
    //    {
    //        Response.Write("Invalid token. Please request a new token and try again.");
    //        return false;
    //    }
    //}

    public bool AutenticateSilentLogin()
    {
        bool isValid = false;
        try
        {
            // Validate that we have a query string request called 'token'
            if (Request.QueryString["confirm"] != null)
            {
                // Decrypt the 'token' query string
                //var decryptedString = Decrypt(Request.QueryString["token"], WebSettingsContext.SilentLogins.EncryptionKey);

                // Are we even allowed to silently login? If not, let's stop right here.
                if (1 == 2) //!WebSettingsContext.SilentLogins.AllowSilentLogins)
                {
                    Response.Write("Silent logins are not permitted at this time.");
                    isValid = false;
                    return isValid;
                }
                else
                {
                    isValid = true;
                }
            }
            else
            {
                Response.Write("Missing token.");
                isValid = false;
                return isValid;
            }
        }

        // If ANYTHING goes wrong, let's just return a generic message so as not to frighten the users.
        catch
        {
            Response.Write("Invalid token. Please request a new token and try again.");
            isValid = false;
            return isValid;
        }
        return isValid;
    }
    #endregion

    //#region Cookie Methods
    ////Used to encrypt each permission and adds it to the userCookie
    //public void AddEncryptedCookieValues(HttpCookie userCookie, User user)
    //{
    //    try
    //    {
    //        if (user.UserPermissions != null)
    //        {
    //            foreach (var q in user.UserPermissions)
    //            {
    //                userCookie.Values.Add(string.Format("{0}", q.Key), Encrypt(string.Format("{0}", q.Value), "justdoit"));
    //            }
    //        }
    //    }
    //    catch
    //    {
    //        //
    //    }
    //}

    //public void SaveUserCookie(string loginName)
    //{
    //    int CustomerID = 0;
    //    CustomerType type = new CustomerType();
    //    //string LitmostApi = "";

    //    var cust = (from c in Api.OData.Customers
    //                where c.LoginName == loginName
    //                select new
    //                {
    //                    c.CustomerID,
    //                    c.CustomerType,
    //                    //c.Field4
    //                });
    //    foreach (var c in cust)
    //    {
    //        CustomerID = c.CustomerID;
    //        type = c.CustomerType;
    //        //LitmostApi = c.Field4;
    //    }


    //    //create user with permissions
    //    User user = new User(type, CustomerID);
    //    //user.LitmosAPI = LitmostApi; //Assign a Litmos login if it exists
    //    user.CustomerID = CustomerID;

    //    if (Request.Cookies["userCookie"] == null) //if there isn't a cookie, create one
    //    {
    //        //set user permissions through encryption into UserCookie
    //        CreateNewUserCookie(user);
    //    }

    //    if (Request.Cookies["userCookie"] != null) //if the "userCookie" cookie exists, delete it and create a new one in case anything has changed since the last login.
    //    {
    //        Request.Cookies["userCookie"].Expires = DateTime.Now.AddDays(-1);
    //        CreateNewUserCookie(user);
    //    }
    //}

    //public void CreateNewUserCookie(User user)
    //{
    //    try
    //    {
    //        HttpCookie userCookie = new HttpCookie("userCookie");
    //        userCookie.Expires = DateTime.Now.AddDays(1);

    //        AddEncryptedCookieValues(userCookie, user);
    //        //userCookie.Values.Add("LitmosAPI", user.LitmosAPI);
    //        userCookie.Values.Add("CustomerID", user.CustomerID.ToString());

    //        Response.Cookies.Add(userCookie);
    //    }
    //    catch
    //    {
    //        // ErrorString = "Your request could not be completed.  If you continue to receive this error, please contact support";
    //    }
    //}
    //#endregion



    #region Helper Methods
        public bool TryParseGuid(string value, out Guid result)
        {
            try
            {
                result = new Guid(value.Replace("-", "")); // needed to cater for wrong hyphenation
                return true;
            }
            catch
            {
                result = Guid.Empty;
                return false;
            }
        }
    #endregion














/// <summary>
/// ///////////////////////////////////////////////////
/// </summary>






















    #region Properties


    public int _NewCustomerID { get; set; }
    protected int NewCustomerID
    {
        get
        {
            return _NewCustomerID;
        }
        set { _NewCustomerID = value; }
    }
    public int _NewOrderID { get; set; }
    protected int NewOrderID
    {
        get
        {
            return _NewOrderID;
        }
        set { _NewOrderID = value; }
    }
    public int _NewAutoshipID { get; set; }
    protected int NewAutoshipID
    {
        get
        {
            return _NewAutoshipID;
        }
        set { _NewAutoshipID = value; }
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
    public string _NewPassword { get; set; }
    protected string NewPassword
    {
        get
        {
            return _NewPassword;
        }
        set { _NewPassword = value; }
    }
    public string _IsOptingIn { get; set; }
    protected string IsOptingIn
    {
        get
        {
            return _IsOptingIn;
        }
        set { _IsOptingIn = value; }
    }
    #endregion




    #region Data population
    public void DecryptQueryString()
    {
        string[] Decrypted = Server.UrlDecode(Decrypt(Request.QueryString["confirm"], "justdoit")).Split('|');
        _NewCustomerID = Convert.ToInt32(Decrypted[0]);
        _NewUsername = (Decrypted[1]);
    }
    #endregion

    #region Render
    protected void RenderOrderRecap()
    {
        if (NewOrderID != 0)
        {
            HtmlTextWriter writer = new HtmlTextWriter(Response.Output);

            writer.Write(@"
                <h5>Today's Order</h5>
                <p>
                    Your Order Number: {0}
                    <br />
                    <br />
                    Your order usually ships in 2 to 3 days.
                </p>"
            , NewOrderID);
        }
    }
    #endregion

    #region Encryption Methods
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




}
