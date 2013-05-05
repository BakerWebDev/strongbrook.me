using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Exigo;
//using Exigo.API;
//using Exigo.Settings;
using Exigo.OData;
using Exigo.WebService;

/// <summary>
/// Summary description for Bonus
/// </summary>
namespace Strongbrook.Bonus
{
    public class Bonus
    {
        public Bonus()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region private properties
        private int _periodTypeID;
        private int _periodID;
        private string _bonusCode;
        private string _toCustomer;
        private string _fromCustomer;
        private int _earnedAmount;
        private List<string> weeklyBonuses = new List<string>() { "rc", "wtb", "wec" };
        private List<string> monthlyBonuses = new List<string>() { "mec", "mtb", "smb", "reb" };
        private List<string> quarterlyBonuses = new List<string>() { "bbp" };

        //private ExigoApiServices _api;
        
        //settings to get from Bonus settings
        public string itemCode;
        #endregion

        #region public properties
        public int PeriodTypeID
        {
            get { return _periodTypeID; }
            set { _periodTypeID = value; }
        }
        public int PeriodID
        {
            get { return _periodID; }
            set { _periodID = value; }
        }
        public string BonusCode
        {
            get { return _bonusCode; }
            set { _bonusCode = value; }
        }
        public string ToCustomer
        {
            get { return _toCustomer; }
            set { _toCustomer = value; }
        }
        public int EarnedAmount
        {
            get { return _earnedAmount; }
            set { _earnedAmount = value; }
        }
        //public ExigoApiServices api
        //{
        //    get
        //    {
        //        if (_api == null)
        //        {
        //            _api = new ExigoApiServices();
        //        }
        //        return _api;
        //    }
        //}
        #endregion

        public Node GetCustomerData(Node customerNode)
        {
            var context = ExigoApiContext.CreateODataContext();
            var cQuery = (from c in context.Customers
                          where c.CustomerID == customerNode.CustomerID
                          select new
                          {
                              c.FirstName,
                              c.LastName,
                              c.SponsorID,
                              c.EnrollerID,
                              c.CustomerTypeID,
                              c.CustomerStatusID,
                              c.RankID,
                              c.CreatedDate,
                          }).FirstOrDefault();

            customerNode.FirstName = cQuery.FirstName;
            customerNode.LastName = cQuery.LastName;
            customerNode.SponsorID = (int)cQuery.SponsorID;
            customerNode.EnrollerID = (int)cQuery.EnrollerID;
            customerNode.CustomerType = cQuery.CustomerTypeID;
            customerNode.RankID = cQuery.RankID;
            customerNode.CreatedDate = cQuery.CreatedDate;
            customerNode.CustomerStatus = cQuery.CustomerStatusID;

            return customerNode;

        }

        public Node GetCommissionsPaidToPersonFromIDInPreviousPeriod(string fromCustomerID, string toCustomerID, int periodID)
        {
            Node customerNode = new Node();
            List<NodeCommissions> comms = new List<NodeCommissions>();

            var context = ExigoApiContext.CreateODataContext();
            var commissionsQuery = (from c in context.CommissionDetails
                                    where c.FromCustomerID == Convert.ToInt32(fromCustomerID)
                                    where c.CustomerID == Convert.ToInt32(toCustomerID)
                                    where c.CommissionRun.PeriodID == periodID
                                    select new
                                    {
                                        c.Bonus.BonusDescription,
                                        c.OrderID,
                                        c.SourceAmount,
                                        c.Percentage,
                                        c.CommissionAmount,
                                        c.FromCustomer.FirstName,
                                        c.FromCustomer.LastName,
                                        c.FromCustomer.CreatedDate
                                    });

            if (commissionsQuery.Count() > 0)
            {
                foreach (var cq in commissionsQuery)
                {
                    if (customerNode.CustomerID != 0)
                    {
                        customerNode.CustomerID = Convert.ToInt32(fromCustomerID);
                        customerNode.FirstName = cq.FirstName;
                        customerNode.LastName = cq.LastName;
                    }
                    
                    NodeCommissions comm = new NodeCommissions()
                    {
                        BonusAmount = cq.CommissionAmount,
                        BonusType = cq.BonusDescription,
                        PercentagePaid = cq.Percentage
                    };
                    comms.Add(comm);
                }
                customerNode.BonusDetails = comms.ToArray();
            }

            return customerNode;
        }

        public bool IsIDPayingInPeriod(int fromCustomerID, int toCustomerID, int periodID)
        {
            bool pay = false;

            var context = ExigoApiContext.CreateODataContext();
            var commissionQuery = (from c in context.CommissionDetails
                                   where c.FromCustomerID == Convert.ToInt32(fromCustomerID)
                                   where c.CustomerID == Convert.ToInt32(toCustomerID)
                                   where c.CommissionRun.PeriodID == periodID
                                   select new
                                   {
                                       c.BonusID
                                   });

            if (commissionQuery.Count() > 0)
            {
                pay = true;
            }

            return pay;
        }

        public KeyValuePair<int, string> GetCurrentPeriodID(int periodTypeID)
        {
            var context = ExigoApiContext.CreateODataContext();
            var periodQuery = (from p in context.Periods
                               where p.PeriodTypeID == periodTypeID
                               where p.IsCurrentPeriod == true
                               select new
                               {
                                   p.PeriodID,
                                   p.PeriodDescription
                               }).FirstOrDefault();

            KeyValuePair<int, string> period = new KeyValuePair<int, string>(periodQuery.PeriodID, periodQuery.PeriodDescription);

            return period;
        }

        public decimal GetTotalCommissionsEarnedForLevel(string toCustomerID, int periodID, int periodType, int level)
        {
            decimal totalCommissions = 0;

            int skipCounter = 0;
            int pageCounter = 50;

            while (pageCounter == 50)
            {
                var context = ExigoApiContext.CreateODataContext();
                var commissionsQuery = (from c in context.CommissionDetails
                                        where c.CustomerID == Convert.ToInt32(toCustomerID)
                                        where c.CommissionRun.PeriodID == periodID
                                        where c.CommissionRun.PeriodTypeID == periodType
                                        where c.Level == level
                                        select new
                                        {
                                            c.CommissionAmount
                                        }).Skip(skipCounter).Take(50);

                skipCounter = pageCounter;
                pageCounter = commissionsQuery.Count();

                if (commissionsQuery.Count() > 0)
                {
                    foreach (var a in commissionsQuery)
                    {
                        totalCommissions += a.CommissionAmount;
                    }
                }
            }

            return totalCommissions;
        }

        public List<Order> GetListOfQualifiedOrders(DateTime beginningDate, DateTime endDate)
        {
            List<Order> qualifiedOrders = new List<Order>();

            return qualifiedOrders;
        }

        public List<Order> GetListOfQualifiedOrders(int periodID, int periodTypeID)
        {
            List<Order> qualifiedOrders = new List<Order>();


            return qualifiedOrders;
        }

        /// <summary>
        /// Get List of orders for a specific period for a specific customer
        /// </summary>
        /// <param name="periodID">Period ID</param>
        /// <param name="periodTypeID">Period Type ID</param>
        /// <param name="customerID">Customer ID</param>
        /// <returns></returns>
        public List<Order> GetListOfQualifiedOrders(int periodID, int periodTypeID, int customerID)
        {
            //Get start and end dates for period
            var context = ExigoApiContext.CreateODataContext();
            var dateQuery = (from d in context.Periods
                             where d.PeriodID == periodID
                             where d.PeriodTypeID == periodTypeID
                             select new
                             {
                                 d.StartDate,
                                 d.EndDate
                             }).FirstOrDefault();

            DateTime startDate = dateQuery.StartDate;
            DateTime endDate = dateQuery.EndDate;

            List<Order> qualifiedOrders = new List<Order>();

            int skipCounter = 0;
            int takeCounter = 50;

            while (takeCounter == 50)
            {
                var orderQuery = (from o in context.Orders
                                  where o.CustomerID == customerID
                                  where o.OrderDate >= startDate
                                  where o.OrderDate <= endDate
                                  where o.OrderStatusID >= 7
                                  where o.OrderStatusID <= 9
                                  select new
                                  {
                                      o.OrderID,
                                      o.CustomerID,
                                      o.OrderDate,
                                      o.Details,
                                      o.BusinessVolumeTotal,
                                      o.CommissionableVolumeTotal,
                                      o.Other1Total, //Strongbrook TV Subscription (Active)
                                      o.Other2Total, //Purchased Kit (Active)
                                      o.Other3Total, //Volume added to BBP (BBP)
                                      o.Other4Total, //Retail Commission (RC)
                                      o.Other5Total, //Transaction Credit (HTC)
                                      o.Other6Total, //BB Poo Qualify (BBP)
                                      o.Other7Total,
                                      o.Other8Total,
                                      o.Other9Total,
                                      o.Other10Total
                                  }).Skip(skipCounter).Take(50);
                skipCounter += takeCounter;
                takeCounter = orderQuery.Count();

                foreach (var q in orderQuery)
                {
                    Order order = new Order()
                    {
                        OrderID = q.OrderID,
                        CustomerID = q.CustomerID,
                        OrderDate = q.OrderDate,
                        Details = q.Details,
                        BusinessVolumeTotal = q.BusinessVolumeTotal,
                        CommissionableVolumeTotal = q.CommissionableVolumeTotal,
                        Other10Total = q.Other10Total,
                        Other1Total = q.Other1Total,
                        Other2Total = q.Other2Total,
                        Other3Total = q.Other3Total,
                        Other4Total = q.Other4Total,
                        Other5Total = q.Other5Total,
                        Other6Total = q.Other6Total,
                        Other7Total = q.Other7Total,
                        Other8Total = q.Other8Total,
                        Other9Total = q.Other9Total
                    };

                    qualifiedOrders.Add(order);
                }
            }

            return qualifiedOrders;
        }

        public List<Order> GetListOfQualifiedOrdersHTC(int periodID, int periodTypeID, string itemCode)
        {
            //Get start and end dates for period entered
            var context = ExigoApiContext.CreateODataContext();
            var dateQuery = (from d in context.Periods
                             where d.PeriodID == periodID
                             where d.PeriodTypeID == periodTypeID
                             select new
                             {
                                 d.StartDate
                             }).FirstOrDefault();

            DateTime startDate = dateQuery.StartDate;

            List<Order> qualifiedOrders = new List<Order>();

            int skipCounter = 0;
            int takeCounter = 50;

            while (takeCounter == 50)
            {
                var orderQuery = (from o in context.OrderDetails
                                  where o.ItemCode == itemCode
                                  where o.Order.OrderDate >= startDate
                                  where o.Order.OrderStatusID >= 7
                                  where o.Order.OrderStatusID <= 9
                                  select new
                                  {
                                      o.OrderID,
                                      o.Order.CustomerID,
                                      o.Order.FirstName,
                                      o.Order.LastName,
                                      o.Order.OrderDate
                                  }).Skip(skipCounter).Take(50);
                skipCounter += takeCounter;
                takeCounter = orderQuery.Count();

                foreach (var q in orderQuery)
                {
                    Order order = new Order()
                    {
                        OrderID = q.OrderID,
                        CustomerID = q.CustomerID,
                        FirstName = q.FirstName,
                        LastName = q.LastName,
                        OrderDate = q.OrderDate
                    };

                    qualifiedOrders.Add(order);
                }
            }

            return qualifiedOrders;
        }

        public List<Order> GetListOfQualifiedOrders(int periodID, int periodTypeID, string itemCode)
        {
            //Get start and end dates for period entered
            var context = ExigoApiContext.CreateODataContext();
            var dateQuery = (from d in context.Periods
                             where d.IsCurrentPeriod == true
                             where d.PeriodTypeID == periodTypeID
                             select new
                             {
                                 d.StartDate,
                                 d.EndDate
                             }).FirstOrDefault();

            DateTime startDate = dateQuery.StartDate;
            DateTime endDate = dateQuery.EndDate;

            List<Order> qualifiedOrders = new List<Order>();

            int skipCounter = 0;
            int takeCounter = 50;

            while (takeCounter == 50)
            {
                var orderQuery = (from o in context.OrderDetails
                                  where o.ItemCode == itemCode
                                  where o.Order.OrderDate >= startDate
                                  where o.Order.OrderDate <= endDate
                                  where o.Order.OrderStatusID >= 7
                                  where o.Order.OrderStatusID <= 9
                                  select new
                                  {
                                      o.OrderID,
                                      o.Order.CustomerID,
                                      o.Order.FirstName,
                                      o.Order.LastName,
                                      o.Order.OrderDate
                                  }).Skip(skipCounter).Take(50);
                skipCounter += takeCounter;
                takeCounter = orderQuery.Count();

                foreach (var q in orderQuery)
                {
                    Order order = new Order()
                    {
                        OrderID = q.OrderID,
                        CustomerID = q.CustomerID,
                        FirstName = q.FirstName,
                        LastName = q.LastName,
                        OrderDate = q.OrderDate
                    };

                    qualifiedOrders.Add(order);
                }
            }

            return qualifiedOrders;
        }

        public List<Customer> GetDownlineEnrollerTree(string topEnrollerID)
        {
            List<Customer> enrollerDownline = new List<Customer>();

            return enrollerDownline;
        }

        public List<Customer> GetDownlineUnilevelTree(string topUnilevelID)
        {
            List<Customer> unilevelDownline = new List<Customer>();

            int skipCounter = 0;
            int takeCounter = 50;

            while (takeCounter == 50)
            {
                var context = ExigoApiContext.CreateODataContext();
                var query = (from c in context.UniLevelTree
                             where c.TopCustomerID == Convert.ToInt32(topUnilevelID)
                             select new
                             {
                                 c.CustomerID
                             }).Skip(skipCounter).Take(50);

                skipCounter += takeCounter;
                takeCounter = query.Count();

                if (query.Count() > 0)
                {
                    foreach (var c in query)
                    {
                        Customer customer = new Customer()
                        {
                            CustomerID = c.CustomerID
                        };

                        unilevelDownline.Add(customer);
                    }
                }
            }

            return unilevelDownline;
        }

        public int GetMaxLevelInUnilevelTree(int topUnilevelID)
        {
            int level = 1;

            level = RunQuery(topUnilevelID, level);

            return level;
        }

        public List<Node> GetUnilevelDownlineTreeAndIfPaid(string topUnilevelID, int level, int period, int periodTypeID)
        {
            List<Node> unilevelDownline = new List<Node>();

            int skipCounter = 0;
            int takeCounter = 50;

            while (takeCounter == 50)
            {
                var context = ExigoApiContext.CreateODataContext();
                var query = (from c in context.UniLevelTreePeriodVolumes
                             where c.TopCustomerID == Convert.ToInt32(topUnilevelID)
                             where c.Level == level
                             where c.PeriodID == period
                             where c.PeriodTypeID == periodTypeID
                             select new
                             {
                                 c.CustomerID,
                                 c.SponsorID,
                                 c.Customer.EnrollerID,
                                 c.Customer.CustomerTypeID,
                                 c.Customer.CustomerStatusID,
                                 c.PeriodVolume.PaidRankID,
                                 c.Customer.FirstName,
                                 c.Customer.LastName,
                                 c.Customer.CreatedDate
                             }).Skip(skipCounter).Take(50);

                skipCounter += takeCounter;
                takeCounter = query.Count();

                if (query.Count() > 0)
                {
                    int nodeID = 0;
                    foreach (var c in query)
                    {
                        Node node = new Node()
                        {
                            CustomerID = c.CustomerID,
                            NodeID = nodeID,
                            ParentID = c.SponsorID,
                            CustomerType = c.CustomerTypeID,
                            CustomerStatus = c.CustomerStatusID,
                            PayRankID = c.PaidRankID,
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            CreatedDate = c.CreatedDate
                        };

                        node.IsPayingOutThisPeriod = IsIDPayingInPeriod(c.CustomerID, Convert.ToInt32(topUnilevelID), period);

                        unilevelDownline.Add(node);
                        nodeID++;
                    }
                }
            }

            return unilevelDownline;
        }

        public List<Order> TryOrdersQualifiedForBonus(string bonusCode, List<Order> orders, List<Customer> customers)
        {
            List<Order> qualifiedOrders = new List<Order>();

            switch (bonusCode)
            {
                case "htc": foreach (Order o in orders)
                            {
                                if (IsOrderQualifiedHTC(o, customers))
                                {
                                    qualifiedOrders.Add(o);
                                }
                            }
                            break;
                case "rc": break;
                case "wtb": break;
                case "wec": break;
                case "mec": break;
                case "mtb": break;
                case "smb": break;
                case "reb": break;
                case "bbp": break;
            }
         
            return qualifiedOrders;
        }

        public List<string[]> TryOrdersQualifiedForBonus(string bonusCode, List<Order> orders, int toCustomerID, Node fromCustomer, List<string[]> qualificationStatus)
        {
            switch (bonusCode)
            {
                case "active": foreach (Order o in orders)
                    {
                        fromCustomer.PCV += o.BusinessVolumeTotal;

                        if (fromCustomer.PCV >= 60)
                        {
                            fromCustomer.Active = true;
                        }
                    }
                    break;
                case "htc": foreach (Order o in orders)
                    {
                        string[] qualified = IsOrderQualifiedHTC(o);
                        if (qualified.Length > 0)
                        {
                            for (int i = 0; i < qualified.Length; i++)
                            {
                                qualificationStatus.Add(qualified);
                            }
                        }
                        else
                        {
                            qualified = new string[] { "Home Transaction Credit", o.OrderID.ToString(), "Not qualified" };
                            qualificationStatus.Add(qualified);
                        }
                    }
                    break;
                case "rc": foreach (Order o in orders)
                    {
                        string[] qualified = IsOrderQualifiedRC(o);
                        if (qualified.Length > 0)
                        {
                            for (int i = 0; i < qualified.Length; i++)
                            {
                                qualificationStatus.Add(qualified);
                            }
                        }
                        else
                        {
                            qualified = new string[] { "Retail Commission", o.OrderID.ToString(), string.Format("{0:N0}", o.Other1Total), "No Retail Value" };
                            qualificationStatus.Add(qualified);
                        }
                    }
                    break;
                case "wtb": 
                    List<string[]> tierQualified = IsOrderQualifiedWTB(orders, toCustomerID, fromCustomer);
                    if (tierQualified.Count() > 0)
                    {
                        foreach (var o in tierQualified)
                        {
                            qualificationStatus.Add(o);
                        }
                    }
                    else
                    {
                        string[] noOrder = new string[] { "Weekly Tier Bonus", "No Orders", "Not Qualified" };
                        qualificationStatus.Add(noOrder);
                    }
                    break;
                case "wec":
                    List<string[]> wecQualified = IsOrderQualifiedWEC(orders, toCustomerID, fromCustomer);
                    if (wecQualified.Count() > 0)
                    {
                        foreach (var o in wecQualified)
                        {
                            qualificationStatus.Add(o);
                        }
                    }
                    else
                    {
                        string[] noOrder = new string[] { "Weekly Enroller Bonus", "No Orders", "Not Qualified" };
                        qualificationStatus.Add(noOrder);
                    }
                    break;
                case "mec": 
                    List<string[]> mecQualified = IsOrderQualifiedMEC(orders, toCustomerID, fromCustomer);
                    if (mecQualified.Count() > 0)
                    {
                        foreach (var o in mecQualified)
                        {
                            qualificationStatus.Add(o);
                        }
                    }
                    else
                    {
                        string[] noOrder = new string[] { "Weekly Enroller Bonus", "No Orders", "Not Qualified" };
                        qualificationStatus.Add(noOrder);
                    }
                    break;
                case "mtb": break;
                case "smb": break;
                case "reb": break;
                case "bbp": break;
            }

            return qualificationStatus;
        }

        public Node GetReasonsNotPaying(Node customerNode, string toID, int periodID, int periodType)
        {
            //Grab the customer data for this node
            customerNode = GetCustomerData(customerNode);

            List<string[]> reasonsNotQualified = new List<string[]>();
            string[] reason;
            //Get the orders of the person 
            List<Order> periodOrders = GetListOfQualifiedOrders(periodID, periodType, Convert.ToInt32(customerNode.CustomerID));


            //Check if active customer status
            if (customerNode.CustomerStatus != 1)
            {
                reason = new string[] { "Customer Status", "Not Active" };
                reasonsNotQualified.Add(reason);
            }

            //check if meeting 60PCV qualification
            if (IsNode60PCV(periodOrders, customerNode))
            {
                customerNode.Active = true;
            }
            else
            {
                customerNode.Active = false;
                reason = new string[] { "PCV", string.Format("{0:N}", customerNode.PCV) };
                reasonsNotQualified.Add(reason);
            }

            //check if any orders exist
            if (periodOrders.Count() > 0)
            {
                //check each bonus against the list of orders to see why they didn't pay out
                switch (periodType)
                {
                    case 1: foreach (var i in weeklyBonuses)
                        {
                            reasonsNotQualified = TryOrdersQualifiedForBonus(i, periodOrders, Convert.ToInt32(toID), customerNode, reasonsNotQualified);
                        }
                        break;
                    case 2: foreach (var i in monthlyBonuses)
                        {
                            reasonsNotQualified = TryOrdersQualifiedForBonus(i, periodOrders, Convert.ToInt32(toID), customerNode, reasonsNotQualified);
                        }
                        break;
                    case 3: foreach (var i in quarterlyBonuses)
                        {
                            reasonsNotQualified = TryOrdersQualifiedForBonus(i, periodOrders, Convert.ToInt32(toID), customerNode, reasonsNotQualified);
                        }
                        break;
                    case 4: break;
                }
            }
            else
            {
                reason = new string[] { "Inactive", "No Qualifying Orders" };
                reasonsNotQualified.Add(reason);
            }

            customerNode.reasonsNotQualified = reasonsNotQualified;

            return customerNode;
        }


        /// <summary>
        /// Method to get Real Time commissions for a specified period type
        /// </summary>
        /// <param name="periodType">int PeriodType to get</param>
        /// <param name="toCustomer">int Customer to Lookup</param>
        /// <returns>Commission Response</returns>
        public CommissionResponse GetRealTimeCommissionsForSpecificPeriodType(int periodType, int toCustomer)
        {
            GetRealTimeCommissionsRequest req = new GetRealTimeCommissionsRequest();
            req.CustomerID = toCustomer;

            var context = ExigoApiContext.CreateWebServiceContext();
            GetRealTimeCommissionsResponse commResponse = context.GetRealTimeCommissions(req);


            CommissionResponse res = new CommissionResponse();

            foreach (var r in commResponse.Commissions.Where(i => i.PeriodType == periodType))
            {
                res = r;
            }

            return res;
        }

        #region Individual Order Qualifications for Bonus
        public bool IsNode60PCV(List<Order> orders, Node customerNode)
        {
            bool Active = false;
            customerNode.PCV = 0;

            foreach (Order o in orders)
            {
                customerNode.PCV += o.BusinessVolumeTotal;
            }

            if (customerNode.PCV >= 60) Active = true;

            return Active;
        }
        public bool IsOrderQualifiedHTC(Order order, List<Customer> customers)
        {
            bool qualified = false;

            foreach (Customer c in customers)
            {
                if (c.CustomerID == order.CustomerID) qualified = true;
            }

            return qualified;
        }
        public string[] IsOrderQualifiedHTC(Order order)
        {
            List<string> qualified = new List<string>();

            foreach (var i in order.Details)
            {
                if (i.ItemCode == "1190")
                {
                    qualified.Add("Home Transaction Credit");
                    qualified.Add(order.OrderID.ToString());
                    qualified.Add(i.Quantity.ToString());
                }
            }

            return qualified.ToArray();
        }
        public string[] IsOrderQualifiedRC(Order order)
        {
            List<string> qualified = new List<string>();

            if (order.Other1Total > 0)
            {
                qualified.Add("Retail Commission");
                qualified.Add(order.OrderID.ToString());
                qualified.Add(order.Other1Total.ToString());
                qualified.Add("Well that was odd");
            }

            return qualified.ToArray();
        }
        public List<string[]> IsOrderQualifiedWTB(List<Order> orders, int toCustomer, Node fromCustomer)
        {
            List<string[]> qualified = new List<string[]>();
            string[] order;

            if (orders.Count() > 0)
            {
                if (toCustomer != fromCustomer.SponsorID)
                {
                    foreach (Order o in orders)
                    {
                        order = new string[] { "Weekly Tier Bonus", o.OrderID.ToString(), "", "Not First Level" };
                        qualified.Add(order);
                    }
                }
                else
                {
                    foreach (Order o in orders)
                    {
                        order = new string[] { "Weekly Tier Bonus", o.OrderID.ToString(), string.Format("{0:N}", o.BusinessVolumeTotal), "No Volume" };
                        qualified.Add(order);
                    }
                }
            }
            else
            {
                order = new string[] { "Weekly Tier Bonus", "No Order", "", "No Volume" };
                qualified.Add(order);
            }

            return qualified;
        }

        public List<string[]> IsOrderQualifiedWEC(List<Order> orders, int toCustomer, Node fromCustomer)
        {
            List<string[]> qualified = new List<string[]>();
            string[] order;

            if (orders.Count() > 0)
            {
                if (toCustomer != fromCustomer.EnrollerID)
                {
                    foreach (Order o in orders)
                    {
                        order = new string[] { "Weekly Enroller Bonus", o.OrderID.ToString(), "", "Not First Level" };
                        qualified.Add(order);
                    }
                }
                else
                {
                    foreach (Order o in orders)
                    {
                        order = new string[] { "Weekly Enroller Bonus", o.OrderID.ToString(), string.Format("{0:N}", o.BusinessVolumeTotal), "No Volume" };
                        qualified.Add(order);
                    }
                }
            }
            else
            {
                order = new string[] { "Weekly Enroller Bonus", "No Order", "", "No Volume" };
                qualified.Add(order);
            }

            return qualified;
        }

        public List<string[]> IsOrderQualifiedMEC(List<Order> orders, int toCustomer, Node fromCustomer)
        {
            List<string[]> qualified = new List<string[]>();
            string[] order;

            if (orders.Count() > 0)
            {
                if (toCustomer == fromCustomer.EnrollerID)
                {
                    foreach (Order o in orders)
                    {
                        order = new string[] { "Monthly Enroller Bonus", o.OrderID.ToString(), "", "First Level Order" };
                        qualified.Add(order);
                    }
                }
                else
                {
                    foreach (Order o in orders)
                    {
                        order = new string[] { "Monthly Enroller Bonus", o.OrderID.ToString(), string.Format("{0:N}", o.BusinessVolumeTotal), "No Volume" };
                        qualified.Add(order);
                    }
                }
            }
            else
            {
                order = new string[] { "Monthly Enroller Bonus", "No Order", "", "No Volume" };
                qualified.Add(order);
            }

            return qualified;
        }
        #endregion

        #region Ranks
        public GetRankQualificationsResponse GetRankQualifications(int toCustomerID, int rankID, int periodType, int periodID)
        {
            GetRankQualificationsRequest req = new GetRankQualificationsRequest()
            {
                CustomerID = toCustomerID,
                RankID = rankID,
                PeriodType = periodType,
                PeriodID = periodID
            };

            var context = ExigoApiContext.CreateWebServiceContext();
            GetRankQualificationsResponse res = context.GetRankQualifications(req);

            return res;
        }

        public List<GetRankQualificationsResponse> GetAllRankQualifications(int toCustomerID, int periodType, int periodID)
        {
            List<GetRankQualificationsResponse> responses = new List<GetRankQualificationsResponse>();
            List<int> ranks = new List<int>();
            var context = ExigoApiContext.CreateODataContext();
            var rankQuery = from r in context.Ranks
                             select r;

            if (rankQuery.Count() > 0)
            {
                foreach (var r in rankQuery)
                {
                    ranks.Add(r.RankID);
                }
            }

            foreach (int r in ranks)
            {
                GetRankQualificationsRequest req = new GetRankQualificationsRequest()
                {
                    CustomerID = toCustomerID,
                    PeriodType = periodType,
                    PeriodID = periodID,
                    RankID = r
                };
                var context2 = ExigoApiContext.CreateWebServiceContext();
                GetRankQualificationsResponse res = context2.GetRankQualifications(req);

                responses.Add(res);
            }

            return responses;
        }

        public KeyValuePair<int, string> GetHighestRank(int customerID)
        {
            KeyValuePair<int, string> rank;
            var context = ExigoApiContext.CreateODataContext();
            var customerQuery = (from c in context.Customers
            //var customerQuery = (from c in api.OData.Customers
                                 where c.CustomerID == customerID
                                 select new
                                 {
                                     c.RankID,
                                     c.Rank.RankDescription
                                 }).FirstOrDefault();

            if (customerQuery != null)
            {
                rank = new KeyValuePair<int, string>(customerQuery.RankID, customerQuery.RankDescription);
            }
            else
            {
                rank = new KeyValuePair<int, string>(0, "Not Ranked");
            }

            return rank;

        }
        #endregion

        #region Helper Methods
        public int RunQuery(int topCustomerID, int level)
        {
            int Level = level;

            var context = ExigoApiContext.CreateODataContext();
            var query = (from c in context.UniLevelTree
                         where c.TopCustomerID == topCustomerID
                         where c.Level > level
                         select new
                         {
                             c.Level
                         });

            if (query.Count() > 0)
            {
                List<int> l = new List<int>();
                foreach (var i in query)
                {
                    l.Add(i.Level);
                }
                Level = l.Max();
                return RunQuery(topCustomerID, Level);
            }
            else
            {
                return Level;
            }
            
        }
        #endregion
    }

    public enum CustomerTypeID
    {
        RetailCustomer = 1,
        PreferredCustomer,
        WholesaleCustomer,
        IBD
    }

    public enum CustomerStatusID
    {
        Active = 1,
        Terminated,
        Inactive,
        Cancelled
    }
}