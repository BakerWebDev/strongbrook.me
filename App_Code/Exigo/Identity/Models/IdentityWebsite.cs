using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

public class IdentityWebsite
{
	public IdentityWebsite()
	{
        // Because there are more than one API call here, let's do this in Tasks.
        var tasks = new List<Task>();
        var customerID = Identity.Current.CustomerID;

        // Get the website information
        tasks.Add(Task.Factory.StartNew(() => {
            try
            {
                var data = ExigoApiContext.CreateWebServiceContext().GetCustomerSite(new GetCustomerSiteRequest
                {
                    CustomerID = customerID
                });

                this.WebAlias = data.WebAlias;
            }
            catch {}
        }));


        // Get the login information
        tasks.Add(Task.Factory.StartNew(() => {
            var data = ExigoApiContext.CreateODataContext().Customers
                .Where(c => c.CustomerID == customerID)
                .Select(c => new {
                    c.LoginName
                })
                .SingleOrDefault();
            if(data == null) return;

            this.LoginName = data.LoginName;
        }));


        // Wait for all tasks to complete before ending this call.
        Task.WaitAll(tasks.ToArray());
        tasks.Clear();
	}

    public string WebAlias { get; set; }
    public string LoginName { get; set; }
}