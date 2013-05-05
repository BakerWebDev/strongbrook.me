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
using Exigo.CustomOData;
using System.Data.Entity;

public static class ExigoApiServicesToo
{
    #region Properties
    private static string LoginName = GlobalSettings.ExigoApiCredentials.LoginName;
    private static string Password = GlobalSettings.ExigoApiCredentials.Password;
    private static string Company = GlobalSettings.ExigoApiCredentials.CompanyKey;
    #endregion Properties

    public static ExigoContext CreateODataContext()
    {
        var context = new ExigoContext(new Uri("http://api.exigo.com/4.0/" + Company + "/report"));
        context.IgnoreMissingProperties = true;
        context.IgnoreResourceNotFoundException = true;
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(LoginName + ":" + Password));
        context.SendingRequest +=
            (object s, SendingRequestEventArgs e) =>
                e.RequestHeaders.Add("Authorization", "Basic " + credentials);
        return context;
    }
}