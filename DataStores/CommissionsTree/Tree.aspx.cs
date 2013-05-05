using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using Exigo.API;
using Exigo.OData;
using Exigo.WebService;
using Strongbrook.Bonus;

public partial class Tree : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bon = new Bonus();
        UnilevelNodes = bon.GetUnilevelDownlineTreeAndIfPaid(_customerID, _level, _periodID, _periodType);
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

    private string _customerID
    {
        get
        {
            if (Request.QueryString["customerID"] != null)
            {
                return Request.QueryString["customerID"];
            }
            else
            {
                return "Invalid";
            }
        }
    }

    private int _level
    {
        get
        {
            if (Request.QueryString["level"] != null)
            {
                return Convert.ToInt32(Request.QueryString["level"]);
            }
            else
            {
                return 1;
            }
        }
    }

    #endregion


    public List<Node> UnilevelNodes;
    public Bonus bon;

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
        writer.Write("<div class='Nodes'>");
        int i = 0;
        foreach (Node node in UnilevelNodes.OrderBy(k => k.ParentID))
        {
            if (node.IsPayingOutThisPeriod)
            {
                writer.Write(string.Format(@"
                <div class='Node Paying' data-name='{0}' id='Node{1}' data-my-parent='{2}'>

                </div>
                ", node.CustomerID, i, node.ParentID));
            }
            else
            {
                writer.Write(string.Format(@"
                <div class='Node' data-name='{0}' id='Node{1}' data-my-parent='{2}'>

                </div>
                ", node.CustomerID, i, node.ParentID));
            }
            i++;
        }
        writer.Write("</div>");
        base.Render(writer);
    }

    #endregion
}