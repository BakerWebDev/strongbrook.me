using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Exigo.API;
using Strongbrook.Bonus;

public partial class Secure_DataStores_CommissionsTree_bottominfo : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Bon = new Bonus();
        CustomerNode = new Node();
        CustomerNode = Bon.GetCommissionsPaidToPersonFromIDInPreviousPeriod(_fromCustomerID, _toCustomerID, _periodID);
        if (CustomerNode.CustomerID == 0)
        {
            CustomerNode.IsPayingOutThisPeriod = false;

            //Need to get Customer Info and why they didn't get paid
            CustomerNode.CustomerID = Convert.ToInt32(_fromCustomerID);
            Node nonPayingNode = Bon.GetReasonsNotPaying(CustomerNode, _toCustomerID, _periodID, _periodType);
        }
        else
        {
            CustomerNode.IsPayingOutThisPeriod = true;

            //Need to get More info
        }
    }

    #region private variables
    private int _periodID
    {
        get
        {
            if (Request.QueryString["period"] != null)
            {
                return Convert.ToInt32(Request.QueryString["period"]);
            }
            else
            {
                return GetCurrentPeriod(_periodType);
            }
        }
    }

    private int _periodType
    {
        get
        {
            if (Request.QueryString["periodtype"] != null)
            {
                return Convert.ToInt32(Request.QueryString["periodtype"]);
            }
            else
            {
                return 1;
            }
        }
    }

    private string _toCustomerID
    {
        get
        {
            if (Request.QueryString["customerID"] != null)
            {
                return Request.QueryString["customerID"];
            }
            else
            {
                return Identity.Current.CustomerID.ToString();
            }
        }
    }
    private string _fromCustomerID
    {
        get
        {
            if (Request.QueryString["fromcustomer"] != null)
            {
                return Request.QueryString["fromcustomer"];
            }
            else
            {
                return "Invalid";
            }
        }
    }

    #endregion

    public Bonus Bon;
    public Node CustomerNode;
    public bool IsPayingOut;

    #region API Calls
    public int GetCurrentPeriod(int periodType)
    {
        int currentPeriod;

        var context = ExigoApiContext.CreateODataContext();
        var periodQuery = (from p in context.Periods
                           where p.IsCurrentPeriod == true
                           where p.PeriodTypeID == periodType
                           select new
                           {
                               p.PeriodID
                           }).FirstOrDefault();

        currentPeriod = periodQuery.PeriodID;

        return currentPeriod;
    }

    protected override void Render(HtmlTextWriter writer)
    {
        if (!CustomerNode.IsPayingOutThisPeriod)
        {
            writer.Write(string.Format(@"
            <div class=""customer info"">
                Customer ID: {0}<br />
                Parent ID: {1}<br />
                Rank ID: {2} <br />
                Pay Rank ID: {3} <br />
                Full Name: {4} <br />
            </div>
        ", CustomerNode.CustomerID, CustomerNode.ParentID, CustomerNode.RankID, CustomerNode.PayRankID, CustomerNode.FullName));
            if (_periodType == 1) //Render method for weekly bonuses
            {
                if (CustomerNode.reasonsNotQualified != null)
                {
                    //Write out the Weekly Tier Bonus Orders
                    writer.Write(string.Format(@"
                <div class=""nonpaying bonus"">
                    <span class=""header"">Weekly Tier Bonus</span><br />
                    <div class=""tier order"" style=""display: none;"">
                        <ul>
                            <li class=""order head"">Order</li>
                "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Weekly Tier Bonus")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[1]));
                    }
                    writer.Write(string.Format(@"        
                        </ul>
                    </div>
                    <div class=""tier volume"" style=""display: none;"">
                        <ul>
                            <li class=""volume head"">Order Volume</li>
                "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Weekly Tier Bonus")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[2]));
                    }
                    writer.Write(string.Format(@"
                        </ul>
                    </div>
                    <div class=""tier reason"" style=""display: none;"">
                        <ul>
                            <li class=""reason head"">Reason not Qualified</li>
                "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Weekly Tier Bonus")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[3]));
                    }
                    writer.Write(string.Format(@"
                        </ul>
                    </div>
                    "));

                    //Write out the Weekly Enroller Bonus Orders
                    writer.Write(string.Format(@"<div class=""clearboth""></div><span class=""header"">Weekly Enroller Bonus</span></br>"));
                    writer.Write(string.Format(@"
                    <div class=""enroller order"" style=""display: none;"">
                        <ul>
                            <li class=""order head"">Order</li>
                    "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Weekly Enroller Bonus")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[1]));
                    }
                    writer.Write(string.Format(@"
                        <ul>
                    </div>
                    <div class=""enroller volume"" style=""display: none;"">
                        <ul>
                            <li class=""volume head"">Order Volume</li>
                    "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Weekly Enroller Bonus")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[2]));
                    }
                    writer.Write(string.Format(@"
                        </ul>
                    </div>
                    <div class=""enroller reason"" style=""display: none;"">
                        <ul>
                            <li class=""reason head"">Reason not Qualified</li>
                    "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Weekly Enroller Bonus")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[3]));
                    }
                    writer.Write(string.Format(@"
                        </ul>
                    </div>
                    "));

                    //Write out Retail Commissions
                    writer.Write(string.Format(@"<div class=""clearboth""></div><span class=""header"">Retail Commission</span></br>"));
                    writer.Write(string.Format(@"
                    <div class=""retail order"" style=""display: none;"">
                        <ul>
                            <li class=""order head"">Order</li>
                    "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Retail Commission")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[1]));
                    }
                    writer.Write(string.Format(@"
                        <ul>
                    </div>
                    <div class=""retail volume"" style=""display: none;"">
                        <ul>
                            <li class=""volume head"">Order Volume</li>
                    "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Retail Commission")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[2]));
                    }
                    writer.Write(string.Format(@"
                        </ul>
                    </div>
                    <div class=""retail reason"" style=""display: none;"">
                        <ul>
                            <li class=""reason head"">Reason not Qualified</li>
                    "));
                    foreach (var i in CustomerNode.reasonsNotQualified.Where(k => k.Contains("Retail Commission")))
                    {
                        writer.Write(string.Format(@"<li class=""order"">{0}</li>", i[3]));
                    }
                    writer.Write(string.Format(@"
                        </ul>
                    </div>
                    "));

                    writer.Write(string.Format(@"
                </div>
                "));
                }
            }
            
        }
        else
        {
            writer.Write("<ul>");

            writer.Write(string.Format(@"
            Customer ID: {0}<br />
            Parent ID: {1}<br />
            Rank ID: {2} <br />
            Pay Rank ID: {3} <br />
            Full Name: {4} <br />
        ", CustomerNode.CustomerID, CustomerNode.ParentID, CustomerNode.RankID, CustomerNode.PayRankID, CustomerNode.FullName));
            foreach (var c in CustomerNode.BonusDetails)
            {
                writer.Write(string.Format(@"
                Bonus: {0}<br />
                Amount: {1:C2}<br/>
            ", c.BonusType, c.BonusAmount));
            }
            writer.Write("</ul>");
        }
        base.Render(writer);
    }
    #endregion
}