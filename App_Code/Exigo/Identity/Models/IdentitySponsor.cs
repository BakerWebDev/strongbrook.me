using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class IdentitySponsor
{
	public IdentitySponsor()
	{
        var data = ExigoApiContext.CreateODataContext().Customers
            .Where(c => c.CustomerID == Identity.Current.SponsorID)
            .Select(c => new {
                c.CustomerID,
                c.FirstName,
                c.LastName,
                c.Company
            }).SingleOrDefault();

        if(data != null)
        {
            this.SponsorID              = data.CustomerID;
            this.FirstName              = data.FirstName;
            this.LastName               = data.LastName;
            this.Company                = data.Company;
        }
	}

    public int SponsorID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }

    public string DisplayName
    {
        get { return GlobalUtilities.Coalesce(this.Company, this.FirstName + " " + this.LastName); }
    }
}