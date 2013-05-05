<%@ Page Title="" Language="C#" MasterPageFile="~/SitePages/MasterPages/SitePages.master" AutoEventWireup="true" CodeFile="GPR_MonthlyDetails.aspx.cs" Inherits="GPR_MonthlyDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="../Assets/Styles/reports.min.css" rel="stylesheet" />
    <script src="../Assets/Scripts/exigo.util.js"></script>
    <script src="../Assets/Scripts/exigo.report.js"></script>
    <script src="../Assets/Scripts/exigo.report.searching.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'organization'
        };
        
        $(function () {
            // Report
            setInitialSort('IndentedSort', report.sortOrderTypes.ASCENDING);
            initializeReport();

            // Searching in report
            searching.listdatasources.countries = <%=SqlReportSearchListJsonDataSource.Countries() %>;
            searching.listdatasources.states = <%=SqlReportSearchListJsonDataSource.States() %>;
            searching.listdatasources.ranks = <%=SqlReportSearchListJsonDataSource.Ranks() %>;
            searching.listdatasources.types = <%=SqlReportSearchListJsonDataSource.CustomerTypes() %>;
            searching.listdatasources.statuses = <%=SqlReportSearchListJsonDataSource.CustomerStatuses() %>;
            initializeReportSearch();    
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Organization</h1>

    <div class="sidebar">
        <sitePagesNavigation:Organization ID="SideNavigation" ActiveNavigation="leads" runat="server" />
    </div>

    <div class="maincontent">
        <h2>Game Plan Submissions - Monthly</h2>
        <div class="well well-white list">

            <div id="searchwrapper">
                <h3>Search:</h3>
                <span style="font-size:21px; margin-bottom:8px; padding-right:2px; font-family:SegoeUILight; color:#0088cc; ">If the:</span>
                <select id="lstSearchField" class="input-medium">
                    <option value="CustomerID" data-fieldtype="int">ID</option>
                    <option value="Level" data-fieldtype="int">Level</option>
                    <option value="Customer.FirstName" data-fieldtype="string">First Name</option>
                    <option value="Customer.LastName" data-fieldtype="string">Last Name</option>
                    <option value="Customer.Company" data-fieldtype="string">Company</option>
                    <option value="Customer.MainCountry" data-fieldtype="string" data-source="countries">Country</option>
                    <option value="Customer.MainState" data-fieldtype="string" data-source="states">State</option>
                    <option value="Customer.CustomerType.CustomerTypeID" data-fieldtype="string" data-source="types">Type</option>
                    <option value="Customer.CustomerStatus.CustomerStatusID" data-fieldtype="string" data-source="statuses">Status</option>


                    <option value="PeriodVolume.Volume83" data-fieldtype="int">Monthly Personal</option>
                    <option value="PeriodVolume.Volume82" data-fieldtype="int">Lifetime Personal</option>
                    <option value="PeriodVolume.Volume100" data-fieldtype="int">Monthly Organizaion</option>
                    <option value="PeriodVolume.Volume98" data-fieldtype="int">Lifetime Organizaion</option>


                </select>
            </div>

            <div class="gridreporttablewrapper">
                <table class="table gridreporttable" id="gridreporttable">
                    <tr class="table-headers">
                        <th><a class="sortable" data-field="IndentedSort">Level</a></th>
                        <th><a class="sortable" data-field="CustomerID">ID</a></th>
                        <th><a class="sortable" data-field="Customer.LastName">Name</a></th>

                        <th><a class="sortable" data-field="PeriodVolume.Volume83">Monthly Personal</a></th>                        
                        <th><a class="sortable" data-field="PeriodVolume.Volume82">Lifetime Personal</a></th>
                        <th><a class="sortable" data-field="PeriodVolume.Volume100">Monthly Organizaion</a></th>
                        <th><a class="sortable" data-field="PeriodVolume.Volume98">Lifetime Organizaion</a></th>
                    </tr>
                </table>
            </div>
            <div style="margin-bottom:15px"></div>
            <div class="well well-gray">
                <span style="color:blue; font-size:23px; vertical-align:top">*</span><span style="margin-left:3px; vertical-align:middle">The average number of Game Plan Report requests (GPRRs) per Active IBD in your sales team during the past 4 weeks.</span><br />
            </div>
        </div>
    </div>
</asp:Content>

