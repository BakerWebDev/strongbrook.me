using Exigo.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CommissionsPortal : System.Web.UI.Page
{
    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion

    #region Real-Time Commission Check
    public CommissionResponse CommissionCheckDetails
    {
        get
        {
            if (_commissionCheckDetails == null)
            {

                var details = FetchRealTimeCommissions();

                if (details.Length > 0)
                {
                    _commissionCheckDetails = details
                        .Where(c => c.PeriodType == PeriodTypes.Default)
                        .FirstOrDefault();
                    if (_commissionCheckDetails == null)
                    {
                        _commissionCheckDetails = new CommissionResponse();
                    }
                    else
                    {
                         _commissionCheckDetails = details
                        .Where(c => c.PeriodType == PeriodTypes.Default)
                        .FirstOrDefault(); 
                    }
                }
                else
                {
                    _commissionCheckDetails = new CommissionResponse();
                }
            }
            return _commissionCheckDetails;
        }
    }
    private CommissionResponse _commissionCheckDetails;

    public void RenderCommissionCheckAmountInEnglish(decimal amount)
    {
        NumberToEnglish converter = new NumberToEnglish();
        string amountString = converter.ChangeCurrencyToWords(Convert.ToDouble(amount));

        Html32TextWriter writer = new Html32TextWriter(Response.Output);
        writer.Write(amountString);
    }

    private CommissionResponse[] FetchRealTimeCommissions()
    {
        try
        {
            return ExigoApiContext.CreateWebServiceContext().GetRealTimeCommissions(new GetRealTimeCommissionsRequest
            {
                CustomerID = Identity.Current.CustomerID
            }).Commissions;
        }
        catch { return new CommissionResponse[0]; }
    }
    #endregion
}