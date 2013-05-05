using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Strongbrook.Bonus;
//using Exigo.API;
using Exigo.OData;
using Exigo.WebService;

namespace Strongbrook.Ranks
{
    /// <summary>
    /// Summary description for RankQualifications
    /// </summary>
    public class RankQualifications
    {
    	public RankQualifications()
    	{
    		//
    		// TODO: Add constructor logic here
    		//
    	}

        public StringBuilder DisplayRankQualifications(int toCustomer, int periodType)
        {
            StringBuilder s = new StringBuilder();

            if (toCustomer == 0)
            {
                s.AppendLine("You must supply a Customer ID");
            }
            if (periodType == 0)
            {
                s.AppendLine("You must supply a Period Type");
            }
            if (s.Length != 0)
            {
                return s;
            }
            else
            {
                Bonus.Bonus bon = new Bonus.Bonus();

                KeyValuePair<int, string> periodKVP = bon.GetCurrentPeriodID(periodType);
                List<GetRankQualificationsResponse> qualifications = bon.GetAllRankQualifications(toCustomer, periodType, periodKVP.Key);

//                foreach (var q in qualifications.Where(k => k.Qualifies == true).OrderByDescending(k => k.RankID).Take(1))
//                {
//                    s.AppendLine(string.Format(@"
//                    <table>
//                        <tr>
//                            <td>Current Rank Description</td>
//                            <td>{0}</td>
//                        </tr>
//                        <tr>
//                            <td>Qualifies?</td>
//                            <td>{1}</td>
//                        </tr>
//                        <tr>
//                            <td>Next Rank</td>
//                            <td>{2}</td>
//                        </tr>
//                        <tr>
//                            <td>Previous Rank</td>
//                            <td>{3}</td>
//                        </tr>
//                    </table>
//                    ", q.RankDescription, q.Qualifies, q.NextRankDescription, q.BackRankDescription));
//                }

                foreach (var q in qualifications)
                {
                    s.AppendLine(string.Format(@"
                    <div class=""month"">
                        <table>
                            <tr>
                                <td>Current Rank Description</td>
                                <td>{0}</td>
                            </tr>
                            <tr>
                                <td>Qualifies?</td>
                                <td>{1}</td>
                            </tr>
                            <tr>
                                <td>Next Rank</td>
                                <td>{2}</td>
                            </tr>
                            <tr>
                                <td>Previous Rank</td>
                                <td>{3}</td>
                            </tr>
                        </table>
                    </div>
                    <br />
                    ", q.RankDescription, q.Qualifies, q.NextRankDescription, q.BackRankDescription));
                }
            }

            return s;
        }

        public StringBuilder DisplayRankQualifications(int toCustomer, int periodType ,bool currentPeriod, int rankID)
        {
            StringBuilder s = new StringBuilder();

            if (toCustomer == 0)
            {
                s.AppendLine("You must supply a Customer ID");
            }
            if (periodType == 0)
            {
                s.AppendLine("You must supply a Period Type");
            }
            if (currentPeriod == false)
            {
                s.AppendLine("You must supply a Period ID");
            }
            if (rankID == 0)
            {
                s.AppendLine("You must supply a Rank ID");
            }
            if (s.Length != 0)
            {
                return s;
            }
            else
            {
                Bonus.Bonus bon = new Bonus.Bonus();

            }

            return s;
        }

        public StringBuilder DisplayRankQualifications(int toCustomer, int periodType, int currentPeriod, bool rankID)
        {
            StringBuilder s = new StringBuilder();

            if (toCustomer == 0)
            {
                s.AppendLine("You must supply a Customer ID");
            }
            if (periodType == 0)
            {
                s.AppendLine("You must supply a Period Type");
            }
            if (currentPeriod == 0)
            {
                s.AppendLine("You must supply a Period ID");
            }
            if (rankID == false)
            {
                s.AppendLine("You must supply a Rank ID");
            }
            if (s.Length != 0)
            {
                return s;
            }
            else
            {
                Bonus.Bonus bon = new Bonus.Bonus();

            }

            return s;

        }

        public StringBuilder DisplayRankQualifications(int toCustomer, int periodType, int periodID, int rankID)
        {
            StringBuilder s = new StringBuilder();

            if (periodID == 0)
            {
                s.AppendLine("You must supply a Period ID");
            }
            if (rankID == 0)
            {
                s.AppendLine("You must supply a Rank ID");
            }
            if (toCustomer == 0)
            {
                s.AppendLine("You must supply a Customer ID");
            }
            if (periodType == 0)
            {
                s.AppendLine("You must supply a Period Type");
            }
            if (s.Length != 0)
            {
                return s;
            }
            else
            {

            }

            return s;
        }
    }
}