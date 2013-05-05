using Exigo.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CustomerProfile : System.Web.UI.Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            // If the customerID was not valid, or if this customer is not in our genealogy, tell the user that we can't find the customer they want.
            if(Customer.CustomerID == 0)
            {
                Response.Clear();
                Response.Write("We cannot find the customer you are searching for. Please contact customer service for more information.");
                Response.End();
                return;
            }
        }
    }
    #endregion

    #region Properties
    public int CustomerID
    {
        get
        {
            if(Request.QueryString["id"] == null)
                return 0;
            else
                return Convert.ToInt32(Request.QueryString["id"]);
        }
    }

    public bool IsPersonallyEnrolled
    {
        get
        {
            if(ViewState["IsPersonallyEnrolled"] == null)
            {
                ViewState["IsPersonallyEnrolled"] = ((from c in ExigoApiContext.CreateODataContext().EnrollerTree
                                                      where c.TopCustomerID == Identity.Current.CustomerID
                                                      where c.CustomerID == CustomerID
                                                      where c.EnrollerID == Identity.Current.CustomerID
                                                      select new
                                                      {
                                                          c.Customer
                                                      }).Count() > 0) || (CustomerID == Identity.Current.CustomerID);
            }
            return Convert.ToBoolean(ViewState["IsPersonallyEnrolled"]);
        }
    }

    public string FullName(Customer customer)
    {
        if(!string.IsNullOrEmpty(customer.Company))
        {
            return customer.Company;
        }
        else
        {
            return customer.FirstName + " " + customer.LastName;
        }
    }

    public Customer Customer
    {
        get
        {
            if(_customer == null)
            {
                try
                {
                    _customer = (from c in ExigoApiContext.CreateODataContext().Customers
                                 where c.CustomerID == CustomerID
                                 select new Customer()
                                 {
                                     CustomerID = c.CustomerID,
                                     FirstName = c.FirstName,
                                     LastName = c.LastName,
                                     Company = c.Company,
                                     CreatedDate = c.CreatedDate,
                                     LoginName = c.LoginName,
                                     Phone = c.Phone,
                                     Phone2 = c.Phone2,
                                     MobilePhone = c.MobilePhone,
                                     Fax = c.Fax,
                                     Email = c.Email,
                                     EnrollerID = c.EnrollerID,
                                     SponsorID = c.SponsorID,
                                     RankID = c.RankID
                                 }).FirstOrDefault();

                    _customer.EnrollerID = (_customer.EnrollerID != null) ? (int)_customer.EnrollerID : 0;
                    _customer.SponsorID = (_customer.SponsorID != null) ? (int)_customer.SponsorID : 0;
                }
                catch
                {
                    _customer = new Customer() { CustomerID = 0 };
                }
            }
            return _customer;
        }
    }
    private Customer _customer;

    public Customer Enroller
    {
        get
        {
            if(_enroller == null)
            {
                if(Customer.EnrollerID == null || Customer.EnrollerID == 0)
                {
                    _enroller = new Customer();
                }
                else
                {
                    _enroller = (from c in ExigoApiContext.CreateODataContext().Customers
                                 where c.CustomerID == Customer.EnrollerID
                                 select new Customer
                                 {
                                     CustomerID = c.CustomerID,
                                     FirstName = c.FirstName,
                                     LastName = c.LastName,
                                     Company = c.Company,
                                     EnrollerID = c.EnrollerID,
                                     SponsorID = c.SponsorID
                                 }).FirstOrDefault();

                    _enroller.EnrollerID = (_enroller.EnrollerID != null) ? (int)_enroller.EnrollerID : 0;
                    _enroller.SponsorID = (_enroller.SponsorID != null) ? (int)_enroller.SponsorID : 0;
                }
            }
            return _enroller;
        }
    }
    private Customer _enroller;

    public Customer Sponsor
    {
        get
        {
            if(_sponsor == null)
            {
                if(Customer.SponsorID == null || Customer.SponsorID == 0)
                {
                    _sponsor = new Customer();
                }
                else
                {
                    _sponsor = (from c in ExigoApiContext.CreateODataContext().Customers
                                where c.CustomerID == Customer.SponsorID
                                select new Customer
                                {
                                    CustomerID = c.CustomerID,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    Company = c.Company,
                                    EnrollerID = c.EnrollerID,
                                    SponsorID = c.SponsorID
                                }).FirstOrDefault();

                    _sponsor.EnrollerID = (_sponsor.EnrollerID != null) ? (int)_sponsor.EnrollerID : 0;
                    _sponsor.SponsorID = (_sponsor.SponsorID != null) ? (int)_sponsor.SponsorID : 0;
                }
            }
            return _sponsor;
        }
    }
    private Customer _sponsor;

    public PeriodVolume Volumes
    {
        get
        {
            if(_volumes == null)
            {
                _volumes = (from c in ExigoApiContext.CreateODataContext().PeriodVolumes
                            where c.CustomerID == CustomerID
                            where c.PeriodTypeID == (int)PeriodTypes.Default
                            where c.Period.IsCurrentPeriod == true
                            select new PeriodVolume
                                {
                                    PaidRank = c.PaidRank,
                                    Rank = c.Rank
                                }).FirstOrDefault();
            }
            return _volumes;
        }
    }
    private PeriodVolume _volumes;
    #endregion

    #region Report Properties
    protected string DisplayName
    {
        get
        {
            var displayName = string.Empty;
            if(Customer.Company == "" || Customer.Company == null)
            {
                displayName = Customer.FirstName + " " + Customer.LastName;
            }
            else
            {
                displayName = Customer.Company;
            }
            return displayName;
        }
    }
    #endregion
}