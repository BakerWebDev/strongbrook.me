using Exigo.OData;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GPRLeadManager : System.Web.UI.Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion

    #region Fetching Data
    public List<ReportDataNode> FetchReportData()
    {
        // Get the data
        var customerID = Identity.Current.CustomerID;

        // We have to get this data from the web service because it is real-time.
        // This call can be slow because it has to fetch all the rows every call.
        var data = ExigoApiContext.CreateWebServiceContext().GetCustomerLeads(new GetCustomerLeadsRequest
        {
            CustomerID = customerID           
        });


        // If we somehow didn't get any records back, stop here.
        if(data == null) return new List<ReportDataNode>();


        // Create a new list of our reporting nodes based on the nodes we got back from the web service.
        // In other words, convert their collection into one of our own.
        var nodes = data.CustomerLeads.ToList().Select(c => new ReportDataNode
        {
            CustomerID = 24100, // c.CustomerLeadID,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Company = c.Company,
            Email = c.Email,
            Phone = c.Phone,
            Phone2 = c.Phone2,
            LeadID = c.CustomerLeadID,
            CreatedDate = c.BirthDate.ToString(),
            Zip = c.Zip
        });


        // Order the nodes, since the web service can't do it. THIS IS A MUST FOR PAGINATION!
        var orderedNodes = nodes.OrderBy(c => c.LeadID);


        // Return the records we need, taking pagination into account.
        return orderedNodes.ToList();
    }
    #endregion

    #region Render
    protected override void Render(HtmlTextWriter writer) // Ask Travis why the word "Render" is so important here.
    {
        if(Request.QueryString["action"] != null)
        {
            switch(Request.QueryString["action"])
            {
                case "fetch":
                    //Fetch the nodes
                    var nodes = FetchReportData();
                    
                    //Assemble the records
                    var html = new StringBuilder();

                    //First, add our record count
                    html.AppendFormat("{0}^", nodes.Count());
                    
                    #region For each record in the response write a row.
                    foreach(var record in nodes)
                    {
                        if (record.Zip == "1") // Only show customer leads where the Zip is equal to 1, because I set the Zip to 1 if the lead was created by the Game Plan Request form.
                        { 
                            // Assemble our html
                            html.AppendFormat("<tr>");
//                            #region 1st column
//                            html.AppendFormat(@"
//                                        <td class='customerdetails'>
//                                            <span class='name'><a href='Profile.aspx?id={0}' title='View profile'>{0}</a></span>
//                                        </td>",
//                                GlobalUtilities.Coalesce(record.Company, record.FirstName + " " + record.LastName));
//                            #endregion
//                            #region 2nd column
//                            var email = (!string.IsNullOrEmpty(record.Email)) ? string.Format("<i class='icon-envelope'></i>&nbsp;<a href='CreateMessage.aspx?to={0}' title='Send email'>{0}</a><br />", record.Email) : "";
//                            var phone = (!string.IsNullOrEmpty(record.Phone)) ? string.Format("<i class='icon-home'></i>&nbsp;{0}<br />", record.Phone) : "";
//                            var phone2 = (!string.IsNullOrEmpty(record.Phone2)) ? string.Format("<i class='icon-briefcase'></i>&nbsp;{0}", record.Phone2) : "";
//                            html.AppendFormat(@"
//                                        <td>
//                                            {0}
//                                            {1}
//                                            {2}
//                                        </td>
//                                    ", email,
//                                 phone,
//                                 phone2);
//                            #endregion
//                            #region 3rd column
//                            html.AppendFormat("<td>{0}</td>", record.LeadID);
//                            #endregion
//                            #region 4th column
//                            var email2 = (!string.IsNullOrEmpty(record.Email)) ? string.Format("<i class='icon-envelope'></i>&nbsp;<a href='CreateMessage.aspx?to={0}' title='Send email'>{0}</a><br />", record.Email) : "";
//                            html.AppendFormat(@"
//                                        <td>
//                                            {0}
//                                        </td>
//                                    ", email);
//                            #endregion

                            #region 1st column
                            html.AppendFormat("<td>{0}</td>", record.CreatedDate);
                            #endregion
                            #region 2nd column
                            html.AppendFormat("<td>{0}</td>", record.DisplayName);
                            #endregion
                            #region 3rd column
                            html.AppendFormat("<td>{0}</td>", record.Phone);
                            #endregion
                            #region 4th column
                            html.AppendFormat("<td>{0}</td>", record.Email);
                            #endregion

                            html.AppendFormat("</tr>");                        
                        }

                    }
                    #endregion

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
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string DisplayName
        {
            get
            {
                return GlobalUtilities.Coalesce(this.Company, this.FirstName + " " + this.LastName);
            }
        }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        //public string CustomerType { get; set; }
        //public string CustomerStatus { get; set; }
        public string State { get; set; }
        public string CreatedDate { get; set; }
        public int LeadID { get; set; }
        public string Zip { get; set; }
    }
    #endregion

    #region Error Handling

    #endregion
}