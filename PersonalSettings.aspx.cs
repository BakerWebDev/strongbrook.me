using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class PersonalSettings : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            PopulateCustomerData();

            if(Request.QueryString["status"] == "1")
            {
                Error.Type = Exigo.WebControls.ErrorMessageType.Success;
                Error.Header = "Success!";
                Error.Message = "Your personal information has been saved.";
            }
        }
    }

    #region Properties
    public string FirstName
    {
        get { return txtFirstName.Text.FormatForExigo(); }
        set { txtFirstName.Text = value; }
    }
    public string LastName
    {
        get { return txtLastName.Text.FormatForExigo(); }
        set { txtLastName.Text = value; }
    }
    public string Company
    {
        get { return txtCompany.Text.FormatForExigo(); }
        set { txtCompany.Text = value; }
    }
    public string Email
    {
        get { return txtEmail.Text.FormatForExigo(ExigoDataFormatType.Email); }
        set { txtEmail.Text = value; }
    }
    public string Phone
    {
        get { return txtPhone.Text.FormatForExigo(ExigoDataFormatType.Phone); }
        set { txtPhone.Text = value; }
    }
    public string Phone2
    {
        get { return txtPhone2.Text.FormatForExigo(ExigoDataFormatType.Phone); }
        set { txtPhone2.Text = value; }
    }
    public string MobilePhone
    {
        get { return txtMobilePhone.Text.FormatForExigo(ExigoDataFormatType.Phone); }
        set { txtMobilePhone.Text = value; }
    }
    public int BirthDay
    {
        get { return Convert.ToInt32(lstBirthDay.SelectedValue); }
        set { lstBirthDay.SelectedValue = value.ToString(); }
    }
    public int BirthMonth
    {
        get { return Convert.ToInt32(lstBirthMonth.SelectedValue); }
        set { lstBirthMonth.SelectedValue = value.ToString(); }
    }
    public int BirthYear
    {
        get { return Convert.ToInt32(lstBirthYear.SelectedValue); }
        set { lstBirthYear.SelectedValue = value.ToString(); }
    }
    public DateTime BirthDate
    {
        get { return new DateTime(BirthYear, BirthMonth, BirthDay); }
        set
        {
            BirthDay        = value.Day;
            BirthMonth      = value.Month;
            BirthYear       = value.Year;
        }
    }    
    public string Address1
    {
        get { return txtAddress1.Text.FormatForExigo(); }
        set { txtAddress1.Text = value; }
    }
    public string Address2
    {
        get { return txtAddress2.Text.FormatForExigo(); }
        set { txtAddress2.Text = value; }
    }
    public string City
    {
        get { return txtCity.Text.FormatForExigo(); }
        set { txtCity.Text = value; }
    }
    public string State
    {
        get { return lstState.SelectedValue; }
        set { lstState.SelectedValue = value; }
    }
    public string Zip
    {
        get { return txtZip.Text.FormatForExigo(); }
        set { txtZip.Text = value; }
    }
    public string Country
    {
        get { return lstCountry.SelectedValue; }
        set { lstCountry.SelectedValue = value; }
    }
    #endregion    

    #region Exigo API Requests
    private UpdateCustomerRequest Request_UpdateCustomer()
    {
        var request = new UpdateCustomerRequest();

        request.CustomerID      = Identity.Current.CustomerID;
        request.FirstName       = FirstName;
        request.LastName        = LastName;
        request.Company         = Company;
        request.Email           = Email;
        request.Phone           = Phone;
        request.Phone2          = Phone2;
        request.MobilePhone     = MobilePhone;
        request.BirthDate       = BirthDate;
        request.MainCountry     = Country;
        request.MainAddress1    = Address1;
        request.MainAddress2    = Address2;
        request.MainCity        = City;
        request.MainState       = State;
        request.MainZip         = Zip;

        return request;
    }
    #endregion

    #region Submitting Changes
    private void SubmitChanges()
    {
        ExigoApiContext.CreateWebServiceContext().UpdateCustomer(Request_UpdateCustomer());

        IdentityAuthenticationService service = new IdentityAuthenticationService();
        service.RefreshIdentity();
    }
    #endregion

    #region Event Handlers
    public void SubmitChanges_Click(object sender, EventArgs e)
    {
        try
        {
            SubmitChanges();
            Response.Redirect(Request.Url.AbsolutePath + "?status=1");
        }
        catch(Exception exception)
        {
            Error.Type = Exigo.WebControls.ErrorMessageType.Failure;
            Error.Header = "Oops!";
            Error.Message = "We were unable to update your personal information: " + exception.Message;
        }
    }
    public void Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsolutePath);
    }
    #endregion

    #region Country/Region Population
    public Dictionary<DropDownList, DropDownList> CountryRegionDropdownsDictionary
    {
        get
        {
            Dictionary<DropDownList, DropDownList> dict = new Dictionary<DropDownList, DropDownList>();

            dict.Add(lstCountry, lstState);

            return dict;
        }
    }
    public void PopulateRegions(string countryCode, DropDownList regionList)
    {
        // Get the data
        var response = ExigoApiContext.CreateWebServiceContext().GetCountryRegions(new GetCountryRegionsRequest()
        {
            CountryCode = countryCode
        });

        // Clear the items from the region dropdown
        regionList.Items.Clear();

        // Populate the new regions into the dropdown
        foreach (RegionResponse r in response.Regions)
        {
            regionList.Items.Add(new ListItem()
            {
                Value = r.RegionCode,
                Text = r.RegionName
            });
        }
    }
    public void PopulateAllCountryRegions(string optionalCountryCode)
    {
        // Get the data
        var response = ExigoApiContext.CreateWebServiceContext().GetCountryRegions(new GetCountryRegionsRequest()
        {
            CountryCode = (!string.IsNullOrEmpty(optionalCountryCode)) ? optionalCountryCode : Identity.Current.Address.Country
        });

        foreach (KeyValuePair<DropDownList, DropDownList> pair in CountryRegionDropdownsDictionary)
        {

            // Clear the items from the dropdowns
            pair.Key.Items.Clear();
            pair.Value.Items.Clear();

            // Populate the new regions into the dropdown
            foreach (CountryResponse r in response.Countries)
            {
                pair.Key.Items.Add(new ListItem()
                {
                    Value = r.CountryCode,
                    Text = r.CountryName
                });
            }
            if (!string.IsNullOrEmpty(optionalCountryCode)) pair.Key.SelectedValue = optionalCountryCode; // Set the default country if we provided one

            // Populate the new regions into the dropdown
            foreach (RegionResponse r in response.Regions)
            {
                pair.Value.Items.Add(new ListItem()
                {
                    Value = r.RegionCode,
                    Text = r.RegionName
                });
            }
        }
    }
    public void PopulateRegions_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is DropDownList)
        {
            DropDownList regionsList;
            if (CountryRegionDropdownsDictionary.TryGetValue(((DropDownList)sender), out regionsList))
            {
                PopulateRegions(((DropDownList)sender).SelectedValue, regionsList);
            }
        }
    }
    #endregion

    #region Populate Form Fields
    private void PopulateCustomerData()
    {
        // Get the customer's data.
        var customer        = ExigoApiContext.CreateODataContext().Customers
                                .Where(c => c.CustomerID == Identity.Current.CustomerID)
                                .Select(c => new
                                {
                                    c.FirstName, 
                                    c.LastName,
                                    c.Company,
                                    c.Email,
                                    c.Phone,
                                    c.Phone2,
                                    c.MobilePhone,
                                    c.BirthDate,
                                    c.MainAddress1,
                                    c.MainAddress2,
                                    c.MainCity,
                                    c.MainState,
                                    c.MainZip,
                                    c.MainCountry
                                })
                                .FirstOrDefault();

        // Populate all fields that need to be populated first.
        PopulateAllCountryRegions(customer.MainCountry);
        PopulateBirthdayFields();


        // Now load the customer's data
        FirstName           = customer.FirstName;
        LastName            = customer.LastName;
        Company             = customer.Company;
        Email               = customer.Email;
        Phone               = customer.Phone;
        Phone2              = customer.Phone2;
        MobilePhone         = customer.MobilePhone;
        BirthDate           = GlobalUtilities.TryParse<DateTime>(customer.BirthDate, DateTime.Now.AddYears(-18));
        Country             = customer.MainCountry;
        Address1            = customer.MainAddress1;
        Address2            = customer.MainAddress2;
        City                = customer.MainCity;
        State               = customer.MainState;
        Zip                 = customer.MainZip;
    }

    private void PopulateBirthdayFields()
    {
        lstBirthMonth.Items.Clear();
        lstBirthDay.Items.Clear();
        lstBirthYear.Items.Clear();

        for(var month = 1; month <= 12; month++) 
        {
            var date = new DateTime(DateTime.Now.Year, month, 1);
            lstBirthMonth.Items.Add(new ListItem(date.ToString("MMMM"), month.ToString()));
        }

        for(var day = 1; day <= 31; day++) 
        {
            lstBirthDay.Items.Add(new ListItem(day.ToString(), day.ToString()));
        }

        var startYear = DateTime.Now.AddYears(-18).Year;
        var endYear = DateTime.Now.AddYears(-98).Year;
        for(var year = startYear; year >= endYear; year--) 
        {
            lstBirthYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
        }
    }
    #endregion
}