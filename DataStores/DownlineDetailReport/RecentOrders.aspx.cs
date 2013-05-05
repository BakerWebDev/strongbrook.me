using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.WebService;

public partial class RecentOrdersData : Page
{
    public int OrderCount = 3;

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
        // Get the data
        var context = ExigoApiContext.CreateWebServiceContext();
        var orders = context.GetOrders(new GetOrdersRequest
        {
            CustomerID = Convert.ToInt32(Request.QueryString["id"]),
            BatchSize = OrderCount
        }).Orders;


        // Render it
        if (orders.Length == 0)
        {
            writer.Write("No recent order activity.");
        }
        else
        {
            int orderCounter = 0;
            foreach (OrderResponse order in orders.OrderByDescending(o => o.OrderDate))
            {
                if (orderCounter > 0)
                    writer.Write("<div class='Divider'></div>");

                writer.Write(string.Format(@"
                    <table width='100%' class='ColumnedData'>
                        <tr>
                            <td colspan='2'>
                                <strong><a href='#' title='View the invoice for this order'>Order# {0}</a></strong>
                            </td>
                        </tr>
                        <tr>
                            <td class='Description'>
                                Order Date:
                            </td>
                            <td class='Value'>
                                {1:MMMM d, yyyy}
                            </td>
                        </tr>
                        <tr>
                            <td class='Description'>
                                Status:
                            </td>
                            <td class='Value'>
                                {2}
                            </td>
                        </tr>
                        <tr>
                            <td class='Description'>
                                Total:
                            </td>
                            <td class='Value'>
                                {3:C}
                            </td>
                        </tr>
                        <tr>
                            <td class='Description'>
                                PQV:
                            </td>
                            <td class='Value'>
                                {4:N0}
                            </td>
                        </tr>
                    </table>
                ", order.OrderID,
                    order.OrderDate,
                    order.OrderStatus.ToString(),
                    order.Total,
                    order.BusinessVolumeTotal));

                orderCounter++;
            }
        }

        base.Render(writer);
    }
    #endregion
}