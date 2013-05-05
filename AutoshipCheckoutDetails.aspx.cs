using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Exigo.OData;
using Exigo.WebService;

public partial class AutoshipCheckoutDetails : Page, IPostBackEventHandler
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        // If there are no items in the cart, redirect them to the cart page.
        if (Autoship.Cart.Items.Count == 0)
        {
            Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Cart));
            return;
        }


        if (!IsPostBack)
        {
            PopulateAutoshipFrequencyTypes();
            PopulatePropertyBagValues_Load();
        }
    }
    #endregion

    #region Properties
    public AutoshipCartManager Autoship
    {
        get
        {
            if (_autoship == null)
            {
                _autoship = new AutoshipCartManager();
            }
            return _autoship;
        }
    }
    private AutoshipCartManager _autoship;

    public DateTime StartDate
    {
        get { return Convert.ToDateTime(txtStartDate.Text); }
        set { txtStartDate.Text = value.ToString("dddd, MMMM d, yyyy"); }
    }
    #endregion

    #region Load Data
    public void PopulatePropertyBagValues_Load()
    {
        lstFrequency.SelectedValue = ((int)Autoship.PropertyBag.Frequency).ToString();

        // Handle the start/next date
        var startDate = new DateTime();
        if(Autoship.PropertyBag.ExistingAutoshipID == 0) startDate = Autoship.PropertyBag.StartDate;
        else startDate = Autoship.PropertyBag.NextRunDate;

        // Handle empty datetimes
        if(startDate == new DateTime())
        {
            if(Autoship.PropertyBag.ExistingAutoshipID != 0)
            {
                startDate = GlobalUtilities.GetNextAvailableAutoOrderStartDate(Autoship.PropertyBag.StartDate);
            }
            else
            {
                startDate = GlobalUtilities.GetNextAvailableAutoOrderStartDate(DateTime.Now.AddDays(1).Date);
            }
        }

        StartDate = startDate;
    }
    #endregion

    #region Save Data
    public void SaveDataToPropertyBag()
    {
        Autoship.PropertyBag.AutoshipDescription = "My Monthly Autoship";
        Autoship.PropertyBag.Frequency = (Exigo.WebService.FrequencyType)Enum.Parse(typeof(Exigo.WebService.FrequencyType), lstFrequency.SelectedValue);
        Autoship.PropertyBag.StartDate = StartDate;

        Autoship.PropertyBag.Save();
    }
    #endregion

    #region IPostBackEventHandlers
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('|');

        switch (args[0])
        {
            case "SaveChanges":
                SaveDataToPropertyBag();

                if (Autoship.PropertyBag.ReferredByEndOfCheckout)
                {
                    Autoship.PropertyBag.ReferredByEndOfCheckout = false;
                    Autoship.PropertyBag.Save();

                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.Review));
                }
                else
                {
                    Response.Redirect(Autoship.GetStepUrl(AutoshipManagerStep.ShippingAddress));
                }
                break;

            default:
                throw new Exception("RaisePostBackEvent argument '" + args[0] + "' is not defined.");
        }
    }
    #endregion

    #region Populate Form Options
    private void PopulateAutoshipFrequencyTypes()
    {
        lstFrequency.Items.Clear();

        foreach (var frequency in Autoship.AutoshipSettings.AvailableFrequencyTypes)
        {
            var listItemText = string.Empty;
            switch (frequency)
            {
                case Exigo.WebService.FrequencyType.BiMonthly:          listItemText = Resources.Shopping.EveryOtherMonth; break;
                case Exigo.WebService.FrequencyType.BiWeekly:           listItemText = Resources.Shopping.EveryOtherWeek; break;
                case Exigo.WebService.FrequencyType.EveryFourWeeks:     listItemText = Resources.Shopping.EveryFourWeeks; break;
                case Exigo.WebService.FrequencyType.Monthly:            listItemText = Resources.Shopping.EveryMonth; break;
                case Exigo.WebService.FrequencyType.Quarterly:          listItemText = Resources.Shopping.FourTimesAYear; break;
                case Exigo.WebService.FrequencyType.SemiYearly:         listItemText = Resources.Shopping.TwiceAYear; break;
                case Exigo.WebService.FrequencyType.Weekly:             listItemText = Resources.Shopping.EveryWeek; break;
                case Exigo.WebService.FrequencyType.Yearly:             listItemText = Resources.Shopping.OnceAYear; break;
            }

            var listItem = new ListItem();
            listItem.Text = listItemText;
            listItem.Value = ((int)frequency).ToString();
            lstFrequency.Items.Add(listItem);
        }

        lstFrequency.SelectedValue = ((int)Autoship.AutoshipSettings.DefaultFrequencyType).ToString();
    }
    #endregion
}
