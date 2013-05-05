using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GamePlanReport_Weekly : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region Fetching Data

    public List<ReportDataNode> FetchReportData()
    {
        // Assemble the query for Personal Volumes
        var query = ExigoApiContext.CreateODataContext().PeriodVolumes
            .Where(c => c.CustomerID == 10005) // Identity.Current.CustomerID)
            .Where(c => c.Period.StartDate < DateTime.Now)
            .Where(c => c.PeriodTypeID == PeriodTypes.Weekly);


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
            VolumeColumn1          = c.Volume99,  // Personal GPR Credits Weekly
            VolumeColumn2          = c.Volume98,  // Personal GPR Credits in Lifetime
            AverageDownlineGPRsubmissions          = c.Volume75, // Total Game Plan Reports Submitted In This Week By Your Downline
            VolumeColumn6          = c.Volume100 // GPRR Credits Monthly
        }).Skip((helper.Page - 1) * helper.RecordCount).Take(helper.RecordCount).ToList();
       

        // Return the nodes
        return nodes;
    }

    public List<decimal> WeeklyList = new List<decimal>();
    public List<ReportDataNode> FetchUniLevelData()
    {
        //      Query the OData tables for UniLevel Volumes      //
        #region Query the OData tables for UniLevel Volumes
        var query = ExigoApiContext.CreateODataContext().UniLevelTreePeriodVolumes
            .Where(c => c.TopCustomerID == 10005) // Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Weekly)
            .Where(c => c.Period.IsCurrentPeriod)
            .Where(c => c.PeriodVolume.Volume99 != 0);
        #endregion Query the OData tables

        //      Fetch the nodes     //
        #region Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
        {
            CustomerID = c.CustomerID,
            PeriodID                        = c.PeriodID,
            PeriodTypeID                    = c.PeriodTypeID,
            PeriodDescription               = c.Period.PeriodDescription,
            StartDate                       = c.Period.StartDate,
            EndDate                         = c.Period.EndDate,
            PersonalGPsubmissionsForWeek    = c.PeriodVolume.Volume99, // Personal GPR Credits Weekly
            PersonalGPsubmissionsInLifetime = c.PeriodVolume.Volume98  // Personal GPR Credits in Lifetime
        }).ToList();
        #endregion Fetch the nodes

        //      Add values to the Weekly GPR List  //
        #region Add values to the Weekly GPR List
        foreach (var customer in nodes)
        {
            decimal theValueOfVolumeBucket99 = customer.PersonalGPsubmissionsForWeek;

            WeeklyList.Add(theValueOfVolumeBucket99);
        }
        #endregion

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
                    var uninodes = FetchUniLevelData();

                    // Assemble the records
                    var html = new StringBuilder();

                    // First, add our record count
                    html.AppendFormat("{0}^", nodes.Count());

                    foreach(var record in nodes)
                    {
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
        public int PeriodID { get; set; }
        public int PeriodTypeID { get; set; }
        public string PeriodDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string HighestRankAchieved { get; set; }
        public string PaidAsRank { get; set; }
        public decimal VolumeColumn1 { get; set; }
        public decimal VolumeColumn2 { get; set; }
        public decimal AverageDownlineGPRsubmissions { get; set; }
        public decimal PersonalGPsubmissionsInLifetime { get; set; }
        public decimal PersonalGPsubmissionsForWeek { get; set; }
        public decimal VolumeColumn6 { get; set; }
    }
    #endregion
}