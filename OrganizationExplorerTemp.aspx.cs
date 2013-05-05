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

public partial class OrganizationExplorerTemp : System.Web.UI.Page
{
    public int CustomerID                           = Identity.Current.CustomerID;

    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    #endregion Page Load
}