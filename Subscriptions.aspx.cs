using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Subscriptions : System.Web.UI.Page
{
    #region Render
    public void RenderSubscriptionHistory()
    {
        // Get the subscriptions first
        var subscriptions = ExigoApiContext.CreateODataContext().Subscriptions;

        // Get the customer's subscriptions next
        var customersubscriptions = ExigoApiContext.CreateODataContext().CustomerSubscriptions.Expand("SubscriptionStatus")
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Select(c => c);


        StringBuilder html = new StringBuilder();



        html.Append("<table class='table'>");

        // Table headers
        html.Append(@"
                        <tr>
                            <th>Description</th>
                            <th style='width: 15%;'>Status</th>
                            <th style='width: 20%;'>Expires</th>
                        </tr>
        ");

        foreach(var subscription in subscriptions)
        {
            var customersubscription = customersubscriptions.Where(c => c.SubscriptionID == subscription.SubscriptionID).FirstOrDefault();

            if(customersubscription != null)
            {
                html.Append(string.Format(@"
                        <tr>
                            <td>{0}</td>
                            <td>{1}</td>
                            <td>{2:M/d/yyyy}</td>
                        </tr>", subscription.SubscriptionDescription,
                              customersubscription.SubscriptionStatus.SubscriptionStatusDescription,
                              customersubscription.ExpireDate));
            }
            else
            {
                html.Append(string.Format(@"
                        <tr>
                            <td>{0}</td>
                            <td>Not Activated</td>
                            <td>--</td>
                        </tr>", subscription.SubscriptionDescription));
            }
        }

        html.Append("</table>");



        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    #endregion
}