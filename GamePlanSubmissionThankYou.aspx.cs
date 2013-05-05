using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Security.Cryptography;
using System.IO;

public partial class GamePlanSubmissionThankYou : System.Web.UI.Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion

    #region Render
    protected void ShowThisMessage()
    {
        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
        StringBuilder s = new StringBuilder();

        s.AppendLine(string.Format(@"
             <div class=""panelarea panel"" id=""reciept"" style=""position: absolute; width: 100%;"">
                 <h1 class=""heading"">Thank You</h1>
                 <table class=""grid"" id=""grid2"">
                     <tbody>
                         <tr class=""gridrow first last"">
                             <td class=""col0 gridcell first"">
                                 <div class=""notes panel"">
                                     A Game Plan Counselor will attempt to call you within the next business day to spend a few minutes asking questions to generate your custom Game Plan Report. We may call you outside of the time you indicated in an effort to reach you sooner and have your report immediately sent to you!<br>
                                     <br>
                                     Check your email for more information.<br>
                                     <br>
                                     We look forward to showing you options that will help create, manage, protect and grow your wealth.<br>
                                     <br>
                                     Successfully,<br>
                                     <br>
                                     {0}
                                 </div>
                             </td>
                             <td class=""col1 gridcell last"">
                                 <div class=""reminder panel"" id=""reminder""></div>
                             </td>
                         </tr>
                     </tbody>
                 </table>
             </div>"
            , "Strongbrook Team"
        ));

        writer.Write(s.ToString());
    }
    #endregion

    #region Exigo Service request
    //ExigoApiServices Api = new ExigoApiServices();
    #endregion

    #region Error Handling
    public string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message += value;
            ShowMessage.Value = "True";
        }
    }
    private string _message;

    private void ClearMessage()
    {
        Message = string.Empty;
        ShowMessage.Value = "";
    }

    #endregion
}