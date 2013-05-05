<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="GamePlanReport_Weekly.aspx.cs" Inherits="GamePlanReport_Weekly" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/commissions.min.css" rel="stylesheet" />
    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/exigo.util.js"></script>
    <script src="Assets/Scripts/exigo.report.js"></script>
    <script src="Assets/Scripts/exigo.report.searching.js"></script>
    <script>
        // Set page variables
        var page = {
            activenavigation: 'organization'
        };


        $(function () {
            // Report
            setInitialSort('PeriodID', report.sortOrderTypes.DESCENDING);
            initializeReport();

            // Searching in report
            searching.listdatasources.countries = <%=SqlReportSearchListJsonDataSource.Countries() %>;
            searching.listdatasources.states = <%=SqlReportSearchListJsonDataSource.States() %>;
            searching.listdatasources.ranks = <%=SqlReportSearchListJsonDataSource.Ranks() %>;
            searching.listdatasources.types = <%=SqlReportSearchListJsonDataSource.CustomerTypes() %>;
            searching.listdatasources.statuses = <%=SqlReportSearchListJsonDataSource.CustomerStatuses() %>;
            searching.listdatasources.periods = <%=SqlReportSearchListJsonDataSource.Periods() %>;
            initializeReportSearch();    
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Organization</h1>

    <div class="sidebar">
        <navigation:Organization ID="SideNavigation" ActiveNavigation="personallyenrolled" runat="server" />
    </div>

    <div class="maincontent">
        <h2>Game Plan Submissions - Weekly</h2>

        <div class="well well-small well-white">
            <div id="searchwrapper">
                <h3>Search:</h3>
                <select id="lstSearchField" class="input-medium">
                    <option value="PeriodID" data-fieldtype="int" data-source="periods">Period ID</option>
                    <option value="RankID" data-fieldtype="int" data-source="ranks">Highest Rank Achieved</option>
                    <option value="PaidRankID" data-fieldtype="int" data-source="ranks">Paid As Rank</option>
                    <option value="Period.StartDate" data-fieldtype="date">Start Date</option>
                    <option value="Period.EndDate" data-fieldtype="date">End Date</option>
                    <option value="Volume1" data-fieldtype="int">PCV</option>
                    <option value="Volume3" data-fieldtype="int">OCV</option>
                    <option value="Volume75" data-fieldtype="int">3 Month PCV</option>
                    <option value="Volume79" data-fieldtype="int">3 Month OCV</option>
                </select>
            </div>

            <div class="gridreporttablewrapper">
                <table class="table table-condensed gridreporttable" id="gridreporttable">
                    <tr class="table-headers">
                        <th><a class="sortable" data-field="PeriodID">Period</a></th>
                        <th><a class="sortable" data-field="Volume99">Game Plan Reports Submitted In This Week</a></th>
                        <th><a class="sortable" data-field="Volume98">Lifetime Game Plan Reports Submitted</a></th>
                    </tr>
                </table>
            </div>

            <br />
            <br />


            <div class="gridreporttablewrapper">
                <table class="table table-condensed gridreporttable" id="gridreporttable2">
                    <tr class="table-headers">
                        <th><a class="sortable" data-field="Volume99">Game Plan Reports Submitted In This Week</a></th>
                        <th><a class="sortable" data-field="Volume98">Lifetime Game Plan Reports Submitted</a></th>
                        <th><a class="sortable" data-field="Volume99">Total Game Plan Reports Submitted In This Week By Your Downline</a></th>
                    </tr>
                </table>
            </div>




        </div>

    </div>
</asp:Content>

