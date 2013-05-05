using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GamePlanReport_Monthly : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region Fetching Data
    public List<ReportDataNode> FetchReportData()
    {
        // Assemble the query
        var query = ExigoApiContext.CreateODataContext().PeriodVolumes
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Where(c => c.Period.StartDate < DateTime.Now)
            .Where(c => c.PeriodTypeID == PeriodTypes.Monthly);


        // Apply ordering and filtering
        var helper = new GridReportHelper();
        query = helper.ApplyFiltering<PeriodVolume>(query);
        query = helper.ApplyOrdering<PeriodVolume>(query);


        // Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
        {
            
            PeriodID               = c.PeriodID,
            PeriodTypeID           = c.PeriodTypeID,
            PeriodDescription      = c.Period.PeriodDescription,
            StartDate              = c.Period.StartDate,
            EndDate                = c.Period.EndDate,
            HighestRankAchieved    = c.Rank.RankDescription,
            PaidAsRank             = c.PaidRank.RankDescription,
            VolumeColumn1          = c.Volume1,  // Current Month PCV
            VolumeColumn2          = c.Volume3,  // Current Month OCV
            VolumeColumn3          = c.Volume75, // 3 Month PCV
            VolumeColumn4          = c.Volume98, // GPRR Credits Lifetime
            VolumeColumn5          = c.Volume99, // GPRR Credits Weekly
            VolumeColumn6          = c.Volume100 // GPRR Credits Monthly
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
                        html.AppendFormat(@"
                                <td>
                                    <strong>{0}</strong><br />
                                    <small>{1:M/d/yyyy} - {2:M/d/yyyy}</small>
                                </td>"
                            , record.PeriodDescription,
                            record.StartDate,
                            record.EndDate);






                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn1);
                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn2);
                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn3);
                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn4);
                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn5);
                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn6);












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

        public int PeriodID { get; set; }
        public int PeriodTypeID { get; set; }
        public string PeriodDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string HighestRankAchieved { get; set; }
        public string PaidAsRank { get; set; }
        public decimal VolumeColumn1 { get; set; }
        public decimal VolumeColumn2 { get; set; }
        public decimal VolumeColumn3 { get; set; }
        public decimal VolumeColumn4 { get; set; }
        public decimal VolumeColumn5 { get; set; }
        public decimal VolumeColumn6 { get; set; }
    }
    #endregion
}