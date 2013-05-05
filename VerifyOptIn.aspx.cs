using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VerifyOptIn : Page, IPostBackEventHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["confirm"] != null)
        {
            OptInCustomer();
            RaisePostBackEvent("Verified");
        }
        else
        {
            RaisePostBackEvent("AccessDenied");
        }
    }

    #region Properties
    public CustomerResponse Customer
    {
        get
        {
            if (_customer == null)
            {
                _customer = ExigoApiContext.CreateWebServiceContext().GetCustomers(new GetCustomersRequest()
                {
                    CustomerID = CustomerID
                }).Customers[0];
            }
            return _customer;
        }
    }
    private CustomerResponse _customer;

    public int CustomerID
    {
        get
        {
            if (Request["confirm"] != null)
            {
                string s = Decrypt(Request.QueryString["confirm"]);
                string[] sa = s.Split('|');

                _customerID = Convert.ToInt32(sa[0]);
            }
            return _customerID;
        }
    }
    private int _customerID;

    public string Email
    {
        get
        {
            if (Request["confirm"] != null)
            {
                string s = Decrypt(Request.QueryString["confirm"]);
                string[] sa = s.Split('|');

                _email = sa[1];
            }
            return _email;
        }
    }
    private string _email;
    #endregion

    #region API Methods
    public void OptInCustomer()
    {
        // Check to see if the customer is opted in first. If so, let's opt them out.
        if (Customer.IsSubscribedToBroadcasts)
        {
            ExigoApiContext.CreateWebServiceContext().UpdateCustomer(new UpdateCustomerRequest()
            {
                CustomerID = CustomerID,
                SubscribeToBroadcasts = false
            });
        }

        string currentIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

        // If this is a localhost test, set the IP address for testing purposes
        if (currentIP == "::1") currentIP = "1.0.0.0";

        ExigoApiContext.CreateWebServiceContext().UpdateCustomer(new UpdateCustomerRequest()
        {
            CustomerID = CustomerID,
            Email = Email,
            SubscribeToBroadcasts = true,
            SubscribeFromIPAddress = currentIP
        });
    }
    #endregion

    #region Panels & Navigation
    // Resets all panels within the supplied control. Pass the Page object to reset all panels on the page.
    private void ResetPanels(Control cnt)
    {
        if (cnt is Panel)
        {
            ((Panel)cnt).Visible = false;
        }
        foreach (Control subCnt in cnt.Controls)
        {
            ResetPanels(subCnt);
        }
    }

    // Implementation from the IPostBackEventHandler interface. Handles all panel showing and hiding through the passed argument.
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] a = eventArgument.Split('|');

        switch (a[0])
        {
            // Panels
            case "Verified": ResetPanels(Page); Panel_Verified.Visible = true; break;
            case "AccessDenied": ResetPanels(Page); Panel_AccessDenied.Visible = true; break;
        }
    }
    #endregion

    #region Helper Methods
    // Used for verification URL decryption
    string Decrypt(string coded)
    {
        RijndaelManaged cryptProvider = new RijndaelManaged();
        cryptProvider.KeySize = 256;
        cryptProvider.BlockSize = 256;
        cryptProvider.Mode = CipherMode.CBC;
        SHA256Managed hashSHA256 = new SHA256Managed();
        cryptProvider.Key = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes("OptInRequest"));
        string iv = "OptInRequest";
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
