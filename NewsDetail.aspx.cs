using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class NewsDetailPage : System.Web.UI.Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        // If we don't have a valid news item, redirect back to the archive page
        if(Request.QueryString["id"] == null)
        {
            Response.Redirect("NewsArchive.aspx");
        }
    }
    #endregion

    #region Data
    public GetCompanyNewsItemResponse NewsDetail
    {
        get
        {
            if(_newsDetail == null)
            {
                _newsDetail = ExigoApiContext.CreateWebServiceContext().GetCompanyNewsItem(new GetCompanyNewsItemRequest
                {
                    NewsID = Convert.ToInt32(Request.QueryString["id"])
                });
            }
            return _newsDetail;
        }
    }
    private GetCompanyNewsItemResponse _newsDetail;
    #endregion

    #region Render
    public void RenderCompanyNewsDetail()
    {
        HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
        StringBuilder s = new StringBuilder();

        // Render the data
        if(NewsDetail.NewsID > 0 && NewsDetail.CompanySettings != NewsCompanySettings.AccessNotAvailable && NewsDetail.WebSettings == NewsWebSettings.AccessAvailable)
        {
            s.AppendLine(string.Format(@"
                <div class='newsdetail'>
                    <div class='title'>{0}</div>
                    <div class='date'>Published on {1:dddd, MMMM d, yyyy, h:mmtt}</div>
                    <div class='content'>{2}</div>
                </div>
            ", NewsDetail.Description,
                NewsDetail.CreatedDate,
                NewsDetail.Content));
        }
        else
        {
            Response.Redirect("Home.aspx");
        }



        writer.Write(s.ToString());
    }
    #endregion
}