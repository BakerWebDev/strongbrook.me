using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

public class IdentityRanks
{
	public IdentityRanks()
	{
        // Use tasks to get the data as fast as possible.
		var tasks = new List<Task>();
        var customerID = Identity.Current.CustomerID;


        // Get the highest rank ever achieved at the close of any period.
        tasks.Add(Task.Factory.StartNew(() => {
            var data = ExigoApiContext.CreateODataContext().Customers
                .Where(c => c.CustomerID == customerID)
                .Select(c => new {
                    c.Rank
                })
                .SingleOrDefault();

            this.HighestAchievedRankID = (data != null) ? data.Rank.RankID : 0;
            this.HighestAchievedRankDescription = (data != null) ? data.Rank.RankDescription : "Unknown";
        }));


        // Get the current period ranks
        tasks.Add(Task.Factory.StartNew(() => {
            var data = ExigoApiContext.CreateODataContext().PeriodVolumes
                .Where(c => c.CustomerID == customerID)
                .Where(c => c.PeriodTypeID == PeriodTypes.Default)
                .Where(c => c.Period.IsCurrentPeriod)
                .Select(c => new {
                    c.Rank,
                    c.PaidRank
                })
                .SingleOrDefault();

            this.HighestCurrentPeriodRankID = (data != null) ? data.Rank.RankID : 0;
            this.HighestCurrentPeriodRankDescription = (data != null) ? data.Rank.RankDescription : "Unknown";

            this.CurrentPeriodRankID = (data != null) ? data.PaidRank.RankID : 0;
            this.CurrentPeriodRankDescription = (data != null) ? data.PaidRank.RankDescription : "Unknown";
        }));


        // Wait for all tasks to complete before ending the call.
        Task.WaitAll(tasks.ToArray());
        tasks.Clear();
	}

    public int CurrentPeriodRankID { get; set; }
    public string CurrentPeriodRankDescription { get; set; }

    public int HighestCurrentPeriodRankID { get; set; }
    public string HighestCurrentPeriodRankDescription { get; set; }

    public int HighestAchievedRankID { get; set; }
    public string HighestAchievedRankDescription { get; set; }
}