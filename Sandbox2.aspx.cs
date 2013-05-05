using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Sandbox2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region Fetch GPR Data
    public List<decimal> list = new List<decimal>();

    public List<ReportDataNode> FetchReportData()
    {
        // Query the OData tables
        var query = ExigoApiContext.CreateODataContext().UniLevelTreePeriodVolumes
            .Where(c => c.TopCustomerID == 10005) // Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Weekly)
            .Where(c => c.Period.IsCurrentPeriod)
            .Where(c => c.PeriodVolume.Volume99 != 0);

        // Fetch the nodes
        var nodes = query.Select(c => new ReportDataNode
        {
            CustomerID = c.CustomerID,
            VolumeBucket7 = c.PeriodVolume.Volume7,  // Everyone should have a 1 here.
            VolumeBucket98  = c.PeriodVolume.Volume98, // GPRR Credits Lifetime
            VolumeBucket99  = c.PeriodVolume.Volume99, // GPRR Credits Weekly
            VolumeBucket100 = c.PeriodVolume.Volume100 // GPRR Credits Monthly
        }).ToList();

        #region Add the amount in each of the Unilevel customers Volume Bucket 98 to a the list.
        foreach (var customer in nodes)
        {
            decimal theValueOfVolumeBucket99 = customer.VolumeBucket99;

            list.Add(theValueOfVolumeBucket99);
        }
        #endregion Add the number of items in the Unilevel customers Volume Bucket 98 to a the list.

        // Return the nodes
        return nodes;
    }
    #endregion Fetch the Data

    #region Render GPR Data
    public void ArrayTest1()
    {
        var nodes = FetchReportData();

        #region Turn the list into an array, and get the average of those numbers.
        decimal[] listNumbersConverted = list.ToArray();

        decimal[] numbers = { listNumbersConverted[0] }; 
  
        decimal averageNum = numbers.Average();
        #endregion Turn the list into an array, and get the average of those numbers.

        var html = new StringBuilder();

        #region Render the count of nodes.
        html.AppendFormat(@"
            Count: {0}
            <br /><br />
            "
            , nodes.Count()
            );
        #endregion
        #region Render the number of GPR's for each person in UniLeval downline.
        foreach (var number in list)
        {
            html.AppendFormat(@"
                {0}
                "
                , number
                );
        }
        #endregion Render the number of GPR's for each person in UniLeval downline.
        #region Render the Average number of GPR's for UniLevel downline.
        html.AppendFormat(@"<br />
            The Average Number is: {0}
            <br /><br />
            "
            , averageNum.ToString()
            );
        #endregion Render the Average number of GPR's for UniLevel downline.

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());
    }

    public void Render_UniLevelDownlineGPR_Count()
    {
        var html = new StringBuilder();

        #region Render the count of nodes.
        var nodes = FetchReportData();

        html.AppendFormat(@"
            {0}
            "
            , nodes.Count()
            );
        #endregion

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());    
    }

    public void Render_UniLevelDownlineGPR_CountPerPerson()
    {
        var html = new StringBuilder();

        #region Render the number of GPR's for each person in UniLeval downline.
        var nodes = FetchReportData();

        foreach (var number in list)
        {
            html.AppendFormat(@"
                {0}
                "
                , number.ToString("N")
                );
        }
        #endregion Render the number of GPR's for each person in UniLeval downline.

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());    
    }

    public void Render_UniLevelDownlineGPR_Average()
    {
        var html = new StringBuilder();

        #region Render the Average number of GPR's for UniLevel downline.
        #region Turn the list into an array, and get the average of those numbers.
        decimal[] listNumbersConverted = list.ToArray();

        decimal[] numbers = { listNumbersConverted[0] }; 
  
        decimal averageNum = numbers.Average();
        #endregion Turn the list into an array, and get the average of those numbers.

        html.AppendFormat(@"<br />
            {0}
            "
            , averageNum.ToString("N")
            );
        #endregion Render the Average number of GPR's for UniLevel downline.

        var writer = new HtmlTextWriter(Response.Output);
        writer.Write(html.ToString());    
    }
    #endregion  Render GPR Data

    public class ReportDataNode
    {
        public int CustomerID { get; set; }
        public decimal VolumeBucket7 { get; set; }
        public decimal VolumeBucket98 { get; set; }
        public decimal VolumeBucket99 { get; set; }
        public decimal VolumeBucket100 { get; set; }
    }

}