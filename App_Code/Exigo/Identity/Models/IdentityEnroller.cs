using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class IdentityEnroller
{
	public IdentityEnroller()
	{
		var data = ExigoApiContext.CreateODataContext().Customers
            .Where(c => c.CustomerID == Identity.Current.EnrollerID)
            .Select(c => new {
                c.CustomerID,
                c.FirstName,
                c.LastName,
                c.Company
            }).SingleOrDefault();

        if(data != null)
        {
            this.EnrollerID             = data.CustomerID;
            this.FirstName              = data.FirstName;
            this.LastName               = data.LastName;
            this.Company                = data.Company;
        }
	}

    public int EnrollerID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }

    public string DisplayName
    {
        get { return GlobalUtilities.Coalesce(this.Company, this.FirstName + " " + this.LastName); }
    }
}