using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.WebService;

public partial class SummaryData : Page
{
    public int CustomerID { get { return Convert.ToInt32(Request.QueryString["id"]); } }

    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] == null)
        {
            Response.Write("Invalid Customer ID");
            Response.End();
        }
    }
    #endregion

    #region Render
    protected override void Render(HtmlTextWriter writer)
    {
        var context = ExigoApiContext.CreateWebServiceContext();
        var customer = context.GetCustomers(new GetCustomersRequest
        {
            CustomerID = CustomerID
        }).Customers[0];


        string webalias = string.Empty;
        try
        {
            webalias = context.GetCustomerSite(new GetCustomerSiteRequest
            {
                CustomerID = CustomerID
            }).WebAlias;
        }
        catch
        {
            webalias = string.Empty;
        }


        // Render it
        writer.Write(string.Format(@"
                <img src='Assets/Images/avatar.png' id='ProfilePic' />
                <div>
                    <h3>{0} {1} - ID# {2}</h3>
                    email:<a href='mailto:{3}'>&nbsp;{3}</a><br />
                    Phone: {4}
                </div>
                <div class='ClearAllFloats'></div>

            ", customer.FirstName   // 0
             , customer.LastName    // 1
             , customer.CustomerID  // 2
             , customer.Email       // 3
             , customer.Phone       // 4
             ));

        base.Render(writer);
    }
    #endregion
}