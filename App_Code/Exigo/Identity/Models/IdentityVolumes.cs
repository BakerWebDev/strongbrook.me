using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

public class IdentityVolumes
{
	public IdentityVolumes()
	{
        var data = ExigoApiContext.CreateODataContext().PeriodVolumes
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Where(c => c.PeriodTypeID == PeriodTypes.Default)
            .Where(c => c.Period.IsCurrentPeriod)
            .Select(c => new {
                c.Volume1,
                c.Volume2,
                c.Volume3,
                c.Volume4
            })
            .SingleOrDefault();

        if(data == null) return;

		this.Volume1 = data.Volume1;
        this.Volume2 = data.Volume2;
        this.Volume3 = data.Volume3;
        this.Volume4 = data.Volume4;
	}

    public decimal Volume1 { get; set; }
    public decimal Volume2 { get; set; }
    public decimal Volume3 { get; set; }
    public decimal Volume4 { get; set; }
}