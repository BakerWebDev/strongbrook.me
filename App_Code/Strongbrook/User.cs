using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Exigo.OData;
using Exigo.WebService;

/// <summary>
/// User Class to determine navigation fields for back office dashboard
/// </summary>

public class User : System.Web.UI.Page
{
    public User()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    #region Private Properties
    //private permission variables, set through the permission methods
    private int _customerID { get; set; }
    private bool _allowDashboard { get; set; }
    private bool _allowBusiness { get; set; }
    private bool _allowShop { get; set; }
    private bool _allowAutoship { get; set; }
    private bool _allowAccounts { get; set; }
    private bool _allowTraining { get; set; }
    private bool _allowMaster { get; set; }
    private string _permissionLevel1 { get; set; }
    private string _permissionLevel2 { get; set; }
    private bool _allowWealthClub { get; set; }
    private bool _allowAcademy { get; set; }
    private bool _allowChat { get; set; }
    private bool _allowCreditRepair { get; set; }
    private bool _allowLiveEvents { get; set; }
    private bool _allowOnlineWebinars { get; set; }
    private bool _allowWebinarArchive { get; set; }
    private bool _allowPreferredSeating { get; set; }
    private bool _allowWealthProtection { get; set; }
    private bool _allowHealthProtection { get; set; }
    private Exigo.OData.CustomerType _customerType { get; set; }
    private string _litmosAPI { get; set; }
    private bool _allowBackOffice { get; set; }
    private bool _allowWebsites { get; set; }
    private bool _allowEnrollIBD { get; set; }
    private bool _allowEnrollClient { get; set; }
    private bool _allowWealthSummit { get; set; }
    private bool _allowMemberConnect { get; set; }
    private bool _allowLiveCoaching { get; set; }
    private string _accessLevel_1 { get; set; }
    private string _superUserAccessLevel { get; set; }
    #endregion

    #region Public Properties
    public int CustomerID
    {
        get { return _customerID; }
        set { _customerID = value; }
    }
    public bool AllowDashboard
    {
        get { return _allowDashboard; }
        set { _allowDashboard = value; }
    }
    public bool AllowBusiness
    {
        get { return _allowBusiness; }
        set { _allowBusiness = value; }
    }
    public bool AllowShop
    {
        get { return _allowShop; }
        set { _allowShop = value; }
    }
    public bool AllowAutoship
    {
        get { return _allowAutoship; }
        set { _allowAutoship = value; }
    }
    public bool AllowAccounts
    {
        get { return _allowAccounts; }
        set { _allowAccounts = value; }
    }
    public bool AllowTraining
    {
        get { return _allowTraining; }
        set { _allowTraining = value; }
    }
    public bool AllowMaster
    {
        get { return _allowMaster; }
        set { _allowMaster = value; }
    }
    public string SuperUserPermissionLevel_1 // Property ID5
    {
        get { return _permissionLevel1; }
        set { _permissionLevel1 = value; }
    }
    public string SuperUserPermissionLevel_2 // Property ID6
    {
        get { return _permissionLevel2; }
        set { _permissionLevel2 = value; }
    }
    public bool AllowAcademy
    {
        get { return _allowAcademy; }
        set { _allowAcademy = value; }
    }
    public bool AllowChat
    {
        get { return _allowChat; }
        set { _allowChat = value; }
    }
    public bool AllowCreditRepair
    {
        get { return _allowCreditRepair; }
        set { _allowCreditRepair = value; }
    }
    public bool AllowLiveEvents
    {
        get { return _allowLiveEvents; }
        set { _allowLiveEvents = value; }
    }
    public bool AllowOnlineWebinars
    {
        get { return _allowOnlineWebinars; }
        set { _allowOnlineWebinars = value; }
    }
    public bool AllowWebinarArchive
    {
        get { return _allowWebinarArchive; }
        set { _allowWebinarArchive = value; }
    }
    public bool AllowPreferredSeating
    {
        get { return _allowPreferredSeating; }
        set { _allowPreferredSeating = value; }
    }
    public bool AllowWealthProtection
    {
        get { return _allowWealthProtection; }
        set { _allowWealthProtection = value; }
    }
    public bool AllowHealthProtection
    {
        get { return _allowHealthProtection; }
        set { _allowHealthProtection = value; }
    }
    public Exigo.OData.CustomerType CustomerType
    {
        get { return _customerType; }
        set { _customerType = value; }
    }
    public string LitmosAPI
    {
        get { return _litmosAPI; }
        set { _litmosAPI = value; }
    }
    public bool AllowWealthClub
    {
        get { return _allowWealthClub; }
        set { _allowWealthClub = value; }
    }
    public bool AllowBackOffice
    {
        get { return _allowBackOffice; }
        set { _allowBackOffice = value; }
    }
    public bool AllowWebsites
    {
        get { return _allowWebsites; }
        set { _allowWebsites = value; }
    }
    public bool AllowEnrollIBD
    {
        get { return _allowEnrollIBD; }
        set { _allowEnrollIBD = value; }
    }
    public bool AllowEnrollClient
    {
        get { return _allowEnrollClient; }
        set { _allowEnrollClient = value; }
    }
    public bool AllowWealthSummit
    {
        get { return _allowWealthSummit; }
        set { _allowWealthSummit = value; }
    }
    public bool AllowMemberConnect
    {
        get { return _allowMemberConnect; }
        set { _allowMemberConnect = value; }
    }
    public bool AllowLiveCoaching
    {
        get { return _allowLiveCoaching; }
        set { _allowLiveCoaching = value; }
    }




    public string AccessLevel_1
    {
        get { return _accessLevel_1; }
        set { _accessLevel_1 = value; }
    }



    public bool IsAllowedUserLevel_1_Access { get; set; } // Property ID1
    public bool IsAllowedUserLevel_2_Access { get; set; } // Property ID2

    public bool IsAllowedSuperUserLevel_1_Access { get; set; } // Property ID3
    public bool IsAllowedSuperUserLevel_2_Access { get; set; } // Property ID4




    public Dictionary<string, string> UserPermissions;
    #endregion


    /// <summary>
    /// Checks customer's orders in last 5 weeks for one of the 
    /// wealth club itemcodes and if the order was fully processed
    /// </summary>
    /// <param name="CustomerID">The CustomerID to check for Orders</param>
    /// <returns>True if order exists, false if not</returns>
    public bool DoesTheAdminUserHaveSuperUserRights(int CustomerID)
    {
        return true;
    }

    #region API Methods
    /// <summary>
    /// Checks customer's orders in last 5 weeks for one of the 
    /// wealth club itemcodes and if the order was fully processed
    /// </summary>
    /// <param name="CustomerID">The CustomerID to check for Orders</param>
    /// <returns>True if order exists, false if not</returns>
    public bool CheckIfOrderInLastFiveWeeks(int CustomerID)
    {
        bool autoship = false;
        bool WealthClubOrder = false;
        string level = "";
        Dictionary<int, string> ItemList = new Dictionary<int, string>();

        var context = ExigoApiContext.CreateODataContext();

        //First thing check to see if an autoship for the wealth club exists and if it has run
        //if true, check for 5 week order, if false allow login (if no autoship exists, return true
        var autoshipQuery = (from o in context.AutoOrderDetails
                             where o.ItemCode.CompareTo("3109") > 0
                             where o.ItemCode.CompareTo("3184") < 0
                             where o.AutoOrder.CustomerID == CustomerID
                             where o.AutoOrder.AutoOrderStatusID == 0
                             select new
                             {
                                 o.AutoOrder.StartDate
                             }).FirstOrDefault();

        if (autoshipQuery == null)
        {
            autoship = true;
        }
        else
        {
            if (autoshipQuery.StartDate > DateTime.Now)
            {
                autoship = false;
                WealthClubOrder = true;

                var levelQuery = (from c in context.Customers
                                  where c.CustomerID == CustomerID
                                  select new
                                  {
                                      c.Field13
                                  });

                foreach (var c in levelQuery)
                {
                    SuperUserPermissionLevel_1 = c.Field13; // Property ID5
                }
            }
            else
                autoship = true;
        }


        if (autoship)
        {
            //Query returns the current Wealth Club level of the Customer (used to see if order change was made)
            var levelQuery = (from c in context.Customers
                              where c.CustomerID == CustomerID
                              select new
                              {
                                  c.Field13
                              });

            foreach (var c in levelQuery)
            {
                level = c.Field13;
            }

            SuperUserPermissionLevel_1 = level; // Property ID5

            //Query to get the item codes for orders in last 5 weeks
            var query = (from o in context.OrderDetails
                         where o.Order.CustomerID == CustomerID
                         where o.Order.OrderDate >= DateTime.Today.AddDays(-35)
                         where o.Order.OrderStatusID >= 7
                         where o.Order.OrderStatusID <= 9
                         where o.ItemCode.CompareTo("3109") > 0
                         where o.ItemCode.CompareTo("3184") < 0
                         select new
                         {
                             o.OrderID,
                             o.ItemCode
                         });

            if (query.Count() > 1)
            {
                var array = query.ToArray();
                var q = array.Last();
                ItemList.Add(q.OrderID, q.ItemCode);
            }
            else if (query.Count() > 0)
            {
                foreach (var q in query)
                {
                    ItemList.Add(q.OrderID, q.ItemCode);
                }
            }
            foreach (var i in ItemList.Where(item => Convert.ToInt32(item.Value) >= 3100 && Convert.ToInt32(item.Value) < 4000))
            {
                if (i.Value == "3110" || i.Value == "3181")
                {
                    if (level != "Wealth")
                    {
                        level = "Wealth";
                        ChangeUserWealthClubLevel(level, CustomerID);
                    }
                }
                if (i.Value == "3120" || i.Value == "3182")
                {
                    if (level != "Wealth")
                    {
                        level = "Wealth";
                        ChangeUserWealthClubLevel(level, CustomerID);
                    }
                }
                if (i.Value == "3130" || i.Value == "3183")
                {
                    if (level != "Wealth")
                    {
                        level = "Wealth";
                        ChangeUserWealthClubLevel(level, CustomerID);
                    }
                }
                WealthClubOrder = true;
            }

            //if they have a pack in Field 13 and have not made a purchase in last 15 days, change the field to blank
            if (level != "" && WealthClubOrder != true)
            {
                level = "";
                ChangeUserWealthClubLevel(level, CustomerID);
            }
        }
        return WealthClubOrder;
    }

    /// <summary>
    /// Will update the customer record for Field13 (Wealth Club Level) to reflect upgrade/downgrade of Wealth Club Pack
    /// </summary>
    /// <param name="level"></param>
    public void ChangeUserWealthClubLevel(string level, int CustomerID)
    {
        var context = ExigoApiContext.CreateWebServiceContext();

        UpdateCustomerRequest req = new UpdateCustomerRequest();
        req.CustomerID = CustomerID;
        req.Field13 = level;

        SuperUserPermissionLevel_1 = level; // Property ID5

        UpdateCustomerResponse res = context.UpdateCustomer(req);
    }
    #endregion


    #region Set Permission Methods

    #region Permission Level Constructor
    public User(Exigo.OData.CustomerType custType, int CustomerID)
    {
        if (DoesTheAdminUserHaveSuperUserRights(CustomerID))
        {
            //Switch case to determine what wealth club benefits (if any) should appear)
            //Switch case to determine what sub level (if any) should appear)
            switch (SuperUserPermissionLevel_1) // Property ID5
            {
                case "Wealth":
                case "Gold": SetGoldLevelPermissions(); break;
                case "Silver": SetSilverLevelPermissions(); break;
                case "Bronze": SetBronzeLevelPermissions(); break; // Reference ID-4 // Override ID-3
                case "SuperUserPermissionLevel_1": SetSuperUserLevel_1_Permissions(); break; // Case ID2 // Reference ID-5
                case "SuperUserPermissionLevel_2": SetSuperUserLevel_2_Permissions(); break;
                default: SetWealthClubAllFalse(); break;
            }
        }
        else
            SetWealthClubAllFalse();

        //Switch case to determine if IBD or Customer or Administrator links should appear
        switch (custType.CustomerTypeDescription)
        {
            case "IBD": SetIBDPermissions(); break; // Case ID-1 // Reference ID-1
            case "Retail Customer": SetCustomerPermissions(); break; // Case ID-2 // Reference ID2.1
            case "Wholesale Customer": SetCustomerPermissions(); break; // Reference ID2.1
            case "Preferred Customer": SetCustomerPermissions(); break; // Reference ID2.1
            case "Administrator": SetPermissionsForCustomerTypeAdministratorWithNoSuperUserAccess(); break; // Case ID-3 // Reference ID-3
            default: break;
        }
    }
    #endregion Permission Level Constructor

    #region Set Permission Levels
    /// <summary>
    /// Permissions for Gold Level Wealth Club Members
    /// </summary>
    public void SetGoldLevelPermissions()
    {
        //dictionary used in navigation screen to encrypt into user cookie
        UserPermissions = new Dictionary<string, string>();

        UserPermissions.Add("WealthClub", "true");   //Access to the Wealth Club link in the Portal
        UserPermissions.Add("CreditRepair", "true");   //Access to the Counseling Request Panel in Wealth Club
        UserPermissions.Add("LiveEvents", "true");    //Access to nothing so far - not in use
        UserPermissions.Add("OnlineWebinars", "false"); //Access to online webucations - not in use
        UserPermissions.Add("WebinarArchive", "false");  //Access to archive of webucations - not in use
        UserPermissions.Add("Academy", "true");  //Access to Litmos Academy
        UserPermissions.Add("WealthProtection", "true"); //Access to Counseling Request Panel
        UserPermissions.Add("HealthProtection", "true");  //Access to Counseling Request Panel
        UserPermissions.Add("WealthSummit", "true");  //Access to info about the Wealth Summit
        UserPermissions.Add("MemberConnect", "false");  //Access to the Member Connect Panel - not in use
        UserPermissions.Add("LiveCoaching", "false");    //Access to the LivePerson chat system - not in use
        UserPermissions.Add("WealthClubLevel", "Gold");  //Set the current Wealth Club Level. // Override ID-1
    }

    ///<summary>
    /// Permissions for Silver Level Wealth Club Members
    ///</summary>
    public void SetSilverLevelPermissions()
    {
        //dictionary used in navigation screen to encrypt into user cookie
        UserPermissions = new Dictionary<string, string>();

        UserPermissions.Add("WealthClub", "true");  //Access to the Wealth Club link in the Portal
        UserPermissions.Add("CreditRepair", "true");  //Access to the Counseling Request Panel in Wealth Club
        UserPermissions.Add("LiveEvents", "true");   //Access to nothing so far - not in use
        UserPermissions.Add("OnlineWebinars", "false");  //Access to online webucations - not in use
        UserPermissions.Add("WebinarArchive", "false");   //Access to archive of webucations - not in use
        UserPermissions.Add("Academy", "true");  //Access to Litmos Academy
        UserPermissions.Add("WealthProtection", "true"); //Access to Counseling Request Panel
        UserPermissions.Add("HealthProtection", "true");  //Access to Counseling Request Panel
        UserPermissions.Add("WealthSummit", "false"); //Access to info about the Wealth Summit
        UserPermissions.Add("MemberConnect", "false");  //Access to the Member Connect Panel - not in use
        UserPermissions.Add("LiveCoaching", "false");  //Access to the LivePerson chat system - not in use
        UserPermissions.Add("WealthClubLevel", "Silver");  //Set the current Wealth Club Level. // Override ID-1
    }

    ///<summary>
    /// Permissions for Bronze Level Wealth Club Members
    ///</summary>
    public void SetBronzeLevelPermissions() // Reference ID-4
    {
        //dictionary used in navigation screen to encrypt into user cookie
        UserPermissions = new Dictionary<string, string>();

        UserPermissions.Add("WealthClub", "true");  //Access to the Wealth Club link in the Portal
        UserPermissions.Add("CreditRepair", "true");  //Access to the Counseling Request Panel in Wealth Club
        UserPermissions.Add("LiveEvents", "false");  //Access to nothing so far - not in use
        UserPermissions.Add("OnlineWebinars", "false");  //Access to online webucations - not in use
        UserPermissions.Add("WebinarArchive", "false");  //Access to archive of webucations - not in use
        UserPermissions.Add("Academy", "false");  //Access to Litmos Academy
        UserPermissions.Add("WealthProtection", "true"); //Access to Counseling Request Panel
        UserPermissions.Add("HealthProtection", "true");  //Access to Counseling Request Panel
        UserPermissions.Add("WealthSummit", "false");  //Access to info about the Wealth Summit
        UserPermissions.Add("MemberConnect", "false");  //Access to the Member Connect Panel - not in use
        UserPermissions.Add("LiveCoaching", "false");  //Access to the LivePerson chat system - not in use
        UserPermissions.Add("WealthClubLevel", "Bronze");  //Set the current Wealth Club Level. // Override ID-1 // Override ID-3
    }


    /// <summary>
    /// Permissions for a Person with no Wealth Club Access - Used to avoid Null Exception
    /// </summary>
    public void SetWealthClubAllFalse()
    {
        UserPermissions = new Dictionary<string, string>();

        UserPermissions.Add("WealthClub", "false"); //Access to the Wealth Club link in the Portal
        UserPermissions.Add("CreditRepair", "false");  //Access to the Counseling Request Panel in Wealth Club;
        UserPermissions.Add("LiveEvents", "false");  //Access to nothing so far - not in use
        UserPermissions.Add("OnlineWebinars", "false");  //Access to online webucations - not in use
        UserPermissions.Add("WebinarArchive", "false");   //Access to archive of webucations - not in use
        UserPermissions.Add("Academy", "false");  //Access to Litmos Academy
        UserPermissions.Add("WealthProtection", "false"); //Access to Counseling Request Panel
        UserPermissions.Add("HealthProtection", "false");  //Access to Counseling Request Panel
        UserPermissions.Add("WealthSummit", "false");  //Access to info about the Wealth Summit
        UserPermissions.Add("MemberConnect", "false");  //Access to the Member Connect Panel - not in use;
        UserPermissions.Add("LiveCoaching", "false");  //Access to the LivePerson chat system - not in use
        UserPermissions.Add("WealthClubLevel", ""); //Set the current Wealth Club Level. // Override ID-1
        UserPermissions.Add("AdminPermissionsLevel_1_And_SuperUserLevel", ""); //Set the current Wealth Club Level. // Override ID-1
    }



    ///<summary>
    /// Permissions for Sub-Level 1 Users
    ///</summary>
    public void SetSuperUserLevel_1_Permissions() // Reference ID-5 // This is like WealthClub Level: Gold
    {
        //dictionary used in navigation screen to encrypt into user cookie
        UserPermissions = new Dictionary<string, string>();

        UserPermissions.Add("WealthClub", "true");  //Access to the Wealth Club link in the Portal
        UserPermissions.Add("CreditRepair", "true");  //Access to the Counseling Request Panel in Wealth Club
        UserPermissions.Add("LiveEvents", "false");  //Access to nothing so far - not in use
        UserPermissions.Add("OnlineWebinars", "false");  //Access to online webucations - not in use
        UserPermissions.Add("WebinarArchive", "false");  //Access to archive of webucations - not in use
        UserPermissions.Add("Academy", "false");  //Access to Litmos Academy
        UserPermissions.Add("WealthProtection", "true"); //Access to Counseling Request Panel
        UserPermissions.Add("HealthProtection", "true");  //Access to Counseling Request Panel
        UserPermissions.Add("WealthSummit", "false");  //Access to info about the Wealth Summit
        UserPermissions.Add("MemberConnect", "false");  //Access to the Member Connect Panel - not in use
        UserPermissions.Add("LiveCoaching", "false");  //Access to the LivePerson chat system - not in use
        UserPermissions.Add("WealthClubLevel", "Gold");  //Set the current Wealth Club Level. // Override ID-1 // Override ID-3
        UserPermissions.Add("AdminPermissionsLevel_1_And_SuperUserLevel", "SuperUserPermissionLevel_1");  //Set the current User Level. // Override ID-5 // Override ID-3
    }
    ///<summary>
    /// Permissions for Sub-Level 2 Users
    ///</summary>
    public void SetSuperUserLevel_2_Permissions() // Reference ID-5 // This is like WealthClub Level: Silver
    {
        //dictionary used in navigation screen to encrypt into user cookie
        UserPermissions = new Dictionary<string, string>();

        UserPermissions.Add("WealthClub", "true");  //Access to the Wealth Club link in the Portal
        UserPermissions.Add("CreditRepair", "true");  //Access to the Counseling Request Panel in Wealth Club
        UserPermissions.Add("LiveEvents", "false");  //Access to nothing so far - not in use
        UserPermissions.Add("OnlineWebinars", "false");  //Access to online webucations - not in use
        UserPermissions.Add("WebinarArchive", "false");  //Access to archive of webucations - not in use
        UserPermissions.Add("Academy", "false");  //Access to Litmos Academy
        UserPermissions.Add("WealthProtection", "true"); //Access to Counseling Request Panel
        UserPermissions.Add("HealthProtection", "true");  //Access to Counseling Request Panel
        UserPermissions.Add("WealthSummit", "false");  //Access to info about the Wealth Summit
        UserPermissions.Add("MemberConnect", "false");  //Access to the Member Connect Panel - not in use
        UserPermissions.Add("LiveCoaching", "false");  //Access to the LivePerson chat system - not in use
        UserPermissions.Add("WealthClubLevel", "Silver");  //Set the current Wealth Club Level. // Override ID-1 // Override ID-3
        UserPermissions.Add("AdminPermissionsLevel_1_And_SuperUserLevel", "SuperUserPermissionLevel_2");  //Set the current User Level. // Override ID-5 // Override ID-3
    }
    #endregion Set Permission Levels

    #region SuperUser Permissions
    //Permissions for IBD
    public void SetIBDPermissions()
    {
        AllowBackOffice = true;
        AllowWebsites = true;
        AllowEnrollIBD = true;
        AllowEnrollClient = true;
    }

    public void SetIBDPermissions(string WealthClubLevel) // Reference ID-1 Override ID-1
    {
        AllowBackOffice = true;
        AllowWebsites = true;
        AllowEnrollIBD = true;
        AllowEnrollClient = true;
    }


    //Permissions for Customers
    public void SetCustomerPermissions()
    {
        AllowBackOffice = false;
        AllowWebsites = false;
        AllowEnrollClient = false;
        AllowEnrollIBD = false;
    }

    public void SetCustomerPermissions(string WealthClubLevel) // Reference ID-2 Override ID-1
    {
        AllowBackOffice = false;
        AllowWebsites = false;
        AllowEnrollClient = false;
        AllowEnrollIBD = false;
    }



    //Permissions for Administrators
    public void SetPermissionsForCustomerTypeAdministratorWithNoSuperUserAccessAndOnlyOneLink() // Reference ID-3
    {
        IsAllowedUserLevel_1_Access = false; // Property ID1
        IsAllowedUserLevel_2_Access = true; // Property ID2
        IsAllowedSuperUserLevel_1_Access = false; // Property ID3
        IsAllowedSuperUserLevel_2_Access = false; // Property ID4
    }

    public void SetPermissionsForCustomerTypeAdministratorWithNoSuperUserAccess() // Reference ID-3
    {
        IsAllowedUserLevel_1_Access = true; // Property ID1
        IsAllowedUserLevel_2_Access = true; // Property ID2
        IsAllowedSuperUserLevel_1_Access = false; // Property ID3
        IsAllowedSuperUserLevel_2_Access = false; // Property ID4
    }

    public void SetPermissionsForCustomerTypeAdministrator(string AdminPermissionsLevel_1_And_SuperUserLevel) // Reference ID-3 Override ID-5
    {
        AllowBackOffice = false;
        AllowWebsites = false;
        AllowEnrollClient = false;
        AllowEnrollIBD = false;
        IsAllowedUserLevel_1_Access = true; // Property ID1
        IsAllowedUserLevel_2_Access = true; // Property ID2
        IsAllowedSuperUserLevel_1_Access = true; // Property ID3
        IsAllowedSuperUserLevel_2_Access = true; // Property ID4
    }

    public void SetPermissionsForCustomerTypeAdministratorsWithLessAccess(string AdminPermissionsLevel_1_And_SuperUserLevel) // Reference ID-3 Override ID-5
    {
        AllowBackOffice = false;
        AllowWebsites = false;
        AllowEnrollClient = false;
        AllowEnrollIBD = false;
        IsAllowedUserLevel_1_Access = true; // Property ID1
        IsAllowedUserLevel_2_Access = true; // Property ID2
        IsAllowedSuperUserLevel_1_Access = false; // Property ID3
        IsAllowedSuperUserLevel_2_Access = true; // Property ID4
    }

    #endregion SuperUser Permissions
    #endregion

    //Methods to decrypt cookie values
    #region Decryption Methods

    /// <summary>
    /// Decrypts the user permissions from a cookie
    /// </summary>
    /// <param name="readCookie">Cookie containing permissions</param>
    /// <param name="user">User to store permissions in</param>
    /// <returns>a User with decrypted permissions</returns>
    public User DecryptUserInformation(HttpCookie readCookie, User user)
    {
        //Method to decrypt user permissions from user cookie
        user.AllowAcademy = true;
        user.AllowCreditRepair = true; // Convert.ToBoolean(Decrypt(readCookie.Values["CreditRepair"], "justdoit"));
        user.AllowLiveEvents = true; // Convert.ToBoolean(Decrypt(readCookie.Values["LiveEvents"], "justdoit"));
        user.AllowOnlineWebinars = true; // Convert.ToBoolean(Decrypt(readCookie.Values["OnlineWebinars"], "justdoit"));
        user.AllowWebinarArchive = true; // Convert.ToBoolean(Decrypt(readCookie.Values["WebinarArchive"], "justdoit"));
        user.AllowWealthClub = true; // Convert.ToBoolean(Decrypt(readCookie.Values["WealthClub"], "justdoit"));
        user.AllowWealthProtection = true; // Convert.ToBoolean(Decrypt(readCookie.Values["WealthProtection"], "justdoit"));
        user.AllowHealthProtection = true; // Convert.ToBoolean(Decrypt(readCookie.Values["HealthProtection"], "justdoit"));
        user.AllowMemberConnect = true; // Convert.ToBoolean(Decrypt(readCookie.Values["MemberConnect"], "justdoit"));
        user.AllowWealthSummit = true; // Convert.ToBoolean(Decrypt(readCookie.Values["WealthSummit"], "justdoit"));
        user.AllowLiveCoaching = true; // Convert.ToBoolean(Decrypt(readCookie.Values["LiveCoaching"], "justdoit"));
        user.SuperUserPermissionLevel_1 = "SuperUserPermissionLevel_1";
        user.SuperUserPermissionLevel_2 = "SuperUserPermissionLevel_2";
        //LitmosAPI = readCookie.Values["LitmosAPI"];

        return user;
    }

    public string DecryptSingleUserPermission(string cookieValue)
    {
        string decryptedValue = Decrypt(cookieValue, "justdoit");
        return decryptedValue;
    }

    string Decrypt(string coded, string key)
    {
        coded = coded.Replace(" ", "+");
        RijndaelManaged cryptProvider = new RijndaelManaged();
        cryptProvider.KeySize = 256;
        cryptProvider.BlockSize = 256;
        cryptProvider.Mode = CipherMode.CBC;
        SHA256Managed hashSHA256 = new SHA256Managed();
        cryptProvider.Key = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
        string iv = "user";
        cryptProvider.IV = hashSHA256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(iv));
        byte[] cipherTextByteArray = Convert.FromBase64String(coded);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, cryptProvider.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(cipherTextByteArray, 0, cipherTextByteArray.Length);
        cs.FlushFinalBlock();
        cs.Close();
        byte[] byt = ms.ToArray();
        return Encoding.ASCII.GetString(byt);
    }
    #endregion

}
