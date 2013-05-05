using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BinaryPlacementPreferences : System.Web.UI.Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {            
        if (!IsPostBack)
        {
            PopulateAvailablePlacementPreferences_OnPageLoad();
        }

        if(Request.QueryString["status"] == "1")
        {
            Error.Type = Exigo.WebControls.ErrorMessageType.Success;
            Error.Header = "Success!";
            Error.Message = "Your placement preferences have been saved.";
        }
    }
    #endregion

    #region Properties
    #endregion

    #region API Methods
    public SetBinaryPreferenceRequest Request_SetPlacementPreference()
    {
        SetBinaryPreferenceRequest req = new SetBinaryPreferenceRequest();

        req.CustomerID = Identity.Current.CustomerID;

        // Placement Preference
        req.PlacementType = (BinaryPlacementType)Enum.Parse(typeof(BinaryPlacementType), rdoPlacementPreference.SelectedValue);

        return req;
    }
    #endregion

    #region Event Handlers
    public void SaveChanges_Click(object sender, EventArgs e)
    {
        try
        {
            ExigoApiContext.CreateWebServiceContext().SetBinaryPreference(Request_SetPlacementPreference());
            Response.Redirect(Request.Url.AbsolutePath + "?status=1");
        }
        catch (Exception ex)
        {
            Error.Type = Exigo.WebControls.ErrorMessageType.Failure;
            Error.Header = "Oops!";
            Error.Message = "Your preferences could not be updated: " + ex.Message;
        }
    }
    #endregion

    #region Population of Data
    public void PopulateAvailablePlacementPreferences_OnPageLoad()
    {
        // Reset the radio group just in case
        rdoPlacementPreference.Items.Clear();

        // List of available placements
        Dictionary<BinaryPlacementType, string> availablePlacements = new Dictionary<BinaryPlacementType, string>
        {
            { BinaryPlacementType.BuildLeft, "Build Left" },
            { BinaryPlacementType.BuildRight, "Build Right" },
            { BinaryPlacementType.BalancedBuild, "Balanced" },
            { BinaryPlacementType.EvenFill, "Even Fill" },
            { BinaryPlacementType.LeftEvenFill, "Even Fill - Left Leg Only" },
            { BinaryPlacementType.RightEvenFill, "Even Fill - Right Leg Only" }
        };

        // Get the customer's current preference
        BinaryPlacementType currentPreference = ExigoApiContext.CreateWebServiceContext().GetBinaryPreference(new GetBinaryPreferenceRequest
        {
            CustomerID = Identity.Current.CustomerID
        }).PlacementType;

        // Fill the radio button list
        foreach (var placementTy in availablePlacements)
        {
            ListItem item = new ListItem
            {
                Text = placementTy.Value,
                Value = ((int)placementTy.Key).ToString()
            };
            if (currentPreference == placementTy.Key)
            {
                item.Selected = true;
            }

            rdoPlacementPreference.Items.Add(item);
        }

        // Double-check to ensure that one is checked. If not, auto-select the first one.
        if (rdoPlacementPreference.SelectedIndex == -1)
        {
            rdoPlacementPreference.SelectedIndex = 0;
        }
    }
    #endregion
}