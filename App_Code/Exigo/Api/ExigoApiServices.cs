using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Data.Services.Client;
using Exigo.WebService;
using Exigo.OData;
using System.Data.Entity;

public static class ExigoApiContext
{
    #region Properties
    private static string LoginName = GlobalSettings.ExigoApiCredentials.LoginName;
    private static string Password = GlobalSettings.ExigoApiCredentials.Password;
    private static string Company = GlobalSettings.ExigoApiCredentials.CompanyKey;
    #endregion Properties

    #region Contexts
    public static ExigoApi CreateWebServiceContext()
    {
        return new ExigoApi
        {
            ApiAuthenticationValue = new ApiAuthentication
            {
                LoginName = LoginName,
                Password = Password,
                Company = Company
            }
        };
    }
    
    public static ExigoContext CreateODataContext()
    {
        var context = new ExigoContext(new Uri("http://api.exigo.com/4.0/" + Company + "/model"));
        context.IgnoreMissingProperties = true;
        context.IgnoreResourceNotFoundException = true;
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(LoginName + ":" + Password));
        context.SendingRequest +=
            (object s, SendingRequestEventArgs e) =>
                e.RequestHeaders.Add("Authorization", "Basic " + credentials);
        return context;
    }

    public static Exigo.OData.Extended.extendeddatacontext CreateCustomODataContext()
    {
        var context = new Exigo.OData.Extended.extendeddatacontext(new Uri("http://api.exigo.com/4.0/" + Company + "/db/extendeddatacontext"));
        context.IgnoreMissingProperties = true;
        context.IgnoreResourceNotFoundException = true;
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(LoginName + ":" + Password));
        context.SendingRequest +=
            (object s, SendingRequestEventArgs e) =>
                e.RequestHeaders.Add("Authorization", "Basic " + credentials);
        return context;
    }

    public static Exigo.CustomOData.CustomerData GetDownlineOrdersODataContext()
    {
        return new Exigo.CustomOData.CustomerData();
    }



    public static ExigoPaymentApi CreatePaymentContext()
    {
        return new ExigoPaymentApi();
    }

    public static ExigoImageApi CreateImagesContext()
    {
        return new ExigoImageApi();
    }
    #endregion Contexts
}

#region Supporting Classes
public class CustomerData
{
    private static string LoginName = GlobalSettings.ExigoApiCredentials.LoginName;
    private static string Password = GlobalSettings.ExigoApiCredentials.Password;
    private static string Company = GlobalSettings.ExigoApiCredentials.CompanyKey;

    public void client()
    {
        var request = new ExigoContext(new Uri("http://api.exigo.com/4.0/" + "strongbrook" + "/report"));
        request.IgnoreMissingProperties = true;
        request.IgnoreResourceNotFoundException = true;
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(LoginName + ":" + Password));
        request.SendingRequest +=
            (object s, SendingRequestEventArgs e) =>
                e.RequestHeaders.Add("Authorization", "Basic " + credentials);
        var response = request;
    }
}

public class ExigoPaymentApi
{
    private string PaymentLoginName = GlobalSettings.ExigoPaymentApiCredentials.LoginName;
    private string PaymentPassword = GlobalSettings.ExigoPaymentApiCredentials.Password;

    /// <summary>
    /// Generate and return a new token to be used in an Exigo credit card transaction.
    /// </summary>
    /// <param name="creditCardNumber">The credit card number you wish to use for this transaction</param>
    /// <param name="expirationMonth">The expiration month of the credit card you wish to use for this transaction</param>
    /// <param name="expirationYear">The expiration year of the credit card you wish to use for this transaction</param>
    /// <returns></returns>
    public string FetchCreditCardToken(string creditCardNumber, int expirationMonth, int expirationYear)
    {
        XNamespace ns = "http://payments.exigo.com";
        var xRequest = new XDocument(
            new XElement(ns + "CreditCard",
                new XElement(ns + "CreditCardNumber", creditCardNumber),
                new XElement(ns + "ExpirationMonth", expirationMonth),
                new XElement(ns + "ExpirationYear", expirationYear)
                ));
        var xResponse = PostRest("https://payments.exigo.com/2.0/token/rest/CreateCreditCardToken", PaymentLoginName, PaymentPassword, xRequest);

        return xResponse.Root.Element(ns + "CreditCardToken").Value;
    }
    private XDocument PostRest(string url, string username, string password, XDocument element)
    {
        string postData = element.ToString();

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password)));
        request.Method = "POST";
        request.ContentLength = postData.Length;
        request.ContentType = "text/xml";

        var writer = new StreamWriter(request.GetRequestStream());
        writer.Write(postData);
        writer.Close();

        try
        {
            var response = (HttpWebResponse)request.GetResponse();
            using (var responseStream = new StreamReader(response.GetResponseStream()))
            {
                return XDocument.Parse(responseStream.ReadToEnd());
            }
        }
        catch (WebException ex)
        {
            var response = (HttpWebResponse)ex.Response;
            if (response.StatusCode == HttpStatusCode.Unauthorized) throw new Exception("Invalid Credentials");
            using (var responseStream = new StreamReader(ex.Response.GetResponseStream()))
            {
                XNamespace ns = "http://schemas.microsoft.com/ws/2005/05/envelope/none";
                XDocument doc = XDocument.Parse(responseStream.ReadToEnd());
                throw new Exception(doc.Root.Element(ns + "Reason").Element(ns + "Text").Value);
            }
        }
    }
}

public class ExigoImageApi
{
    public void SaveImage(string path, string filename, byte[] contents)
    {
        var request = (HttpWebRequest)WebRequest.Create("http://api.exigo.com/4.0/" + GlobalSettings.ExigoApiCredentials.CompanyKey + "/images" + (path.StartsWith("/") ? "" : "/") + path + "/" + filename);
        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(GlobalSettings.ExigoApiCredentials.LoginName + ":" + GlobalSettings.ExigoApiCredentials.Password)));
        request.Method = "POST";
        request.ContentLength = contents.Length;
        var writer = request.GetRequestStream();
        writer.Write(contents, 0, contents.Length);
        writer.Close();
        var response = (HttpWebResponse)request.GetResponse();
    }
}
#endregion