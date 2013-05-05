using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Strongbrook.Bonus;

public partial class Secure_DataStores_CommissionsTree_rightinfo : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bon = new Bonus();

        commissionTotals = CalculateCommissionTotals();
    }

    public string CustomerID
    {
        get
        {
            if (Request.QueryString["customerID"] == null)
            {
                return "Invalid Customer ID";
            }
            else
            {
                return Request.QueryString["customerID"];
            }
        }
    }
    public int PeriodID
    {
        get
        {
            if (Request.QueryString["periodid"] == null)
            {
                return 1;
            }
            else
            {
                return Convert.ToInt32(Request.QueryString["periodid"]);
            }
        }
    }
    public int PeriodType
    {
        get
        {
            if (Request.QueryString["periodtype"] == null)
            {
                return 1;
            }
            else
            {
                return Convert.ToInt32(Request.QueryString["periodtype"]);
            }
        }
    }
    public int MaxLevel
    {
        get
        {
            if (Request.QueryString["maxlevel"] == null)
            {
                return 1;
            }
            else
            {
                return Convert.ToInt32(Request.QueryString["maxlevel"]);
            }
        }
    }

    public Bonus bon;
    Dictionary<int, decimal> commissionTotals;

    public Dictionary<int, decimal> CalculateCommissionTotals()
    {
        Dictionary<int, decimal> comms = new Dictionary<int, decimal>();

        for (int i = 1; i < MaxLevel + 1; i++)
        {
            decimal comm = 0;

            comm = bon.GetTotalCommissionsEarnedForLevel(CustomerID, PeriodID, PeriodType, i);
            comms.Add(i, comm);
        }

        return comms;
    }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.Write(@"
            <div class='header'>
                Click a level to display it
            </div>
        ");
        for (int i = 1; i < MaxLevel + 1; i++)
        {
            writer.Write(string.Format(@"
                <div class='level' data-name='{0:N0}'>
                    <span class='level'>Level {0}</span>
                    <span class='pay'>{1:C2}</span>
                </div>
            ", i, commissionTotals[i]));
        }

        base.Render(writer);
    }
}