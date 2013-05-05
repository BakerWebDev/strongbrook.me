using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Strongbrook.Bonus
{
    /// <summary>
    /// Summary description for Node
    /// </summary>
    public class Node
    {
        public Node()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region private properties
        private int _customerID;
        private int _sponsorID;
        private int _enrollerID;
        private int _nodeID;
        private int _parentID;
        private int _level;
        private int _customerType;
        private int _customerStatus;
        private int _rankID;
        private int _payRankID;
        private string _firstName;
        private string _lastName;
        private string _fullName;
        private DateTime _createdDate;
        private bool _payingOutThisPeriod;
        private bool _active;
        private decimal _PCV;
        #endregion

        #region public properties
        public int CustomerID
        {
            get { return _customerID; }
            set { _customerID = value; }
        }
        public int NodeID
        {
            get { return _nodeID; }
            set { _nodeID = value; }
        }
        public int ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        public int CustomerType
        {
            get { return _customerType; }
            set { _customerType = value; }
        }
        public int CustomerStatus
        {
            get { return _customerStatus; }
            set { _customerStatus = value; }
        }
        public int RankID
        {
            get { return _rankID; }
            set { _rankID = value; }
        }
        public int PayRankID
        {
            get { return _payRankID; }
            set { _payRankID = value; }
        }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }
        public bool IsPayingOutThisPeriod
        {
            get { return _payingOutThisPeriod; }
            set { _payingOutThisPeriod = value; }
        }
        public NodeCommissions[] BonusDetails;
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        public decimal PCV
        {
            get { return _PCV; }
            set { _PCV = value; }
        }
        public int SponsorID
        {
            get { return _sponsorID; }
            set { _sponsorID = value; }
        }
        public int EnrollerID
        {
            get { return _enrollerID; }
            set { _enrollerID = value; }
        }
        public List<string[]> reasonsNotQualified;
        #endregion
    }

    public class NodeCommissions
    {
        public NodeCommissions()
        {
        }

        private decimal _bonusAmount;
        private string _bonusType;
        private decimal _percentagePaid;

        public decimal BonusAmount
        {
            get { return _bonusAmount; }
            set { _bonusAmount = value; }
        }
        public string BonusType
        {
            get { return _bonusType; }
            set { _bonusType = value; }
        }
        public decimal PercentagePaid
        {
            get { return _percentagePaid; }
            set { _percentagePaid = value; }
        }
    }
}