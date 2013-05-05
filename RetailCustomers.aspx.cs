using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RetailCustomers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region Fetching Data
    public List<ReportDataNode> FetchReportData()
    {
        // Assemble the query
        var query = ExigoApiContext.CreateODataContext().EnrollerTree
            .Where(c => c.TopCustomerID == Identity.Current.CustomerID)
            .Where(c => c.Level == 1)
            .Where(c => c.Customer.CustomerTypeID == CustomerTypes.RetailCustomer);


        // Apply ordering and filtering
        var helper = new GridReportHelper();
        query = helper.ApplyFiltering<EnrollerNode>(query);
        query = helper.ApplyOrdering<EnrollerNode>(query);


        // Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
            {
                CustomerID             = c.CustomerID,
                Level                  = c.Level,
                FirstName              = c.Customer.FirstName,
                LastName               = c.Customer.LastName,
                Company                = c.Customer.Company,
                Email                  = c.Customer.Email,
                Phone                  = c.Customer.Phone,
                Phone2                 = c.Customer.Phone2,
                CustomerType           = c.Customer.CustomerType.CustomerTypeDescription,
                CustomerStatus         = c.Customer.CustomerStatus.CustomerStatusDescription,
                JoinedDate             = c.Customer.CreatedDate
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
                        html.AppendFormat("<td>{0} {1}</td>", record.FirstName, record.LastName);

                        var email = (!string.IsNullOrEmpty(record.Email)) ? string.Format("<i class='icon-envelope'></i>&nbsp;<a href='CreateMessage.aspx?to={0}' title='Send email'>{0}</a><br />", record.Email) : "";
                        var phone = (!string.IsNullOrEmpty(record.Phone)) ? string.Format("<i class='icon-home'></i>&nbsp;{0}<br />", record.Phone) : "";
                        var phone2 = (!string.IsNullOrEmpty(record.Phone2)) ? string.Format("<i class='icon-briefcase'></i>&nbsp;{0}", record.Phone2) : "";
                        html.AppendFormat(@"
                                <td>
                                    {0}
                                    {1}
                                    {2}
                                </td>
                            ", email,
                             phone,
                             phone2);

                        html.AppendFormat("<td>{0:M/d/yyyy}</td>", record.JoinedDate);

                        html.AppendFormat(@"
                                <td>
                                    <div class='btn-group pull-right'>
                                        <a href='CreateMessage.aspx?to={0}' class='btn'><i class='icon-envelope'></i></a>
                                        <a href='javascript:;' class='btn dropdown-toggle' data-toggle='dropdown'>
                                            <span class='caret'></span>
                                        </a>
                                        <ul class='dropdown-menu pull-right'>
                                            <li><a href='CreateMessage.aspx?to={0}'><i class='icon-envelope'></i>&nbsp;Email</a></li>

























                                            <li><a href=""OrderHistory.aspx?id={1}""{2}'>View order history</a></li>



























                                        </ul>
                                    </div>
                                </td>
                            ", record.Email
                             , record.CustomerID
                             , record.OpenInSeparateWindow
                             );

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
        public int Level { get; set; }
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
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string CustomerType { get; set; }
        public string CustomerStatus { get; set; }
        public DateTime JoinedDate { get; set; }
        public string OpenInSeparateWindow = "target='_blank'";

        public string NestedLevel
        {
            get
            {
                var result = "";
                for(var x = 0; x < this.Level; x++)
                {
                    result += ".";
                }
                return result + this.Level.ToString();
            }
        }
    }
    #endregion
}