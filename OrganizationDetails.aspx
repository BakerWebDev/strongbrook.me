<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="OrganizationDetails.aspx.cs" Inherits="OrganizationDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
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
        <navigation:Organization ID="SideNavigation" ActiveNavigation="organizationdetails" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Organization Details</h2>

<%--
        <div class="btn-toolbar">
            <a href="javascript:;" class="btn"><i class="icon-plus"></i>&nbsp;Enroll new</a>
        </div>
--%>

        <div class="well well-white list">
            <div id="searchwrapper">
                <h3>Search:</h3>
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
                    <option value="Customer.CreatedDate" data-fieldtype="date">Joined Date</option>
                    <option value="PeriodVolume.Volume1" data-fieldtype="int">PCV</option>
                    <option value="PeriodVolume.Volume3" data-fieldtype="int">OCV</option>
                    <option value="PeriodVolume.Volume75" data-fieldtype="int">3 Month PCV</option>
                    <option value="PeriodVolume.Volume79" data-fieldtype="int">3 Month OCV</option>
                    <option value="PeriodVolume.Volume56" data-fieldtype="int">HTC</option>
                </select>
            </div>

            <div class="gridreporttablewrapper">
                <table class="table gridreporttable" id="gridreporttable">
                    <tr class="table-headers">
                        <th><a class="sortable" data-field="IndentedSort">Level</a></th>
                        <th><a class="sortable" data-field="CustomerID">ID</a></th>
                        <th><a class="sortable" data-field="Customer.LastName">Name</a></th>
                        <th>Contact Info</th>
                        <th><a class="sortable" data-field="Customer.CreatedDate">Joined Date</a></th>
                        <th><a class="sortable" data-field="PeriodVolume.Volume1">*PCV</a></th>
                        <th><a class="sortable" data-field="PeriodVolume.Volume3">*OCV</a></th>
                        <th><a class="sortable" data-field="PeriodVolume.Volume75">**3 Month PCV</a></th>
                        <th><a class="sortable" data-field="PeriodVolume.Volume79">**3 Month OCV</a></th>
                        <th><a class="sortable" data-field="PeriodVolume.Volume56">HTC</a></th>
                        <th class="column-alignright">Actions</th>
                    </tr>
                </table>
            </div>
            <div style="margin-bottom:15px"></div>
            <div class="well well-gray">
                <span style="color:blue; font-size:23px; vertical-align:top">*</span><span style="margin-left:3px; vertical-align:middle">Data reflects current month-to-date figures.</span><br />
                <span style="color:blue; font-size:23px; vertical-align:top">**</span><span style="margin-left:3px; vertical-align:middle">Data reflects Rolling 3 Month Rank Qualification figures – Current month PCV/OCV added together with all PCV/OCV from each of the 2 previous months.</span>
            </div>
        </div>
    </div>
</asp:Content>

