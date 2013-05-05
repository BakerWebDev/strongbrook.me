using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Exigo.OData;
using Exigo.WebService;
using System.Data.SqlClient;
using System.Data;

public interface IAuthenticationService
{
    bool SignIn(string loginName, string password);
    void SignOut();
}
public class IdentityAuthenticationService : IAuthenticationService
{
    /// <summary>
    /// Signs the customer into the backoffice.
    /// </summary>
    /// <param name="loginName">The customer's login name</param>
    /// <param name="password">The customer's password</param>
    /// <returns>Whether or not the customer was successfully signed in.</returns>
    public bool SignIn(string loginName, string password)
    {
        var response = ExigoApiContext.CreateWebServiceContext().LoginCustomer(new LoginCustomerRequest
        {
            LoginName = loginName,
            Password = password
        });

        
        if (response.Result.Status == ResultStatus.Success && !string.IsNullOrEmpty(response.SessionID)) 
        {
            var customer = ExigoApiContext.CreateODataContext().Customers
                .Where(c => c.LoginName == loginName)
                .Select(c => new {
                    c.CustomerID
                })
                .SingleOrDefault();

            if(customer == null) return false;

            return CreateFormsAuthenticationTicket(customer.CustomerID);
        }
        else return false;
    }

    /// <summary>
    /// Signs the customer into the backoffice.
    /// </summary>
    /// <param name="customerID">The customer's ID.</param>
    /// <param name="loginName">The customer's login name.</param>
    /// <returns>Whether or not the customer was successfully signed in.</returns>
    public bool SilentLogin(int customerID, string loginName)
    {
        var cust = (from c in ExigoApiContext.CreateODataContext().Customers
                    where c.CustomerID == customerID
                    where c.LoginName == loginName
                    select new Customer { CustomerID = c.CustomerID }).FirstOrDefault();

        if (cust != null) return CreateFormsAuthenticationTicket(cust.CustomerID);
        else return false;
    }

    /// <summary>
    /// Signs the customer into the backoffice.
    /// </summary>
    /// <param name="sessionID">A SessionID created by the Exigo web service's LoginCustomer method.</param>
    /// <returns>Whether or not the customer was successfully signed in.</returns>
    public bool SilentLogin(string sessionID)
    {
        var response = ExigoApiContext.CreateWebServiceContext().GetLoginSession(new GetLoginSessionRequest
        {
            SessionID = sessionID
        });

        if (response.Result.Status == ResultStatus.Success && response.CustomerID > 0) return CreateFormsAuthenticationTicket(response.CustomerID);
        else return false;
    }

    /// <summary>
    /// Refreshes the current identity.
    /// </summary>
    /// <returns>Whether or not the customer was successfully refreshed.</returns>
    public bool RefreshIdentity()
    {
        return CreateFormsAuthenticationTicket(Identity.Current.CustomerID);
    }

    /// <summary>
    /// Signs the user out of the backoffice
    /// </summary>
    public void SignOut()
    {
        FormsAuthentication.SignOut();
    }

    /// <summary>
    /// Creates the forms authentication ticket
    /// </summary>
    /// <param name="customerID">The customer ID</param>
    /// <returns>Whether or not the ticket was created successfully.</returns>
    public bool CreateFormsAuthenticationTicket(int customerID)
    {
        var data = ExigoApiContext.CreateODataContext().Customers
            .Where(c => c.CustomerID == customerID)
            .Select(c => new {
                c.CustomerID,
                c.EnrollerID,
                c.SponsorID,
                c.FirstName,
                c.LastName,
                c.Company,
                c.LanguageID,
                c.CustomerTypeID,
                c.CustomerStatusID,
                c.DefaultWarehouseID,
                c.CurrencyCode,
                c.CreatedDate,
                c.PayableToName
            }).SingleOrDefault();

        if(data == null) return false;


        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
            customerID.ToString(),
            DateTime.Now,
            DateTime.Now.AddMinutes(GlobalSettings.Backoffice.SessionTimeoutInMinutes),
            false,
            string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}",
                customerID,
                (data.EnrollerID != null) ? data.EnrollerID : 0,
                (data.SponsorID != null) ? data.SponsorID : 0,
                data.FirstName,
                data.LastName,
                data.Company,
                data.LanguageID,
                data.CustomerTypeID,
                data.CustomerStatusID,
                data.DefaultWarehouseID,
                PriceTypes.Distributor, // Price Type
                data.CurrencyCode,
                data.CreatedDate,
                data.PayableToName));

        // encrypt the ticket
        string encTicket = FormsAuthentication.Encrypt(ticket);

        // create the cookie.
        HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName]; //saved user
        if (cookie == null)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }
        else
        {
            cookie.Value = encTicket;
            HttpContext.Current.Response.Cookies.Set(cookie);
        }
        return true;
    }
}