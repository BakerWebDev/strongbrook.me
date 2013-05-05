using Exigo.OData;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CommissionBonusDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    #region Properties
    // Returns 0 if real time commission, 1 if prior commission.
    public CommissionPeriodType CommissionType
    {
        get
        {
            CommissionPeriodType type = CommissionPeriodType.Current;
            if (Request.QueryString["type"] != null)
            {
                type = (CommissionPeriodType)Enum.Parse(typeof(CommissionPeriodType), Request.QueryString["type"]);
            }
            return type;
        }
    }
    #endregion

    #region Fetching Data
    public List<ReportDataNode> FetchPriorCommissionBonusReportData()
    {
        var report = new SqlReportDataHelper();

        var commissionRunID = Convert.ToInt32(Request.QueryString["rid"]);
        var bonusID = Convert.ToInt32(Request.QueryString["bid"]);


        // Assemble the query
        var query = ExigoApiContext.CreateODataContext().CommissionDetails
            .Where(c => c.CommissionRunID == commissionRunID)
            .Where(c => c.BonusID == bonusID)
            .Where(c => c.CustomerID == Identity.Current.CustomerID);


        // Apply ordering and filtering
        var helper = new GridReportHelper();
        query = helper.ApplyOrdering<CommissionDetail>(query);

                
        // Get the nodes
        var nodes = query.Select(c => new ReportDataNode
            {
                FromCustomerID = c.FromCustomerID,
                FromCustomerName = GlobalUtilities.Coalesce(c.FromCustomer.Company, c.FromCustomer.FirstName + " " + c.FromCustomer.LastName),
                OrderID = c.OrderID,
                Level = c.Level,
                PaidLevel = c.PaidLevel,
                SourceAmount = c.SourceAmount,
                Percentage = c.Percentage,
                CommissionAmount = c.CommissionAmount
            }).ToList(); 
        

        // Return the nodes
        return nodes;
    }
    public List<ReportDataNode> FetchCurrentCommissionBonusReportData()
    {
        // Get the data
        var customerID = Identity.Current.CustomerID;
        var periodTypeID = Convert.ToInt32(Request.QueryString["ptid"]);
        var periodID = Convert.ToInt32(Request.QueryString["pid"]);
        var bonusID = Convert.ToInt32(Request.QueryString["bid"]);


        // We have to get this data from the web service because it is real-time.
        // This call can be slow because it has to fetch all the rows every call.
        var data = ExigoApiContext.CreateWebServiceContext().GetRealTimeCommissionDetail(new GetRealTimeCommissionDetailRequest
        {
            CustomerID = customerID,
            BonusID = bonusID,
            PeriodID = periodID,
            PeriodType = periodTypeID
        });


        // If we somehow didn't get any records back, stop here.
        if(data == null) return new List<ReportDataNode>();


        // Create a new list of our reporting nodes based on the nodes we got back from the web service.
        // In other words, convert their collection into one of our own.
        var nodes = data.CommissionDetails.ToList().Select(c => new ReportDataNode
        {
            FromCustomerID = c.FromCustomerID,
            FromCustomerName = c.FromCustomerName,
            OrderID = c.OrderID,
            Level = c.Level,
            PaidLevel = c.PaidLevel,
            SourceAmount = c.SourceAmount,
            Percentage = c.Percentage,
            CommissionAmount = c.CommissionAmount
        });


        // Order the nodes, since the web service can't do it. THIS IS A MUST FOR PAGINATION!
        var orderedNodes = nodes.OrderBy(c => c.CommissionAmount);


        // Return the records we need, taking pagination into account.
        return orderedNodes.ToList();
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
                    var nodes = (CommissionType == CommissionPeriodType.Current) ? FetchCurrentCommissionBonusReportData() : FetchPriorCommissionBonusReportData();
                    
                    // Assemble the records
                    var html = new StringBuilder();

                    // First, add our record count
                    html.AppendFormat("{0}^", nodes.Count());
                    foreach(var record in nodes)
                    {
                        // Assemble our html
                        html.AppendFormat("<tr>");
                        html.AppendFormat("<td>{0}</td>", record.FromCustomerID);
                        html.AppendFormat("<td>{0}</td>", record.FromCustomerName);

                        var orderDisplay = (record.OrderID != 0) ? string.Format("<a href='OrderInvoice.aspx?id={0}' target='_blank'>{0}</a>", record.OrderID) : "---";
                        html.AppendFormat("<td>{0}</td>", orderDisplay);

                        html.AppendFormat("<td>{0}</td>", record.Level);
                        html.AppendFormat("<td>{0}</td>", record.PaidLevel);

                        html.AppendFormat("<td>{0:C}</td>", record.SourceAmount);
                        html.AppendFormat("<td>{0:C} ({1:N0}%)</td>", record.CommissionAmount, record.Percentage);
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
        public int FromCustomerID { get; set; }
        public string FromCustomerName { get; set; }
        public int OrderID { get; set; }
        public int Level { get; set; }
        public int PaidLevel { get; set; }
        public decimal SourceAmount { get; set; }
        public decimal Percentage { get; set; }
        public decimal CommissionAmount { get; set; }
    }
    #endregion

    #region Helper Methods & Enums
    public enum CommissionPeriodType
    {
        Current = 0,
        Prior = 1
    }
    #endregion
}