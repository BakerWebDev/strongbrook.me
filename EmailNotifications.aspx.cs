using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EmailNotifications : System.Web.UI.Page, IPostBackEventHandler
{
    protected string FromEmail = GlobalSettings.Mail.NoReplyEmailAddress;
    protected string ReplyToEmail = GlobalSettings.Mail.NoReplyEmailAddress;





    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check to see if we have a query string variable called 'success'. If so, we know that we just submitted this form successfully, and we are done.
        if (Request.QueryString["success"] != null)
        {
            RaisePostBackEvent("Complete");
            return;
        }

        // If there is no query string variable called 'success', we know we're not done.
        else
        {
            // Load the existing email address into the text field if it's the first time the page is loaded.
            if (!IsPostBack)
            {
                Email = Customer.Email;
            }
        }
    }
    #endregion

    #region Properties
    public string Email
    {
        get { return txtEmail.Text.FormatForExigo(ExigoDataFormatType.Email); }
        set { txtEmail.Text = value; }
    }

    public CustomerResponse Customer
    {
        get
        {
            if (_customer == null)
            {
                _customer = ExigoApiContext.CreateWebServiceContext().GetCustomers(new GetCustomersRequest()
                {
                    CustomerID = Identity.Current.CustomerID
                }).Customers[0];
            }
            return _customer;
        }
    }
    private CustomerResponse _customer;
    #endregion

    #region Render
    public void RenderOptInStatus()
    {
        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);

        if (Customer.IsSubscribedToBroadcasts)
        {
            writer.Write(string.Format("<b style='color: green'>You are receiving email notifications at <u>{0}</u>.</b>  | <a href=\"{1}\">Unsubscribe</a> ",
                Customer.Email,
                Page.ClientScript.GetPostBackClientHyperlink(this, "OptOutCustomer")));
        }
        else
        {
            writer.Write(string.Format("<b>You are not receiving email notifications.</b>"));
        }
    }
    #endregion

    #region Submit Methods
    // Updates the customer's email with the email address provided, sends the email via the MailMessage .NET object and redirects back to this page with the added "success" variable.
    public void SubmitForm()
    {
        // Send the email
        SendEmail(Email);

        // Redirect back to this page. We do this to ensure that if the user refreshes this page after we send the email, we don't re-send the same email.
        string sep = "&";
        if (!Request.RawUrl.Contains("?")) sep = "?";
        Response.Redirect(Request.Url.PathAndQuery + sep + "success=1");
    }

    // Resends the email based on the email address on file rather than the new one. This should only be called if they clicked a link saying they didn't get the email the first time.
    public void ResendEmail()
    {
        // Send the email
        SendEmail(Customer.Email);

        // Redirect back to this page. We do this to ensure that if the user refreshes this page after we send the email, we don't re-send the same email.
        string sep = "&";
        if (!Request.RawUrl.Contains("?")) sep = "?";
        Response.Redirect(Request.Url.PathAndQuery + sep + "success=1");
    }

    // Opts the customer out, and redirects back to this page to ensure our customer variables are up to date.
    public void OptOutCustomer()
    {
        // Opt the customer in or out based on their current settings.
        ExigoApiContext.CreateWebServiceContext().UpdateCustomer(new UpdateCustomerRequest()
        {
            CustomerID = Identity.Current.CustomerID,
            SubscribeToBroadcasts = false
        });

        // Redirect back to this page to reload all the customer variables.
        Response.Redirect(Request.Url.PathAndQuery);
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
            case "OptIn": ResetPanels(Page); Panel_OptIn.Visible = true; break;
            case "Complete": ResetPanels(Page); Panel_Complete.Visible = true; break;

            // Methods for client-side calls
            case "OptOutCustomer": OptOutCustomer(); break;
            case "SubmitForm": SubmitForm(); break;
            case "ResendEmail": ResendEmail(); break;
        }
    }
    #endregion

    #region Helper Methods
    // Sends the email
    public void SendEmail(string RecipientEmail)
    {
        // Configure our message's details        
        MailAddress recipientEmail = new MailAddress(RecipientEmail);
        MailAddress fromEmail = new MailAddress(FromEmail);
        MailAddress replyToEmail = new MailAddress(ReplyToEmail);

        // Credentials used to send mail through SMTP
        SmtpClient smtp = new SmtpClient(GlobalSettings.Mail.SMTPServerUrl, GlobalSettings.Mail.SMTPServerPort);
        smtp.Credentials = new System.Net.NetworkCredential(GlobalSettings.Mail.SMTPServerLoginName, GlobalSettings.Mail.SMTPServerPassword);
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.EnableSsl = GlobalSettings.Mail.SMTPSecureConnectionRequired;

        // Create the MailMessage object
        MailMessage m = new MailMessage(fromEmail, recipientEmail);

        m.Priority = System.Net.Mail.MailPriority.Normal;
        m.ReplyToList.Add(new MailAddress(ReplyToEmail));
        m.Sender = fromEmail;
        m.IsBodyHtml = true;

        m.Subject = string.Format("Email Notifications Confirmation");
        m.Body = string.Format(@"
        <p>
            {1} has received a request to enable this email account to receive email notifications from {1} and your upline.
        </p>

        <p> 
            To confirm this email account, please click the following link:<br />
            <a href='{0}'>{0}</a>
        </p>

        <p>
            If you did not request email notifications from {1}, or believe you have received this email in error, please contact {1} customer service.
        </p>

        <p>
            Sincerely, <br />
            {1} Customer Service
        </p>
        ",
                GetFormattedVerificationURL(),
                GlobalSettings.Company.Name);

        // Send the email
        smtp.Send(m);
    }

    // Used for verification URL encryption
    string Encrypt(string uncoded)
    {
        RijndaelManaged cryptProvider = new RijndaelManaged();
        cryptProvider.KeySize = 256;
        cryptProvider.BlockSize = 256;
        cryptProvider.Mode = CipherMode.CBC;
        SHA256Managed hashSHA256 = new SHA256Managed();
        cryptProvider.Key = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes("OptInRequest"));
        string iv = "OptInRequest";
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

    // Compile and return the verification URL
    public string GetFormattedVerificationURL()
    {
        string sep = "&";
        if (!GlobalSettings.Websites.EmailVerification.Contains("?")) sep = "?";

        string encryptedValues = Server.UrlEncode(Encrypt(string.Format("{0}|{1}|{2}",
            Identity.Current.CustomerID,
            Email,
            DateTime.Now)));

        return GlobalSettings.Websites.EmailVerification + sep + "confirm=" + encryptedValues;
    }
    #endregion
}