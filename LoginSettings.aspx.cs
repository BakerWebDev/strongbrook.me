using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class LoginSettings : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            if(Request.QueryString["status"] == "1")
            {
                Error.Type = Exigo.WebControls.ErrorMessageType.Success;
                Error.Header = "Success!";
                Error.Message = "Your login preferences have been saved.";
            }
        }
    }

    #region Properties
    public string NewLoginName
    {
        get { return txtNewUsername.Text.FormatForExigo(ExigoDataFormatType.Username); }
        set { txtNewUsername.Text = value; }
    }
    public string OldPassword
    {
        get { return txtOldPassword.Text; }
        set { txtOldPassword.Text = value; }
    }
    public string NewPassword
    {
        get { return txtNewPassword.Text; }
        set { txtNewPassword.Text = value; }
    }
    #endregion

    #region Exigo API Requests
    private UpdateCustomerRequest Request_UpdateCustomerLoginName()
    {
        var request = new UpdateCustomerRequest();

        request.CustomerID      = Identity.Current.CustomerID;
        request.LoginName       = NewLoginName;

        return request;
    }
    private UpdateCustomerRequest Request_UpdateCustomerPassword()
    {
        var request = new UpdateCustomerRequest();

        request.CustomerID      = Identity.Current.CustomerID;
        request.LoginPassword   = NewPassword;

        return request;
    }
    #endregion

    #region Submitting Changes
    private void SubmitLoginNameChanges()
    {
        ExigoApiContext.CreateWebServiceContext().UpdateCustomer(Request_UpdateCustomerLoginName());

        IdentityAuthenticationService service = new IdentityAuthenticationService();
        service.RefreshIdentity();
    }
    private void SubmitPasswordChanges()
    {
        ExigoApiContext.CreateWebServiceContext().UpdateCustomer(Request_UpdateCustomerPassword());

        IdentityAuthenticationService service = new IdentityAuthenticationService();
        service.RefreshIdentity();
    }
    #endregion

    #region Event Handlers
    public void SubmitLoginNameChanges_Click(object sender, EventArgs e)
    {
        try
        {
            SubmitLoginNameChanges();
            Response.Redirect(Request.Url.AbsolutePath + "?status=1");
        }
        catch(Exception exception)
        {
            Error.Type = Exigo.WebControls.ErrorMessageType.Failure;
            Error.Header = "Oops!";
            Error.Message = "We were unable to save your new username: " + exception.Message;
        }
    }
    public void SubmitPasswordChanges_Click(object sender, EventArgs e)
    {
        try
        {
            SubmitPasswordChanges();
            Response.Redirect(Request.Url.AbsolutePath + "?status=1");
        }
        catch(Exception exception)
        {
            Error.Type = Exigo.WebControls.ErrorMessageType.Failure;
            Error.Header = "Oops!";
            Error.Message = "We were unable to save your new password: " + exception.Message;
        }
    }
    public void Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
    #endregion
}