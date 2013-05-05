using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class IdentityAddress
{
	public IdentityAddress()
	{
		var data = ExigoApiContext.CreateODataContext().Customers
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Select(c => new {
                Address1 = c.MainAddress1,
                Address2 = c.MainAddress2,
                City = c.MainCity,
                State = c.MainState,
                Zip = c.MainZip,
                Country = c.MainCountry
            }).SingleOrDefault();

        if(data != null)
        {
            this.Address1           = data.Address1;
            this.Address2           = data.Address2;
            this.City               = data.City;
            this.State              = data.State;
            this.Zip                = data.Zip;
            this.Country            = data.Country;
        }
	}

    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
}