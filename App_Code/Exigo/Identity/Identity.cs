using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Security;

[Serializable]
public class Identity : IIdentity
{
    public static Identity Current
    {
        get
        {
            var identity = (HttpContext.Current.User.Identity as Identity);
            return identity;
        }
    }

    #region Constructors
    public Identity(System.Web.Security.FormsAuthenticationTicket ticket)
    {
        string[] a = ticket.UserData.Split('|');
        Name = ticket.Name;

        // WebIdentity Variables
        CustomerID                  = int.Parse(GlobalUtilities.Coalesce(a[0], "0"));
        EnrollerID                  = int.Parse(GlobalUtilities.Coalesce(a[1], "0"));
        SponsorID                   = int.Parse(GlobalUtilities.Coalesce(a[2], "0"));
        FirstName                   = GlobalUtilities.Coalesce(a[3], "");
        LastName                    = GlobalUtilities.Coalesce(a[4], "");
        Company                     = GlobalUtilities.Coalesce(a[5], "");
        LanguageID                  = int.Parse(GlobalUtilities.Coalesce(a[6], Languages.English.ToString()));
        CustomerTypeID              = int.Parse(GlobalUtilities.Coalesce(a[7], CustomerTypes.Distributor.ToString()));
        CustomerStatusID            = int.Parse(GlobalUtilities.Coalesce(a[8], CustomerStatusTypes.Active.ToString()));
        DefaultWarehouseID          = int.Parse(GlobalUtilities.Coalesce(a[9], Warehouses.Default.ToString()));
        PriceTypeID                 = int.Parse(GlobalUtilities.Coalesce(a[10], PriceTypes.Distributor.ToString()));
        CurrencyCode                = GlobalUtilities.Coalesce(a[11], "usd");
        JoinedDate                  = DateTime.Parse(GlobalUtilities.Coalesce(a[12], DateTime.Now.ToString()));
        PayableToName               = GlobalUtilities.Coalesce(a[13], "");

        Expires = ticket.Expiration;
    }

    // Determine the culture codes
    public string CultureCode
    {
        get
        {
            return GetBrowsersDefaultCultureCode();
        }
    }
    public string UICultureCode
    {
        get
        {
            return GetBrowsersDefaultCultureCode();
        }
    }
    #endregion

    #region Settings
    string IIdentity.AuthenticationType
    {
        get { return "Custom"; }
    }
    bool IIdentity.IsAuthenticated
    {
        get { return true; }
    }
    public string Name { get; set; }
    #endregion

    #region Properties
    public int CustomerID { get; set; }
    public int EnrollerID { get; set; }
    public int SponsorID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }  
    public int LanguageID { get; set; }
    public int CustomerTypeID { get; set; }
    public int CustomerStatusID { get; set; }
    public int DefaultWarehouseID { get; set; }
    public int PriceTypeID { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime JoinedDate { get; set; }
    public string PayableToName { get; set; }
    
    public IdentityAddress Address                      { get { return CacheHelper.GetFromCache<IdentityAddress>("Address"); } }
    public IdentityContactInformation ContactInfo       { get { return CacheHelper.GetFromCache<IdentityContactInformation>("ContactInformation"); } }
    public IdentityRanks Ranks                          { get { return CacheHelper.GetFromCache<IdentityRanks>("Ranks"); } }
    public IdentityVolumes Volumes                      { get { return CacheHelper.GetFromCache<IdentityVolumes>("Volumes"); } }
    public IdentitySubscriptions Subscriptions          { get { return CacheHelper.GetFromCache<IdentitySubscriptions>("Subscriptions"); } }
    public IdentityWebsite Website                      { get { return CacheHelper.GetFromCache<IdentityWebsite>("Website"); } }
    public IdentityEnroller Enroller                    { get { return CacheHelper.GetFromCache<IdentityEnroller>("Enroller"); } }
    public IdentitySponsor Sponsor                      { get { return CacheHelper.GetFromCache<IdentitySponsor>("Sponsor"); } }

    public string DisplayName
    {
        get { return GlobalUtilities.Coalesce(this.Company, this.FirstName + " " + this.LastName); }
    }
    public Market Market 
    {
        get
        {
            return GlobalSettings.Markets.AvailableMarkets.Where(c => c.Countries.Contains(this.Address.Country)).FirstOrDefault();
        }
    }
    public DateTime Expires { get; set; }
    #endregion

    #region Private Methods
    private string GetBrowsersDefaultCultureCode()
    {
        string[] languages = HttpContext.Current.Request.UserLanguages;

        if (languages == null || languages.Length == 0)
            return "en-US";
        try
        {
            string language = languages[0].Trim();
            return language;
        }

        catch (ArgumentException)
        {
            return "en-US";
        }
    }
    #endregion

    #region Serialization
    public static Identity Deserialize(string data)
    {
        try
        {
            var ticket = FormsAuthentication.Decrypt(data);
            return new Identity(ticket);
        }
        catch(Exception ex)
        {
            var service = new IdentityAuthenticationService();
            service.SignOut();
            return null;
        }
    }
    #endregion
}
