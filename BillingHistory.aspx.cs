using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BillingHistory : System.Web.UI.Page
{
    #region Render
    public void RenderBillingHistory()
    {
        // Get the subscriptions first
        var orders = ExigoApiContext.CreateWebServiceContext().GetOrders(new GetOrdersRequest
        {
            CustomerID = Identity.Current.CustomerID
        });


        var html = new StringBuilder();

        html.Append("<table class='table table-condensed'>");

        // Table headers
        html.Append(@"
                        <tr>
                            <th style='width: 15%;'>Date</th>
                            <th>Description</th>
                            <th style='width: 10%;'>Charges</th>
                            <th style='width: 10%;'>Payments</th>
                            <th style='width: 10%;'>Receipt</th>
                        </tr>
        ");

        // Get the most recent activity item
        if(orders.RecordCount == 0)
        {
            html.Append(@"<tr>
                            <td colspan='6'>No orders or payments at this time.</td>
                        </tr>");
        }
        else
        {
            var sortedorders = orders.Orders.OrderByDescending(c => c.OrderDate);

            foreach(var order in sortedorders)
            {
                if(order.Payments.Length > 0)
                {
                    foreach(var payment in order.Payments)
                    {
                        string paymentdescription = string.Empty;
                        switch(payment.PaymentType)
                        {
                            case PaymentType.ACHDebit:          paymentdescription = "ACH/Debit payment"; break;
                            case PaymentType.BankDeposit:       paymentdescription = "Bank deposit payment"; break;
                            case PaymentType.BankDraft:         paymentdescription = "Bank draft payment"; break;
                            case PaymentType.BankWire:          paymentdescription = "Bank wire payment"; break;
                            case PaymentType.Cash:              paymentdescription = "Cash payment"; break;
                            case PaymentType.Check:             paymentdescription = "Check payment"; break;
                            case PaymentType.COD:               paymentdescription = "Payment via COD"; break;
                            case PaymentType.CreditCard:        paymentdescription = "Payment using card ending in " + payment.CreditCardNumberDisplay; break;
                            case PaymentType.MoneyOrder:        paymentdescription = "Money order payment"; break;
                            case PaymentType.PointRedemtion:    paymentdescription = "Point redemption"; break;
                            case PaymentType.UseCredit:         paymentdescription = "Payment via account credits"; break;
                        }

                        html.Append(string.Format(@"
                            <tr>
                                <td>{0:M/d/yyyy}</td>
                                <td>{1}</td>
                                <td>&nbsp;</td>
                                <td>{2}</td>
                                <td><a href='OrderInvoice.aspx?id={3}' target='_blank'>View</a></td>
                            </tr>", payment.PaymentDate,
                                  paymentdescription,
                                  (payment.Amount >= 0) ? string.Format("({0:C})", payment.Amount) : string.Format("{0:C}", payment.Amount),
                                  payment.OrderID));
                    }
                }
                
                string orderdescription = "Order #" + order.OrderID + "<ul>";
                foreach(var detail in order.Details)
                {
                    orderdescription += "<li>" + detail.Description + "</li>";
                }
                orderdescription += "</ul>";

                html.Append(string.Format(@"
                            <tr>
                                <td>{0:M/d/yyyy}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>", order.OrderDate,
                                  orderdescription,
                                  (order.Total >= 0) ? string.Format("{0:C}", order.Total) : string.Format("({0:C})", order.Total)));
            }
        }

        html.Append("</table>");


        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }
    #endregion
}