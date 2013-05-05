using Exigo.OData;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OrderHistory : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var bar = Request.QueryString["id"];
    }

    #region Fetching Data

    public List<ReportDataNode> FetchReportData()
    {
        var helper = new GridReportHelper();
        int custID;

        if(Request.QueryString["id"] != "undefined")
        {
            custID = Convert.ToInt32(Request.QueryString["id"]);
        }
        else
        {
            custID = Identity.Current.CustomerID;            
        }
        // Assemble the query
        var query = ExigoApiContext.CreateODataContext().Orders
            .Where(c => c.CustomerID == Convert.ToInt32(custID));
        


        // Apply ordering and filtering
        
        query = helper.ApplyFiltering<Order>(query);
        query = helper.ApplyOrdering<Order>(query);


        // Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
            {
                OrderID             = c.OrderID,
                OrderStatus         = c.OrderStatus.OrderStatusDescription,
                OrderType           = c.OrderType.OrderTypeDescription,
                OrderDate           = c.OrderDate,
                Total               = c.Total,
                BV                  = c.BusinessVolumeTotal,
                TrackingNumber      = c.TrackingNumber1
            }).Skip((helper.Page - 1) * helper.RecordCount).Take(helper.RecordCount).ToList();


        // Return the nodes
        return nodes;
    }
    #endregion

    #region Render
    protected override void Render(HtmlTextWriter writer)
    {
        if(Request.QueryString["action"] != null)
        {
            switch(Request.QueryString["action"])
            {
                case "fetch":
                    // Fetch the nodes
                    var nodes = FetchReportData();
                    
                    // Assemble the records
                    var html = new StringBuilder();

                    // First, add our record count
                    html.AppendFormat("{0}^", nodes.Count());
                    foreach(var record in nodes)
                    {
                        // Assemble our html
                        html.AppendFormat("<tr>");
                        html.AppendFormat("<td>{0}</td>", record.OrderID);
                        html.AppendFormat("<td>{0}</td>", record.OrderStatus);
                        html.AppendFormat("<td>{0}</td>", record.OrderType);
                        html.AppendFormat("<td>{0:M/d/yyyy}</td>", record.OrderDate);
                        html.AppendFormat("<td>{0:C}</td>", record.Total);
                        html.AppendFormat("<td>{0:N0}</td>", record.BV);
                        html.AppendFormat("<td>{0}</td>", record.TrackingNumber);
                        html.Append(string.Format(@"
                            <td class='column-alignright'>
                                <div class='btn-group'>
                                    <a class='btn' href='OrderInvoice.aspx?id={0}' target='_blank'>" + Resources.Shopping.View + @"</a>
                                </div>
                            </td>
                        ", record.OrderID));
                        html.AppendFormat("</tr>");
                    }

                    Response.Clear();
                    writer.Write(html.ToString());
                    Response.End();
                    break;


                default: 
                    base.Render(writer);
                    break;
            }
        }
        else 
        {
            base.Render(writer);
        }
    }
    #endregion

    #region Models
    public class ReportDataNode
    {
        public int OrderID { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public decimal BV { get; set; }
        public string TrackingNumber { get; set; }
    }
    #endregion
}