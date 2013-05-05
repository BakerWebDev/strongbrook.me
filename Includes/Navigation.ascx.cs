using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Exigo.API;
//using Exigo.Settings;
using Exigo.OData;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.Cryptography;



public partial class Includes_Navigation : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ClearErrorString();

        //Sets the websites for client and ibd enroll
        MyIBDEnrollSite = "http://strongbrookenroll.secure-backoffice.net/default.aspx?ID=" + Identity.Current.CustomerID;
        MyClientEnrollSite = "http://strongbrookshop.secure-backoffice.net/default.aspx?ID=" + Identity.Current.CustomerID;
    }


    #region Public Settings
    public string OpenInSeparateWindow = "target='_blank'";
    public string MyIBDEnrollSite;
    public string MyClientEnrollSite;
    

    //Silent Login URL for Litmos
    public string LitmosLoginKey
    {
        get { return _litmosLoginKey; }
        set { _litmosLoginKey = value; }
    }

    ////Current User Identity Info
    //public Identity Identity { get { return base.Identity; } }



    //public string backOfficeURI = "Default.aspx";
    public string backOfficeURI = "http://strongbrook.me/SilentLogin.aspx";
    #endregion

    #region Private Settings

    //Litmos Api Info
    private string LitmosApiKey
    {
        get { return "558755F0-2546-48CE-925C-18681D4D5909"; }
    }
    private string source
    {
        get { return "StrongBrook"; }
    }
    private string _litmosLoginKey;

    #endregion

    #region Render Methods
    public void RenderPortalLinks()
    {
        /*
         * This method goes through the customer type and checks if they are wealth club purchasers or not.
         * Foreach permission it returns true or false if they can view it or not, if false it doesn't write
         * the html.
         * */
        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);

        StringBuilder s = new StringBuilder();  //Long string - build it all first, will be raw HTML code

        User user = new User();
        user = user.DecryptUserInformation(Request.Cookies["userCookie"], user);

        #region User List (Permissions)
        if (Identity.Current.CustomerID == 24100) // Allow access to Levels: 1, 2, 3, 4
        {
            user.SetPermissionsForCustomerTypeAdministrator(user.SuperUserPermissionLevel_1); // Reference ID-3 // Override ID-5 on User.cs 
        }
        else if (Identity.Current.CustomerID == 22434) // Allow access to Levels: 1, 2, 4
        {
            user.SetPermissionsForCustomerTypeAdministratorsWithLessAccess(user.SuperUserPermissionLevel_2); // Reference ID-3 // Override ID-5 on User.cs
        }
        else if (Identity.Current.CustomerID == 10005) // Allow access to Levels: 1, 2
        {
            user.SetPermissionsForCustomerTypeAdministratorWithNoSuperUserAccess(); // Reference ID-3 // Override ID-5 on User.cs
        }
        else if (Identity.Current.CustomerID == 11309) // Allow access to Level: 2
        {
            user.SetPermissionsForCustomerTypeAdministratorWithNoSuperUserAccessAndOnlyOneLink(); // Reference ID-3 // Override ID-5 on User.cs
        }
        else
        {
            Response.Redirect("~/UnexpectedError.aspx");
        }
        #endregion User List (Permissions)




        #region Access Level Items
        // Navigation List Item # 1
        s.AppendLine(@"<li><a href=""Home.aspx"">Home</a>");
        // Navigation List Item # 2
        if (user.IsAllowedUserLevel_2_Access)
        {
            s.AppendLine(string.Format(@"<li><a href=""{0}"">GPR Manager</a></li>
            ", "GPR_LeadManager.aspx"));
        }


        // Navigation List Item # 3
        if (user.IsAllowedUserLevel_1_Access)
        {
            s.AppendLine(string.Format(@"<li><a href=""{0}"">IsAllowedUserLevel_1_Access</a></li>
            ", "WealthClub_Benefits.aspx"));
        }



        // Navigation List Item # 4
        if (user.IsAllowedSuperUserLevel_2_Access)
        {
            s.AppendLine(string.Format(@"<li><a href=""{0}"">AllowSuperUserLevel_2_Access</a></li>
            ", "ItemCategories.aspx"));
        }
        // Navigation List Item # 5
        if (user.IsAllowedSuperUserLevel_1_Access)
        {
            s.AppendLine(string.Format(@"<li><a href=""{0}"">AllowSuperUserLevel_1_Access</a></li>
            ", "ComplianceTraining.aspx"));
        }
        // Navigation List Item # 6
        if (user.AllowBackOffice)
        {
            s.AppendLine(string.Format(@"<li><a href=""{0}"">Shopping</a></li>
            ", "ShoppingCategories.aspx"));
        }
        // Navigation List Item # 7
        if (user.AllowBackOffice)
        {
            s.AppendLine(string.Format(@"<li><a href=""{0}"">Library</a></li>
            ", "LibraryItems.aspx"));
        }
        // Navigation List Item # 8
        if (user.AllowBackOffice)
        {
            int NewCustomerID = Identity.Current.CustomerID;
            string NewUserName = Identity.Current.Website.LoginName;

            string sep = "&";
            if (!Request.RawUrl.Contains("?")) sep = "?";

            string qs = sep + "confirm=" + Server.UrlEncode(Encrypt(string.Format("{0}|{1}"
                                , NewCustomerID.ToString() // 0   
                                , NewUserName // 1
                                ), "justdoit"));

            s.AppendLine(string.Format(@"<li><a href=""{0}"">My Backoffice</a></li>
            ", backOfficeURI + qs));
        }
        #endregion  Access Level Items
        writer.Write(s);
    }
    #endregion

    #region ErrorHandling
    public string ErrorString
    {
        get
        {
            return _errorString;
        }
        set
        {
            _errorString += value;
            ApplicationErrors.Value = Server.UrlEncode(_errorString.Replace("'", "\"")).Replace("%0a", "").Replace("%0A", "");
        }
    }
    private string _errorString;

    private void ClearErrorString()
    {
        ErrorString = string.Empty;
    }
    #endregion

    #region Encryption method
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
    #endregion
}