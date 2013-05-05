using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Strongbrook.Bonus
{
    public class BonusSettings
    {
        public static class HTC
        {
            #region private properties
            private static string _itemCode = "1190";
            private static string _bonusCode = "htc";
            private static int _periodType = 2;
            private static int _startPeriod = 1;
            #endregion

            #region public properties
            public static string BonusCode
            {
                get { return _bonusCode; }
            }
            public static string ItemCode
            {
                get { return _itemCode; }
            }
            public static int PeriodType
            {
                get { return _periodType; }
            }
            public static int StartPeriod
            {
                get { return _startPeriod; }
            }
            #endregion
        }
    }
}