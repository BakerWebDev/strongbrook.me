using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.WebService;

public partial class NavigationTreeData : Page
{
    public int EnrollerID { get { return Convert.ToInt32(Request.QueryString["enrollerid"]); } }
    public int SponsorID { get { return Convert.ToInt32(Request.QueryString["sponsorid"]); } }
    public int Level { get { return Convert.ToInt32(Request.QueryString["level"]); } }
    public int BatchSize = 10000;
    public Exigo.WebService.TreeType TreeType = TreeType.UniLevel;

    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["sponsorid"] == null)
        {
            Response.Write("Invalid Sponsor ID");
            Response.End();
        }
        else if (Request.QueryString["enrollerid"] == null)
        {
            Response.Write("Invalid Enroller ID");
            Response.End();
        }
        else if (Request.QueryString["level"] == null)
        {
            Response.Write("Invalid Level");
            Response.End();
        }
    }
    #endregion

    #region Render
    protected override void Render(HtmlTextWriter writer)
    {
        var context = ExigoApiContext.CreateWebServiceContext();
        var nodes = context.GetDownline(new GetDownlineRequest
        {
            CustomerID = EnrollerID,
            TreeType = TreeType,
            MaxLevelDepth = Level,
            PeriodType = (int)PeriodTypes.Default,
            BatchSize = BatchSize
        }).Nodes;


        // Render it
        if (nodes.Length == 0)
        {
            writer.Write("No tree nodes.");
        }
        else
        {
            int nodeCounter = 0;
            writer.Write("[");
            foreach (NodeResponse node in nodes.OrderByDescending(n => n.CreatedDate).Where(n => n.CustomerType == 4))
            {
                var FirstName = RemoveUnwantedCharacters(node.FirstName);
                var LastName = RemoveUnwantedCharacters(node.LastName);

                if (nodeCounter > 0)
                    writer.Write(",");

                writer.Write(string.Format(@"
                        {{
                            ""data"": ""{0} {1}"",
                            ""attr"": {{ ""rank"": ""rank{2}"", ""title"": ""ID# {3}"", ""customerid"": ""{3}"" }}
                        }}", FirstName,
                      LastName,
                      node.RankID,
                      node.NodeID));

                nodeCounter++;
            }
            writer.Write("]");
        }

        base.Render(writer);
    }
    #endregion

    #region Helper Methods
    public string RemoveUnwantedCharacters(string s)
    {
        List<string> CharsToRemove = new List<string>
        {
            "\"",
            "(",
            ")"
        };

        foreach (var c in CharsToRemove)
        {
            s = s.Replace(c, "");
        }

        return s;
    }
    #endregion
}