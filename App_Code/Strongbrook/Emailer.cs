using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net.Mail;
using Exigo.WebService;
using Exigo.OData;

/// <summary>
/// Class containing methods to send email messages
/// </summary>
public class Emailer : System.Web.UI.Page
{
	public Emailer()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string UrlProductList                        = "AutoshipProductList.aspx";

    #region email settings
    string host = "smtpout.secureserver.net";
    Int16 port = 25;
    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("support@strongbrookdirect.com", "Reic2012");
    #endregion

    /// <summary>
    /// Transmit an email using HTML, returns true if email sent successfully
    /// </summary>
    /// <param name="to">string: To Address</param>
    /// <param name="from">string: From Address</param>
    /// <param name="fromName">string: From Display Name</param>
    /// <param name="subject">string: Subject Line</param>
    /// <param name="body">StringBuilder: Message Body in HTML or Text</param>
    /// <param name="IsHtml">Bool: Is Email Message HTML</param>
    public bool SendMessage(string to, string from, string fromName, string subject, StringBuilder body, bool IsHtml)
    {
        try
        {
            MailMessage message = new MailMessage(from, to, subject, body.ToString());
            message.IsBodyHtml = IsHtml;

            SmtpClient client = new SmtpClient(host, port);

            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            client.Send(message);

            message.Dispose();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Sets Exigo SubscribeToBroadcast value to false for 1 customer based on Customer ID
    /// </summary>
    /// <param name="customerID">Int value of CustomerID</param>
    /// <returns>string completed message</returns>
    public string OptOut(int customerID)
    {
        string OptedOut = "false";

        try
        {
            var context = ExigoApiContext.CreateWebServiceContext();

            UpdateCustomerResponse res = context.UpdateCustomer(new UpdateCustomerRequest()
            {
                CustomerID = customerID,
                SubscribeToBroadcasts = false
            });

            OptedOut += "Your request has been completed successfully|";
        }
        catch
        {
            OptedOut += "We could not complete this as requested, if this continues please contact support|";
        }

        return OptedOut;
    }

    /// <summary>
    /// Sets Exigo SubscribeToBroadcast value to false for customer based on Email Address.
    /// </summary>
    /// <param name="emailAddress">string Email Address to unsubscribe</param>
    /// <returns>StringBuilder completed message</returns>
    public StringBuilder OptOut(string emailAddress)
    {
        StringBuilder returnMessage = new StringBuilder();
        StringBuilder customerList = new StringBuilder();

        string OptedOut = "false";

        try
        {
            var context = ExigoApiContext.CreateWebServiceContext();

            var query = (from c in ExigoApiContext.CreateODataContext().Customers
                         where c.Email == emailAddress
                         where c.CustomerStatusID == 1
                         where c.CustomerTypeID >= 1
                         where c.CustomerTypeID <= 4
                         select new
                         {
                             c.CustomerID,
                             c.FirstName,
                             c.LastName,
                             c.Email
                         });

            if (query.Count() > 1)
            {
                int counter = 1;
                //What to do if there is more than 1 result
                foreach (var q in query)
                {
                    string FullName = q.FirstName + " " + q.LastName;

                    if (counter != query.Count())
                    {
                        customerList.Append(string.Format(@"{0},{1},{2}|", q.CustomerID, FullName, q.Email));
                    }
                    else
                    {
                        customerList.Append(string.Format(@"{0},{1},{2}", q.CustomerID, FullName, q.Email));
                    }
                    counter++;
                    
                }
                return customerList;
            }
            if (query.Count() == 1)
            {
                foreach (var q in query)
                {
                    UpdateCustomerResponse res = context.UpdateCustomer(new UpdateCustomerRequest()
                    {
                        CustomerID = q.CustomerID,
                        SubscribeToBroadcasts = false
                    });
                }
                OptedOut = "true";

                returnMessage.AppendLine(string.Format(@"Your request has been completed successfully|
                "));
            }
            else
            {
                OptedOut = "error";
                returnMessage.AppendLine("We're sorry, we couldn't find an account with that email address|" + OptedOut);
            }
            
        }
        catch
        {
            returnMessage.AppendLine("We could not complete this as requested, if this continues please contact support|" + OptedOut);
        }

        return returnMessage;
    }

    public StringBuilder OptOut(List<string> customerIDs)
    {
        StringBuilder returnMessage = new StringBuilder();
        
        foreach (string customer in customerIDs)
        {
            try
            {
                var context = ExigoApiContext.CreateWebServiceContext();

                UpdateCustomerResponse res = context.UpdateCustomer(new UpdateCustomerRequest()
                {
                    CustomerID = Convert.ToInt32(customer),
                    SubscribeToBroadcasts = false
                });

                returnMessage.Append("Your request has been completed successfully|");
            }
            catch
            {
                returnMessage.Append("We could not complete this as requested, if this continues please contact support|");
            }
        }

        return returnMessage;
    }

    public StringBuilder OptIn(int customerID)
    {
        StringBuilder returnMessage = new StringBuilder();

        try
        {
            var context = ExigoApiContext.CreateWebServiceContext();

            //For allowing subscription using localhost
            string IP;
            if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] == "::1")
            {
                IP = "192.168.0.1";
            }
            else
            {
                IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            UpdateCustomerResponse res = context.UpdateCustomer(new UpdateCustomerRequest()
            {
                CustomerID = Convert.ToInt32(customerID),
                SubscribeToBroadcasts = true,
                SubscribeFromIPAddress = IP
            });

            returnMessage.Append("Your Request was completed successfully|");
        }
        catch
        {
            returnMessage.Append("We could not complete this as request, if this continues please contact support|");
        }

        return returnMessage;
    }
}