using Exigo.OData;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UnilevelWaitingRoom : System.Web.UI.Page
{
    private int GracePeriodInDays = 30;





    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowRequestedPanel();
        }
    }
    #endregion

    #region Properties
    public int CustomerIDToBePlaced
    {
        get
        {
            if (Request.QueryString["id"] == null) return 0;
            else return Convert.ToInt32(Request.QueryString["id"]);
        }
    }
    #endregion

    #region Render
    protected override void Render(HtmlTextWriter writer)
    {
        if (Request.QueryString["action"] != null)
        {
            if (Request.QueryString["action"] == "validateparent")
            {
                try
                {
                    var nodeID = Convert.ToInt32(Request.QueryString["id"]);
                    var requestedParentID = Convert.ToInt32(Request.QueryString["parent"]);
                    bool IsValidSoFar = true;

                    // Validate that the customerID they entered is not the same as the ID of the customer they are trying to place.
                    if (IsValidSoFar)
                    {
                        if (requestedParentID == nodeID)
                        {
                            IsValidSoFar = false;
                            writer.Write("0|<span>You cannot place a customer underneath themselves.</span>");
                            return;
                        }
                    }


                    // Validate that the parent is in the backoffice owner's unilevel tree
                    if (IsValidSoFar)
                    {
                        var requestedParentIDIsInMyTree = ExigoApiContext.CreateWebServiceContext().Validate(new IsUniLevelChildValidateRequest
                        {
                            ParentID = Identity.Current.CustomerID,
                            ChildID = requestedParentID
                        }).IsValid;

                        if (!requestedParentIDIsInMyTree)
                        {
                            IsValidSoFar = false;
                            writer.Write("0|<span>The parent ID you have requested is not in your organization.</span>");
                            return;
                        }
                    }


                    // If all of the validation rules check out, write a 1 to the screen for Ajax to pick up.
                    if (IsValidSoFar)
                    {
                        var parent = (from c in ExigoApiContext.CreateODataContext().Customers
                                      where c.CustomerID == requestedParentID
                                      select new
                                        {
                                            c.FirstName,
                                            c.LastName,
                                            c.MainCity,
                                            c.MainState,
                                            c.MainCountry
                                        }).FirstOrDefault();
                        writer.Write(string.Format("1|{0} {1} from {2}, {3}, {4}",
                            parent.FirstName,
                            parent.LastName,
                            parent.MainCity,
                            parent.MainState,
                            parent.MainCountry));
                    }
                }
                catch (Exception ex)
                {
                    writer.Write("0|<span>An error occurred while validating your requested parent ID: " + ex.Message + "</span>");
                }
            }
        }
        else
        {
            base.Render(writer);
        }
    }

    public void RenderSuccessfulPlacementMessage()
    {
        if (Request.QueryString["success"] != null)
        {
            var arguments = Request.QueryString["success"].Split('|');
            var customerID = arguments[0];
            var parentID = arguments[1];

            HtmlTextWriter writer = new HtmlTextWriter(Response.Output);
            writer.Write(@"
                <div id='placementsuccessfulmessage'>
                    <b id='messageheader'>Placement Successful!</b><br />
                    ID# " + customerID + @" has been placed underneath ID# " + parentID + @".
                </div>");
        }
    }
    #endregion

    #region Panels
    private void ResetPanels(Control cnt)
    {
        if (cnt is Panel)
        {
            ((Panel)cnt).Visible = false;
        }
        foreach (Control subCnt in cnt.Controls)
        {
            ResetPanels(subCnt);
        }
    }
    private void ShowRequestedPanel()
    {
        if (Request.QueryString["mode"] != null)
        {
            switch (Request.QueryString["mode"])
            {
                case "place":
                    if (CustomerToBePlaced != null)
                    {
                        ResetPanels(Page); Panel_Place.Visible = true;
                    }
                    else
                    {
                        ResetPanels(Page); Panel_List.Visible = true;
                    }
                    break;

                default:
                    ResetPanels(Page);
                    Panel_List.Visible = true;
                    break;
            }
        }
        else
        {
            ResetPanels(Page);
            Panel_List.Visible = true;
        }
    }
    #endregion

    #region Helper Methods
    public string GetLastPlacementOpportunityDateDisplay(DateTime createdDate)
    {
        DateTime endDate = createdDate.AddDays(GracePeriodInDays);
        TimeSpan difference = endDate.Subtract(DateTime.Now);

        if (difference.TotalHours < 1 && difference.TotalHours > 0) return string.Format("{0:dddd, MMMM d, yyyy h:mm tt}  <span class='Red'>( < {1:N0} minutes)</span>", endDate, difference.TotalMinutes);
        else if (difference.TotalDays < 1) return string.Format("{0:dddd, MMMM d, yyyy h:mm tt}  <span class='Red'>( < {1:N0} hours)</span>", endDate, difference.TotalHours);
        else if (difference.TotalDays == 1) return string.Format("Tomorrow, {0:dddd, MMMM d, yyyy h:mm tt}", endDate);
        else return string.Format("{0:dddd, MMMM d, yyyy}", endDate);
    }
    #endregion

    #region Data Fetching
    public List<Customer> WaitingRoomCustomers
    {
        get
        {
            if (_waitingRoomCustomers == null)
            {
                _waitingRoomCustomers = new List<Customer>();
                _waitingRoomCustomers = GetWaitingRoomCustomersQuery().Select(c => new Customer()
                                            {
                                                CustomerID = c.CustomerID,
                                                FirstName = c.FirstName,
                                                LastName = c.LastName,
                                                EnrollerID = c.EnrollerID,
                                                SponsorID = c.SponsorID,
                                                CreatedDate = c.CreatedDate
                                            }).ToList();
            }
            return _waitingRoomCustomers;
        }
    }
    private List<Customer> _waitingRoomCustomers;

    public Customer CustomerToBePlaced
    {
        get
        {
            if (_customerToBePlaced == null)
            {
                _customerToBePlaced = GetWaitingRoomCustomersQuery()
                    .Where(c => c.CustomerID == CustomerIDToBePlaced)
                    .Select(c => new Customer()
                    {
                        CustomerID = c.CustomerID,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        EnrollerID = c.EnrollerID,
                        SponsorID = c.SponsorID
                    }).FirstOrDefault();
            }
            return _customerToBePlaced;
        }
    }
    private Customer _customerToBePlaced;

    private IQueryable<Customer> GetWaitingRoomCustomersQuery()
    {
        return ExigoApiContext.CreateODataContext().Customers
            .Where(c => c.EnrollerID == Identity.Current.CustomerID)
            .Where(c => c.Field1 == string.Empty)
            .Where(c => c.CreatedDate <= DateTime.Now)
            .Where(c => c.CreatedDate >= DateTime.Now.AddDays(GracePeriodInDays * -1))
            .OrderBy(c => c.CreatedDate);
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "PlaceNode")
        {
            int newSponsorID = Convert.ToInt32(txtParentID.Text);

            // Place the node into the unilevel tree
            ExigoApiContext.CreateWebServiceContext().PlaceUniLevelNode(new PlaceUniLevelNodeRequest
            {
                CustomerID = CustomerIDToBePlaced,
                ToSponsorID = newSponsorID,
                Reason = "Waiting Room Placement on " + DateTime.Now
            });

            // Change the customer's Move Date field to today's date
            ExigoApiContext.CreateWebServiceContext().UpdateCustomer(new UpdateCustomerRequest
            {
                CustomerID = CustomerIDToBePlaced,
                Field1 = DateTime.Now.ToString()
            });


            // Serialize our ID's so we can get them on the other side
            Response.Redirect(Request.Url.AbsolutePath + string.Format("?success={0}|{1}", CustomerIDToBePlaced, newSponsorID));
        }
    }
    #endregion
}