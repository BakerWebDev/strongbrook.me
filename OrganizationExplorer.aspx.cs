using Exigo.OData;
using Exigo.RankQualificationGoals;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OrganizationExplorer : System.Web.UI.Page
{
    private int periodTypeID                        = PeriodTypes.Default; //The period type ID used to fetch the downline.
    private int[] allowedCustomerTypeIDs            = new int[] {  }; // The only customer types allowed to display in the report. If you don't want to filter by customer type, leave the array empty.
    private int maxNestedLevelDisplayLength         = 10; // The maximum number of periods to show before a nested level.
    private int maxRecentOrdersCount                = 3; // The maximum number of recent orders to display for a customer.
    private int maxAutoshipsCount                   = 99; // The maximum number of autoships to display for a customer.
    private int maxWallItemsCount                   = 10; // The maximum number of wall items to display for a customer.





    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    #endregion Page Load

    #region Render
    protected override void Render(HtmlTextWriter writer)
    {
        if (Request.QueryString["datakey"] != null)
        {
            Response.Clear();

            int id = Convert.ToInt32(Request.QueryString["id"]);
            switch (Request.QueryString["datakey"])
            {
                case "downlinelist":
                    int page = Convert.ToInt32(Request.QueryString["page"]);
                    RenderDownlineList(writer, id, page);
                    break;
                case "summary":
                    RenderCustomerSummary(writer, id);
                    break;
                case "options":
                    RenderCustomerOptions(writer, id);
                    break;
                case "businesssnapshot":
                    RenderBusinessSnapshot(writer, id);
                    break;
                case "orders":
                    RenderRecentOrders(writer, id);
                    break;
                case "autoships":
                    RenderAutoships(writer, id);
                    break;
                case "wall":
                    RenderCustomerWallFromSql(writer, id);
                    break;
                default:
                    return;
            }

            Response.End();
        }
        else
        {
            base.Render(writer);
        }
    }
    #endregion Render

    #region Downline List
    private void RenderDownlineList(HtmlTextWriter writer, int enrollerID, int page)
    {
        var data = FetchEnrollerDownline(enrollerID, page);


        if (data.Count() == 0)
        {
            return;
        }


        foreach (var node in data)
        {
            writer.Write(@"
                <tr>
                    <td>" + GetNestedLevelDisplay(node.Level) + @"</td>
                    <td>
                        <a href=""javascript:loadCustomerSections(" + node.CustomerID + @");"">
                            " + node.Customer.FirstName + " " + node.Customer.LastName + @"
                        </a>
                    </td>
                    <td>
                        <a href=""javascript:loadCustomerSections(" + node.CustomerID + @");"">
                            " + node.CustomerID + @"
                        </a>
                    </td>
                </tr>"
                );
        }
    }

    private List<EnrollerNode> FetchEnrollerDownline(int enrollerID, int page)
    {
        return (from n in ExigoApiContext.CreateODataContext().EnrollerTree
                where n.TopCustomerID == enrollerID
                select new EnrollerNode()
                {
                    CustomerID = n.CustomerID,
                    Customer = n.Customer,
                    Level = n.Level
                }).Skip((page - 1) * 50).Take(50).ToList();
    }

    private string GetNestedLevelDisplay(int level)
    {
        string nestedString = string.Empty;

        for (int x = 0; x < maxNestedLevelDisplayLength; x++)
        {
            if (level <= x) break;

            nestedString = nestedString + ".";
        }

        return nestedString + level;
    }
    #endregion

    #region Customer Summary
    private void RenderCustomerSummary(HtmlTextWriter writer, int customerID)
    {
        var html = new StringBuilder();


        // Get the data
        var context = ExigoApiContext.CreateODataContext();
        var customer = (from c in context.Customers
                        where c.CustomerID == customerID
                        select new
                        {
                            c.CustomerID,
                            c.FirstName,
                            c.LastName,
                            c.Company,
                            c.Rank.RankDescription,
                            c.CreatedDate,
                            c.MainAddress1,
                            c.MainAddress2,
                            c.MainCity,
                            c.MainState,
                            c.MainZip,
                            c.MainCountry,
                            c.Email,
                            c.Phone,
                            c.Phone2,
                            c.MobilePhone,
                            c.Fax
                        }).FirstOrDefault();

        if (customer == null) writer.Write("No profile found.");


        // Set up some variables
        var displayName = GlobalUtilities.Coalesce(customer.Company, customer.FirstName + " " + customer.LastName);


        // Compile the Html
        html.AppendFormat("<div class='row-fluid'>");
        html.AppendFormat("<span class='span3'>");
        html.AppendFormat("<img src='{0}' class='avatar' />", GlobalUtilities.GetCustomerLargeAvatarUrl(customer.CustomerID));
        html.AppendFormat("</span>");

        html.AppendFormat("<span class='span9'>");
        html.AppendFormat("<div class='details'>");
        html.AppendFormat("<div class='name'>{0}</div>", displayName);
        html.AppendFormat("<div class='rank'>{0}</div>", customer.RankDescription);
        html.AppendFormat("<div class='id'>ID# {0}</div>", customer.CustomerID);
        html.AppendFormat("<div class='socialnetworks'>");
        html.AppendFormat("<a href='javascript:;' title=\"Visit {0}'s Facebook page\"><img src='Assets/Images/icnFacebook.png' /></a>&nbsp;", displayName);
        html.AppendFormat("<a href='javascript:;' title=\"Visit {0}'s Twitter page\"><img src='Assets/Images/icnTwitter.png' /></a>", displayName);
        html.AppendFormat("</div>");
        html.AppendFormat("<div class='date'>Distributor since<br />{0:M/d/yyyy}</div>", customer.CreatedDate);
        html.AppendFormat("</div>");
        html.AppendFormat("<hr />");

        html.AppendFormat("<div class='row-fluid'>");
        html.AppendFormat("<span class='span6'>");
        html.AppendFormat("<p>");
        html.AppendFormat("{0}<br />", customer.MainAddress1);
        if(!string.IsNullOrEmpty(customer.MainAddress2)) html.AppendFormat("{0}<br />", customer.MainAddress2);
        html.AppendFormat("{0}, {1} {2}<br />", customer.MainCity, customer.MainState, customer.MainZip);
        html.AppendFormat("{0}", customer.MainCountry);
        html.AppendFormat("</p>");
        html.AppendFormat("</span>");

        html.AppendFormat("<span class='span6'>");
        html.AppendFormat("<p>");
        if(!string.IsNullOrEmpty(customer.Email)) html.AppendFormat("<i class='icon-envelope'></i>&nbsp;<a href='mailto:{0}' title='Send {1} an email'>{0}</a><br />", customer.Email, displayName);
        if(!string.IsNullOrEmpty(customer.Phone)) html.AppendFormat("<i class='icon-home'></i>&nbsp;{0}<br />", customer.Phone);
        if(!string.IsNullOrEmpty(customer.Phone2)) html.AppendFormat("<i class='icon-briefcase'></i>&nbsp;{0}<br />", customer.Phone2);
        if(!string.IsNullOrEmpty(customer.MobilePhone)) html.AppendFormat("<i class='icon-signal'></i>&nbsp;{0}<br />", customer.MobilePhone);
        if(!string.IsNullOrEmpty(customer.Fax)) html.AppendFormat("<i class='icon-print'></i>&nbsp;{0}", customer.Fax);
        html.AppendFormat("</p>");
        html.AppendFormat("</span>");
        html.AppendFormat("</div>");

        html.AppendFormat("</span>");
        html.AppendFormat("</div>");

        html.AppendFormat(@"
                <script>
                    // All tooltips
                    $('a[title]:not([data-tooltip=false])').tooltip({{
                        html: true,
                        placement: 'top'
                    }});
                </script>
            ");


        writer.Write(html.ToString());
    }
    #endregion Customer Summary

    #region Customer Options
    public void RenderCustomerOptions(HtmlTextWriter writer, int customerID)
    {
        // Use tasks to get the data
        var tasks = new List<Task>();
        var email = string.Empty;
        var webalias = string.Empty;


        // Get the customer's email address
        tasks.Add(Task.Factory.StartNew(() => {
            email = ExigoApiContext.CreateODataContext().Customers
                .Where(c => c.CustomerID == customerID)
                .Select(c => new {
                    c.Email
                })
                .SingleOr(new {
                    Email = string.Empty
                })
                .Email;
        }));


        // Get the customer's web alias
        tasks.Add(Task.Factory.StartNew(() => {
            try
            {
                webalias = ExigoApiContext.CreateWebServiceContext().GetCustomerSite(new GetCustomerSiteRequest
                {
                    CustomerID = customerID
                }).WebAlias;
            }
            catch {  }
        }));


        // Wait for all tasks to complete before proceeding
        Task.WaitAll(tasks.ToArray());
        tasks.Clear();


        // Compile the Html
        var html = new StringBuilder();
        if(!string.IsNullOrEmpty(webalias)) html.AppendFormat("<li><a href='{0}' target='_blank'><i class='icon-share'></i>&nbsp;Visit website</a></li>", GlobalUtilities.GetReplicatedSiteUrl(webalias));
        html.AppendFormat("<li><a href='CreateMessage.aspx?to={0}'><i class='icon-comment'></i>&nbsp;Send message</a></li>", customerID);
        if(!string.IsNullOrEmpty(email)) html.AppendFormat("<li><a href='mailto:{0}'><i class='icon-envelope'></i>&nbsp;Send email</a></li>", email);
        if(!string.IsNullOrEmpty(webalias)) html.AppendFormat("<li><a href='{0}' target='_blank'><i class='icon-globe'></i>&nbsp;Enroll new</a></li>", GlobalUtilities.GetSignupUrl(webalias));


        writer.Write(html.ToString());
    }
    #endregion

    #region Business Snapshot
    private void RenderBusinessSnapshot(HtmlTextWriter writer, int customerID)
    {
        var html = new StringBuilder();
        var service = new RankQualificationGoalsService();


        // Get the data
        try
        {
            var rankID = 1;
            var rank = ExigoApiContext.CreateODataContext().Customers
                .Where(c => c.CustomerID == customerID)
                .Select(c => new { c.RankID })
                .FirstOrDefault();
            if (rank != null) rankID = rank.RankID;


            // Compile the Html
            var response = service.GetRankQualificationGoals(customerID, rankID);
            var qualifications = response.RankQualifications;
            var decimalQualifications = qualifications.Where(c => c.IsBoolean == false).ToList();
            var totalPercentComplete = response.TotalPercentComplete;

            
            html.AppendFormat("<div class='progress-upperlabel progress-upperlabel-large'>{0}</div>", response.RankDescription);
            html.AppendFormat("<div class='progress progress-info progress-striped active'>");
            html.AppendFormat("<div class='bar' style='width: {0:0}%'></div>", totalPercentComplete);
            html.AppendFormat("</div>");
            html.AppendFormat("<div class='progress-lowerlabel'>{0:0}% Complete</div>", totalPercentComplete);
            
            html.AppendFormat("<hr />");


            foreach (var qual in decimalQualifications)
            {
                html.AppendFormat("<div class='progressbarwrapper'>");
                html.AppendFormat("<div class='progress-upperlabel'>{0}</div>", qual.Label);
                html.AppendFormat("<div class='progress progress-thin progress-warning'>");
                html.AppendFormat("<div class='bar' style='width: {0:0}%'></div>", qual.RequiredToActualAsPercent);
                html.AppendFormat("</div>");
                html.AppendFormat("<div class='progress-lowerlabel pull-left'>{0:N0} / {1:N0}</div>", qual.ActualValueAsDecimal, qual.RequiredValueAsDecimal);
                html.AppendFormat("<div class='progress-lowerlabel pull-right'>{0:0}% Complete</div>", qual.RequiredToActualAsPercent);
                html.AppendFormat("<div class='clearfix'></div>");
                html.AppendFormat("</div>");
            }
        }
        catch (Exception exception)
        {
            writer.Write(exception.Message);
        }


        writer.Write(html.ToString());
    }
    #endregion Business Summary

    #region Customer Wall
    private void RenderCustomerWallFromSql(HtmlTextWriter writer, int customerID)
    {
        var html = new StringBuilder();


        // Get the data
        var nodes = ExigoApiContext.CreateODataContext().CustomerWall
            .Where(c => c.CustomerID == customerID)
            .OrderByDescending(c => c.WallItemID)
            .Take(5)
            .Select(c => c);


        // Compile the Html
        if (nodes.Count() == 0)
        {
            html.AppendFormat("No recent activity at this time.");
        }
        else
        {
            var lastDate = new DateTime();


            html.AppendFormat("<ul>");
            foreach (var node in nodes)
            {
                var minifiedDate = new DateTime(node.EntryDate.Year, node.EntryDate.Month, node.EntryDate.Day);
                if (lastDate != minifiedDate)
                {
                    html.AppendFormat("<li class=\"divider\"><span>{0:MMMM d}</span></li>", minifiedDate);
                    lastDate = minifiedDate;
                }

                html.AppendFormat("<li class=\"item item-{0}\">{1} <span class=\"date\">{2}</span></li>", node.Field1.ToLower(), node.Text, node.EntryDate);
            }
            html.AppendFormat("</ul>");
        }



        writer.Write(html.ToString());
    }
    #endregion Customer Summary

    #region Recent Orders
    private void RenderRecentOrders(HtmlTextWriter writer, int customerID)
    {
        var html = new StringBuilder();


        // Get the data
        var startDate = DateTime.Now.AddDays(-60);
        var nodes = ExigoApiContext.CreateODataContext().Orders
            .Where(c => c.CustomerID == customerID)
            .Where(c => c.OrderDate > startDate)
            .OrderByDescending(c => c.OrderDate)
            .Select(c => new {
                c.OrderID,
                c.OrderDate,
                c.OrderStatus.OrderStatusDescription,
                c.Total,
                c.BusinessVolumeTotal,
                c.Details
            });


        // Compile the Html
        if (nodes == null || nodes.Count() == 0)
        {
            html.AppendFormat("No recent orders at this time.");
        }
        else
        {
            html.AppendFormat("<table class='table table-condensed'>");
            html.AppendFormat("<tr>");
            html.AppendFormat("<th>Date</th>");
            html.AppendFormat("<th>Status</th>");
            html.AppendFormat("<th>Item(s)</th>");
            html.AppendFormat("<th>Total</th>");
            html.AppendFormat("<th>BV</th>");
            html.AppendFormat("</tr>");

            // Render the details
            foreach (var node in nodes)
            {
                html.AppendFormat("<tr>");
                html.AppendFormat("<td>{0:MMMM d, yyyy}</td>", node.OrderDate);
                html.AppendFormat("<td>{0}</td>", node.OrderStatusDescription);

                html.AppendFormat("<td>");
                foreach (var detail in node.Details)
                {
                    html.AppendFormat("({0:N0}) {1}<br />", detail.Quantity, detail.ItemDescription);
                }
                html.AppendFormat("</td>");

                html.AppendFormat("<td>{0:C}</td>", node.Total);
                html.AppendFormat("<td>{0:N0}</td>", node.BusinessVolumeTotal);
                html.AppendFormat("</tr>");
            }

            html.AppendFormat("</table>");
        }



        writer.Write(html.ToString());
    }

    public class RecentOrderModel
    {
        public RecentOrderModel()
        {
            this.Details = new List<RecentOrderDetailModel>();
        }

        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public decimal BV { get; set; }
        public string OrderStatus { get; set; }

        public List<RecentOrderDetailModel> Details { get; set; }
    }
    public class RecentOrderDetailModel
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
    }
    #endregion

    #region Autoships
    private void RenderAutoships(HtmlTextWriter writer, int customerID)
    {
        var html = new StringBuilder();


        // Get the data
        var nodes = ExigoApiContext.CreateODataContext().AutoOrders
            .Where(c => c.CustomerID == customerID)
            .Where(c => c.AutoOrderStatusID == 0)
            .OrderByDescending(c => c.NextRunDate)
            .Select(c => new {
                c.AutoOrderID,
                c.NextRunDate,
                c.FrequencyType.FrequencyTypeDescription,
                c.Total,
                c.BusinessVolumeTotal,
                c.Details
            });


        // Compile the Html
        if (nodes == null || nodes.Count() == 0)
        {
            html.AppendFormat("No autoships are setup at this time.");
        }
        else
        {
            html.AppendFormat("<table class='table table-condensed'>");
            html.AppendFormat("<tr>");
            html.AppendFormat("<th>Frequency</th>");
            html.AppendFormat("<th>Next Run Date</th>");
            html.AppendFormat("<th>Item(s)</th>");
            html.AppendFormat("<th>Total</th>");
            html.AppendFormat("<th>BV</th>");
            html.AppendFormat("</tr>");

            foreach (var autoship in nodes)
            {
                html.AppendFormat("<tr>");
                html.AppendFormat("<td>{0}</td>", autoship.FrequencyTypeDescription);
                
                if(autoship.NextRunDate != null)
                {
                    html.AppendFormat("<td>{0:MMMM d, yyyy}</td>", autoship.NextRunDate);
                }
                else
                {
                    html.AppendFormat("<td>Unknown</td>");
                }

                html.AppendFormat("<td>");
                foreach (var detail in autoship.Details)
                {
                    html.AppendFormat("({0:N0}) {1}<br />", detail.Quantity, detail.ItemDescription);
                }
                html.AppendFormat("</td>");

                html.AppendFormat("<td>{0:C}</td>", autoship.Total);
                html.AppendFormat("<td>{0:N0}</td>", autoship.BusinessVolumeTotal);
            }
                html.AppendFormat("</tr>");

            html.AppendFormat("</table>");
        }



        writer.Write(html.ToString());
    }

    public class ActiveAutoshipModel
    {
        public ActiveAutoshipModel()
        {
            this.Details = new List<ActiveAutoshipDetailModel>();
        }

        public int AutoOrderID { get; set; }
        public string Frequency { get; set; }
        public DateTime NextRunDate { get; set; }
        public decimal Total { get; set; }
        public decimal BV { get; set; }

        public List<ActiveAutoshipDetailModel> Details { get; set; }
    }
    public class ActiveAutoshipDetailModel
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
    }
    #endregion
}