using Exigo.WebControls;
using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WebsiteSettings : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            PopulateCustomerData();
        }

        if(Request.QueryString["status"] == "1")
        {
            Error.Type = ErrorMessageType.Success;
            Error.Header = "Success!";
            Error.Message = "Your information has been saved.";
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
        get { return txtCompany.Text; }
        set { txtCompany.Text = value; }
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
    public string Email
    {
        get { return txtEmail.Text.FormatForExigo(ExigoDataFormatType.Email); }
        set { txtEmail.Text = value; }
    }

    public string Address
    {
        get { return txtAddress.Text.FormatForExigo(); }
        set { txtAddress.Text = value; }
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

    public string Notes
    {
        get { return txtNotes.Text; }
        set { txtNotes.Text = value; }
    }
    
    public string Facebook
    {
        get { return txtFacebook.Text; }
        set { txtFacebook.Text = value; }
    }
    public string Twitter
    {
        get { return txtTwitter.Text; }
        set { txtTwitter.Text = value; }
    }
    public string LinkedIn
    {
        get { return txtLinkedIn.Text; }
        set { txtLinkedIn.Text = value; }
    }
    public string YouTube
    {
        get { return txtYouTube.Text; }
        set { txtYouTube.Text = value; }
    }
    public string Pinterest
    {
        get { return txtPinterest.Text; }
        set { txtPinterest.Text = value; }
    }
    #endregion

    #region Event Handlers
    public void Submit_Click(object sender, EventArgs e)
    {
        try
        {
            if(uploadPhoto.HasFile)
            {
                var photobytes = uploadPhoto.FileBytes;


                var helper = new ImageUploadHelper();
                helper.UploadImage(photobytes, string.Empty, true);


                ExigoApiContext.CreateWebServiceContext().SetCustomerSiteImage(new SetCustomerSiteImageRequest
                {
                    CustomerID = Identity.Current.CustomerID,
                    CustomerSiteImageType = CustomerSiteImageType.Primary,
                    ImageName = "avatar",
                    ImageData = GlobalUtilities.ResizeImage(photobytes, GlobalSettings.CustomerImages.MaxImageWidth, GlobalSettings.CustomerImages.MaxImageHeight)
                });
            }
        
            // Save the customer site information
            ExigoApiContext.CreateWebServiceContext().SetCustomerSite(new SetCustomerSiteRequest
            {
                CustomerID = Identity.Current.CustomerID,
                WebAlias = Identity.Current.Website.WebAlias,
                FirstName = FirstName,
                LastName = LastName,
                Company = Company,
                Address1 = Address,
                City = City,
                State = State,
                Zip = Zip,
                Country = Country,
                Phone = Phone,
                Phone2 = Phone2,
                Email = Email,
                Notes1 = Notes
            });

            // Save the social network information
            var socialrequest = new SetCustomerSocialNetworksRequest();
            socialrequest.CustomerID = Identity.Current.CustomerID;

            var urls = new List<CustomerSocialNetworkRequest>();
            if(!string.IsNullOrEmpty(Facebook))     urls.Add(new CustomerSocialNetworkRequest() { SocialNetworkID = (int)SocialNetworks.Facebook,       Url = Facebook });
            if(!string.IsNullOrEmpty(Twitter))      urls.Add(new CustomerSocialNetworkRequest() { SocialNetworkID = (int)SocialNetworks.Twitter,        Url = Twitter });
            if(!string.IsNullOrEmpty(LinkedIn))     urls.Add(new CustomerSocialNetworkRequest() { SocialNetworkID = (int)SocialNetworks.LinkedIn,       Url = LinkedIn });
            if(!string.IsNullOrEmpty(YouTube))      urls.Add(new CustomerSocialNetworkRequest() { SocialNetworkID = (int)SocialNetworks.YouTube,        Url = YouTube });
            if(!string.IsNullOrEmpty(Pinterest))    urls.Add(new CustomerSocialNetworkRequest() { SocialNetworkID = (int)SocialNetworks.Pinterest,      Url = Pinterest });
            socialrequest.CustomerSocialNetworks = urls.ToArray();

            if(socialrequest.CustomerSocialNetworks.Length > 0)
            {
                ExigoApiContext.CreateWebServiceContext().SetCustomerSocialNetworks(socialrequest);
            }



            Response.Redirect(Request.Url.AbsolutePath + "?status=1");
        }
        catch(Exception ex)
        {
            Error.Type = ErrorMessageType.Failure;
            Error.Header = "Oops!";
            Error.Message = "We were unable to save your changes: " + ex.Message;
        }
    }
    #endregion

    #region Populate Existing Data
    private void PopulateCustomerData()
    {
        try
        {
            var customersite = ExigoApiContext.CreateWebServiceContext().GetCustomerSite(new GetCustomerSiteRequest
            {
                CustomerID = Identity.Current.CustomerID
            });

            var customersocial = ExigoApiContext.CreateWebServiceContext().GetCustomerSocialNetworks(new GetCustomerSocialNetworksRequest
            {CustomerID = Identity.Current.CustomerID

            });

            PopulateAllCountryRegions(customersite.Country);

            FirstName = customersite.FirstName;
            LastName = customersite.LastName;
            Company = customersite.Company;
            Phone = customersite.Phone;
            Phone2 = customersite.Phone2;
            Email = customersite.Email;

            Address = customersite.Address1;
            City = customersite.City;
            State = customersite.State;
            Zip = customersite.Zip;
            Country = customersite.Country;

            Notes = customersite.Notes1;

            foreach(var social in customersocial.CustomerSocialNetwork)
            {
                switch(social.SocialNetworkID)
                {
                    case (int)SocialNetworks.Facebook:          Facebook = social.Url; break;
                    case (int)SocialNetworks.Twitter:           Twitter = social.Url; break;
                    case (int)SocialNetworks.LinkedIn:          LinkedIn = social.Url; break;
                    case (int)SocialNetworks.YouTube:           YouTube = social.Url; break;
                    case (int)SocialNetworks.Pinterest:         Pinterest = social.Url; break;
                }
            }
        }
        catch 
        {
            PopulateAllCountryRegions(Identity.Current.Address.Country);
        }

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
}