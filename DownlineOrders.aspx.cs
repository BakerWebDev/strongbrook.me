using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DownlineOrders : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region Fetching Data
    public List<ReportDataNode> FetchReportData()
    {
        /*
        var report = new SqlReportDataHelper();
        var helper = new SqlHelper();

        var table = helper.GetTable(@"
                DECLARE @_page int = " + report.Page + @"
                DECLARE @_rowcount int = " + report.RowsPerPage + @"

                SELECT ud.CustomerID
	                , c.FirstName
	                , c.LastName
	                , c.Company
	                , c.Email
                    , ct.CustomerTypeDescription
                    , cs.CustomerStatusDescription
		            , o.OrderID
                    , os.OrderStatusDescription
		            , o.Total
		            , o.BusinessVolumeTotal
		            , o.OrderDate
                FROM
	                Orders o
	                INNER JOIN UniLevelDownline ud
		                ON ud.CustomerID = o.CustomerID
	                INNER JOIN Customers c
		                ON c.CustomerID = ud.CustomerID
                    INNER JOIN CustomerTypes ct
                        ON ct.CustomerTypeID = c.CustomerTypeID
                    INNER JOIN CustomerStatuses cs
                        ON cs.CustomerStatusID = c.CustomerStatusID     
                    INNER JOIN OrderStatuses os
		                ON os.OrderStatusID = o.OrderStatusID          
                WHERE
                    " + report.WhereClause + @"
	                ud.DownlineCustomerID = {0}
                    AND o.OrderID > GETDATE() - 14
                ORDER BY
	                " + report.OrderByClause + @"

                OFFSET (@_page - 1) * @_rowcount ROWS
                FETCH NEXT @_rowcount ROWS ONLY
            ", 
                    Identity.Current.CustomerID,
                    PeriodTypes.Default,
                    GlobalUtilities.GetCurrentPeriodID());


        // Assemble the nodes
        var nodes = new List<ReportDataNode>();            
        foreach(DataRow row in table.Rows)
        {
            var node = new ReportDataNode();

            node.CustomerID             = Convert.ToInt32(row["CustomerID"]);
            node.FirstName              = row["FirstName"].ToString();
            node.LastName               = row["LastName"].ToString();
            node.Company                = row["Company"].ToString();
            node.Email                  = row["Email"].ToString();
            node.CustomerType           = row["CustomerTypeDescription"].ToString();
            node.CustomerStatus         = row["CustomerStatusDescription"].ToString();
            node.OrderID                = Convert.ToInt32(row["OrderID"]);
            node.OrderStatus            = row["OrderStatusDescription"].ToString();
            node.OrderDate              = Convert.ToDateTime(row["OrderDate"]);
            node.Total                  = Convert.ToDecimal(row["Total"]);
            node.BV                     = Convert.ToDecimal(row["BusinessVolumeTotal"]);

            nodes.Add(node);
        }
        */

        // Return the nodes
        return new List<ReportDataNode>();
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
                        html.AppendFormat(@"
                                <td class='customerdetails'>
                                    <a href='Profile.aspx?id={0}' title='View profile'>
                                        <img src='{1}' class='avatar' /></a>
                                    <span class='name'><a href='Profile.aspx?id={0}' title='View profile'>{2}</a></span>
                                    <span class='title'>{3} {4}</span>
                                </td>", 
                            record.CustomerID,
                            GlobalUtilities.GetCustomerTinyAvatarUrl(record.CustomerID),
                            GlobalUtilities.Coalesce(record.Company, record.FirstName + " " + record.LastName),
                            record.CustomerStatus,
                            record.CustomerType);

                        html.AppendFormat("<td>{0:C}</td>", record.Total);
                        html.AppendFormat("<td>{0:N0}</td>", record.BV);
                        html.AppendFormat("<td>{0:M/d/yyyy}</td>", record.OrderDate);

                        html.AppendFormat(@"
                                <td>
                                    <div class='btn-group pull-right'>
                                        <a href='CreateMessage.aspx?to={0}' class='btn'><i class='icon-envelope'></i></a>
                                        <a href='javascript:;' class='btn dropdown-toggle' data-toggle='dropdown'>
                                            <span class='caret'></span>
                                        </a>
                                        <ul class='dropdown-menu pull-right'>
                                            <li><a href='CreateMessage.aspx?to={0}'><i class='icon-envelope'></i>&nbsp;Email</a></li>
                                            <li><a href='Profile.aspx?id={1}'>View profile</a></li>
                                            <li><a href='OrderHistory.aspx?id={1}'>View order history</a></li>
                                        </ul>
                                    </div>
                                </td>
                            ", record.Email,
                             record.CustomerID);

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
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string DisplayName
        {
            get
            {
                return GlobalUtilities.Coalesce(this.Company, this.FirstName + " " + this.LastName);
            }
        }
        public string Email { get; set; }
        public string CustomerType { get; set; }
        public string CustomerStatus { get; set; }
        public int OrderID { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public decimal BV { get; set; }
    }
    #endregion
}