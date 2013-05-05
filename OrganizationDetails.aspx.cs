using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OrganizationDetails : System.Web.UI.Page
{
    #region Fetching Data
    public List<ReportDataNode> FetchReportData()
    {
        // Create our query
        var query = ExigoApiContext.CreateODataContext().UniLevelTreePeriodVolumes
            .Where(c => c.TopCustomerID == Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Default)
            .Where(c => c.Period.IsCurrentPeriod);
        

        // Apply ordering and filtering
        var helper = new GridReportHelper();
        query = helper.ApplyFiltering<UniLevelNodePeriodVolume>(query);
        query = helper.ApplyOrdering<UniLevelNodePeriodVolume>(query);


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
                CustomerRank           = c.Customer.Rank.RankDescription,
                JoinedDate             = c.Customer.CreatedDate,
                Volume1                 = c.PeriodVolume.Volume1, //Current Month PCV
                Volume2                 = c.PeriodVolume.Volume3, //Current Month OCV
                Volume3                 = c.PeriodVolume.Volume75, //3 Month PCV
                Volume4                 = c.PeriodVolume.Volume79, //3 Month OCV
                Volume5                 = c.PeriodVolume.Volume56 //Home Transaction Credits
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
                        html.AppendFormat("<td>{0}</td>", record.NestedLevel);
                        html.AppendFormat(@"<td class='customerdetails'>
                                                <a href='Profile.aspx?id={0}' title='View profile'>{0}</a><br />
                                                <span class='title'>{1}</span>
                                            </td>", record.CustomerID,
                                                  record.CustomerRank);
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
                        html.AppendFormat("<td>{0:N0}</td>", record.Volume1);
                        html.AppendFormat("<td>{0:N0}</td>", record.Volume2);
                        html.AppendFormat("<td>{0:N0}</td>", record.Volume3);
                        html.AppendFormat("<td>{0:N0}</td>", record.Volume4);
                        html.AppendFormat("<td>{0:N0}</td>", record.Volume5);;

                        html.AppendFormat(@"
                                <td>
                                    <div class='btn-group pull-right'>
                                        <a href='CreateMessage.aspx?to={0}' class='btn'><i class='icon-envelope'></i></a>
                                        <a href='javascript:;' class='btn dropdown-toggle' data-toggle='dropdown'>
                                            <span class='caret'></span>
                                        </a>
                                        <ul class='dropdown-menu pull-right'>
                                            <li><a href='CreateMessage.aspx?to={0}'><i class='icon-envelope'></i>&nbsp;Email</a></li>
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
        public string CustomerRank { get; set; }
        public DateTime JoinedDate { get; set; }
        public decimal Volume1 { get; set; }
        public decimal Volume2 { get; set; }
        public decimal Volume3 { get; set; }
        public decimal Volume4 { get; set; }
        public decimal Volume5 { get; set; }

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