using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Sandbox : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    //    #region Fetching Data
    //    public List<ReportDataNode> FetchReportData()
    //    {
    //        // Assemble the query
    //        var query = ExigoApiContext.CreateODataContext().PeriodVolumes
    //            .Where(c => c.CustomerID == Identity.Current.CustomerID)
    //            .Where(c => c.Period.StartDate < DateTime.Now)
    //            .Where(c => c.PeriodTypeID == PeriodTypes.Weekly);


    //        // Apply ordering and filtering
    //        var helper = new GridReportHelper();
    //        query = helper.ApplyFiltering<PeriodVolume>(query);
    //        query = helper.ApplyOrdering<PeriodVolume>(query);


    //        // Fetch the nodes
    //        var nodes = query.Select(c => new ReportDataNode
    //        {

    //            PeriodID               = c.PeriodID,
    //            PeriodTypeID           = c.PeriodTypeID,
    //            PeriodDescription      = c.Period.PeriodDescription,
    //            StartDate              = c.Period.StartDate,
    //            EndDate                = c.Period.EndDate,
    //            HighestRankAchieved    = c.Rank.RankDescription,
    //            PaidAsRank             = c.PaidRank.RankDescription,
    //            VolumeColumn1          = c.Volume1,  // Current Month PCV
    //            VolumeColumn2          = c.Volume3,  // Current Month OCV
    //            VolumeColumn3          = c.Volume75, // 3 Month PCV
    //            VolumeColumn4          = c.Volume98, // GPRR Credits Lifetime
    //            VolumeColumn5          = c.Volume99, // GPRR Credits Weekly
    //            VolumeColumn6          = c.Volume100 // GPRR Credits Monthly
    //        }).Skip((helper.Page - 1) * helper.RecordCount).Take(helper.RecordCount).ToList();


    //        // Return the nodes
    //        return nodes;
    //    }
    //    #endregion

    //    #region Render
    //    protected override void Render(HtmlTextWriter writer)
    //    {
    //        if(Request.QueryString["action"] != null)
    //        {
    //            switch(Request.QueryString["action"])
    //            {
    //                case "fetch":
    //                    // Fetch the nodes
    //                    var nodes = FetchReportData();


    //                    // Assemble the records
    //                    var html = new StringBuilder();

    //                    // First, add our record count
    //                    html.AppendFormat("{0}^", nodes.Count());
    //                    foreach(var record in nodes)
    //                    {
    //                        // Assemble our html
    //                        html.AppendFormat("<tr>");
    //                        html.AppendFormat(@"
    //                                <td>
    //                                    <strong>{0}</strong><br />
    //                                    <small>{1:M/d/yyyy} - {2:M/d/yyyy}</small>
    //                                </td>"
    //                            , record.PeriodDescription,
    //                            record.StartDate,
    //                            record.EndDate);






    //                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn4);
    //                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn5);
    //                        html.AppendFormat("<td>{0:N0}</td>", record.VolumeColumn6);












    //                        html.AppendFormat("</tr>");
    //                    }

    //                    Response.Clear();
    //                    writer.Write(html.ToString());
    //                    Response.End();
    //                    break;


    //                default: 
    //                    base.Render(writer);
    //                    break;
    //            }
    //        }
    //        else 
    //        {
    //            base.Render(writer);
    //        }
    //    }
    //    #endregion

    //    #region Models
    //    public class ReportDataNode
    //    {

    //        public int PeriodID { get; set; }
    //        public int PeriodTypeID { get; set; }
    //        public string PeriodDescription { get; set; }
    //        public DateTime StartDate { get; set; }
    //        public DateTime EndDate { get; set; }
    //        public string HighestRankAchieved { get; set; }
    //        public string PaidAsRank { get; set; }
    //        public decimal VolumeColumn1 { get; set; }
    //        public decimal VolumeColumn2 { get; set; }
    //        public decimal VolumeColumn3 { get; set; }
    //        public decimal VolumeColumn4 { get; set; }
    //        public decimal VolumeColumn5 { get; set; }
    //        public decimal VolumeColumn6 { get; set; }
    //    }
    //    #endregion



    //public void Linq89()
    //{
    //    var html = new StringBuilder();

    //    int[] numbers = { anArrayOfNumbers() }; 
  
    //    double averageNum = numbers.Average();

    //    // Render the results  
    //    html.AppendFormat("{0}", averageNum);
    //    var writer = new HtmlTextWriter(Response.Output);
    //    writer.Write(html.ToString());
    //}








    //public int theNumber;

    //public int anArrayOfNumbers()
    //{
        
    //    foreach(var number in list)
    //    {
    //        string[] a = number.Split('|');

    //        theNumber = Convert.ToInt32(a[0]);
    //    }  
    //    return theNumber;
    //}

    public List<int> list = new List<int>();

    public void ArrayTest1()
    {
        var html = new StringBuilder();

        #region Fetch the Data
        // Query the OData tables
        var query = ExigoApiContext.CreateODataContext().UniLevelTreePeriodVolumes
            .Where(c => c.TopCustomerID == Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Default)
            .Where(c => c.Period.IsCurrentPeriod)
            .Where(c => c.PeriodVolume.Volume99 != 0);

        // Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
        {
            CustomerID = c.CustomerID,
            VolumeBucket7 = c.PeriodVolume.Volume7,  // Everyone should have a 1 here.
            //VolumeBucket98  = c.PeriodVolume.Volume98, // GPRR Credits Lifetime
            VolumeBucket99  = c.PeriodVolume.Volume99, // GPRR Credits Weekly
            //VolumeBucket100 = c.PeriodVolume.Volume100 // GPRR Credits Monthly
        }).ToList();
        #endregion Fetch the Data

        #region Add the amount in each of the Unilevel customers Volume Bucket 7 to a the list.
        foreach (var customer in nodes)
        {
            decimal theValueOfVolumeBucket7 = customer.VolumeBucket99;

            int sv = Convert.ToInt32(theValueOfVolumeBucket7);

            list.Add(sv);
        }
        #endregion Add the number of items in the Unilevel customers Volume Bucket 7 to a the list.


        // Turn the list into an array.
        int[] fdas = list.ToArray();

        decimal[] numbers = { fdas[0] }; 
  
        decimal averageNum = numbers.Average();



        #region Render the results
        html.AppendFormat(@"
            Count: {0}
            <br /><br />
            "
            , nodes.Count()
            );





        foreach (var number in list)
        {
            ////Take the customerID
            //string customerID = customer.CustomerID.ToString();

            ////Split today's date by the "/" and store each part in array
            //string[] a = customerID.Split('/');

            ////Assemble a new date as all parts of the date from array int MMDDYYYY format
            //string date = a[0];

            //string[] asdf = date.Split('|');

            html.AppendFormat(@"
                {0}
                "
                , number                                  // 0 - CustomerID
                );



        }

        html.AppendFormat(@"<br />
            The Average Number is: {0}
            <br /><br />
            "
            , averageNum.ToString()
            );


        //html.AppendFormat("{0}", list);

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
        #endregion  Render the results
    }

    public class ReportDataNode
    {
        public int CustomerID { get; set; }
        public decimal VolumeBucket7 { get; set; }
        public decimal VolumeBucket98 { get; set; }
        public decimal VolumeBucket99 { get; set; }
        public decimal VolumeBucket100 { get; set; }
    }


}