using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Includes_Header : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Identity.Current != null)
        {
            FullName = Identity.Current.FirstName + " " + Identity.Current.LastName;
        }
    }

    public string FullName;

    public void Click_Logout(Object sender, EventArgs e)
    {
        try
        {
            var svc = new IdentityAuthenticationService();
            svc.SignOut();
            Response.Cookies["userCookie"].Expires = DateTime.Now.AddDays(-20);
            Response.Redirect("~/SignOut.aspx");
        }
        catch
        {
            return;
        }
    }
}