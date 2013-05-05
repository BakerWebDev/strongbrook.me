using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class IdentityContactInformation
{
	public IdentityContactInformation()
	{
		var data = ExigoApiContext.CreateODataContext().Customers
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Select(c => new {
                c.Email,
                c.Phone,
                c.Phone2,
                c.MobilePhone,
                c.Fax
            }).SingleOrDefault();

        if(data != null)
        {
            this.Email                  = data.Email;
            this.Phone                  = data.Phone;
            this.Phone2                 = data.Phone2;
            this.MobilePhone            = data.MobilePhone;
            this.Fax                    = data.Fax;
        }
	}

    public string Email { get; set; }
    public string Phone { get; set; }
    public string Phone2 { get; set; }
    public string MobilePhone { get; set; }
    public string Fax { get; set; }
}