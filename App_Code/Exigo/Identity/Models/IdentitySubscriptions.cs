using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class IdentitySubscriptions
{
	public IdentitySubscriptions()
	{
        // Fetch our data
		var data = ExigoApiContext.CreateODataContext().CustomerSubscriptions
            .Where(c => c.CustomerID == Identity.Current.CustomerID)
            .Select(c => c);


        // Create a blank subscription
        this.AnnualSubscription = new IdentitySubscription();
        this.Subscriptions = new List<IdentitySubscription>();
        if(data == null || data.Count() == 0) return;

        
        // Compile all the subscriptions into one collection
        Subscriptions = data.Select(c => new IdentitySubscription()
        {
            SubscriptionID = c.SubscriptionID,
            IsActive = (c.SubscriptionStatusID == 0),
            StartDate = c.StartDate,
            ExpirationDate = c.ExpireDate
        }).ToList();


        // Single out any subscriptions you know you will use alot for easier access.
        var annualSubscriptionID = 10;
        this.AnnualSubscription = this.Subscriptions
            .Where(c => c.SubscriptionID == annualSubscriptionID)
            .SingleOr(new IdentitySubscription()
            {
                SubscriptionID = annualSubscriptionID,
                IsActive = false,
                StartDate = new DateTime(),
                ExpirationDate = new DateTime()
            });
	}

    public List<IdentitySubscription> Subscriptions { get; set; }
    public IdentitySubscription AnnualSubscription { get; set; }
}

public class IdentitySubscription
{
    public int SubscriptionID { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
}