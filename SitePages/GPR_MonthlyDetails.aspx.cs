using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GPR_MonthlyDetails : System.Web.UI.Page
{
    #region Fetching Data
    public List<ReportDataNode> FetchReportData()
    {
        // Create our query
        var query = ExigoApiContext.CreateODataContext().UniLevelTreePeriodVolumes
            .Where(c => c.TopCustomerID == Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Monthly)
            .Where(c => c.Period.IsCurrentPeriod)
            .Where(c => c.PeriodVolume.Volume98 != 0);
        

        // Apply ordering and filtering
        var helper = new GridReportHelper();
        query = helper.ApplyFiltering<UniLevelNodePeriodVolume>(query);
        query = helper.ApplyOrdering<UniLevelNodePeriodVolume>(query);


        // Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
            {
                CustomerID      = c.CustomerID,
                Level           = c.Level,
                FirstName       = c.Customer.FirstName,
                LastName        = c.Customer.LastName,
                Company         = c.Customer.Company,

                CustomerType    = c.Customer.CustomerType.CustomerTypeDescription,
                CustomerStatus  = c.Customer.CustomerStatus.CustomerStatusDescription,
                CustomerRank    = c.Customer.Rank.RankDescription,

                VolumeColumn4   = c.PeriodVolume.Volume83,  // Monthly Personal GPR Credits
                VolumeColumn5   = c.PeriodVolume.Volume82,  // Lifetime Personal GPR Credits
                VolumeColumn6   = c.PeriodVolume.Volume100, // Monthly Organizaional GPR Credits
                VolumeColumn7   = c.PeriodVolume.Volume98,  // Lifetime Organizaional GPR Credits               

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
                                                <a href='../Profile.aspx?id={0}' title='View profile'>{0}</a><br />
                                                <span class='title'>{1}</span>
                                            </td>", record.CustomerID,
                                                  record.CustomerRank);
                        html.AppendFormat(@"
                                <td class='customerdetails'>
                                    <a href='../Profile.aspx?id={0}' title='View profile'>
                                        <img src='{1}' class='avatar' /></a>
                                    <span class='name'><a href='../Profile.aspx?id={0}' title='View profile'>{2}</a></span>
                                    <span class='title'>{3} {4}</span>
                                </td>", 
                            record.CustomerID,
                            GlobalUtilities.GetCustomerTinyAvatarUrl(record.CustomerID),
                            GlobalUtilities.Coalesce(record.Company, record.FirstName + " " + record.LastName),
                            record.CustomerStatus,
                            record.CustomerType);



                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn4);
                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn5);
                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn6);
                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn7);








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

        public string CustomerType { get; set; }
        public string CustomerStatus { get; set; }
        public string CustomerRank { get; set; }

        public decimal VolumeColumn4 { get; set; }
        public decimal VolumeColumn5 { get; set; }
        public decimal VolumeColumn6 { get; set; }
        public decimal VolumeColumn7 { get; set; }
        public decimal VolumeColumn8 { get; set; }
        public decimal VolumeColumn9 { get; set; }
        public decimal VolumeColumn10 { get; set; }

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