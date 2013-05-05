<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="RetailCustomers.aspx.cs" Inherits="RetailCustomers" %>

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
            setInitialSort('Customer.LastName', report.sortOrderTypes.ASCENDING);
            initializeReport();

            // Searching in report
            searching.listdatasources.countries = <%=SqlReportSearchListJsonDataSource.Countries() %>;
            searching.listdatasources.states = <%=SqlReportSearchListJsonDataSource.States() %>;
            searching.listdatasources.types = <%=SqlReportSearchListJsonDataSource.CustomerTypes() %>;
            searching.listdatasources.statuses = <%=SqlReportSearchListJsonDataSource.CustomerStatuses() %>;
            initializeReportSearch(); 
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Organization</h1>

    <div class="sidebar">
        <navigation:Organization ID="SideNavigation" ActiveNavigation="retailcustomers" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Retail Customers</h2>

        <div class="well well-white list">
            <div id="searchwrapper">
                <h3>Search:</h3>
                <select id="lstSearchField" class="input-medium">
                    <option value="CustomerID" data-fieldtype="int">ID</option>
                    <option value="Customer.FirstName" data-fieldtype="string">First Name</option>
                    <option value="Customer.LastName" data-fieldtype="string">Last Name</option>
                    <option value="Customer.Company" data-fieldtype="string">Company</option>
                    <option value="Customer.MainCountry" data-fieldtype="string" data-source="countries">Country</option>
                    <option value="Customer.MainState" data-fieldtype="string" data-source="states">State</option>
                    <option value="Customer.CustomerTypeID" data-fieldtype="string" data-source="types">Type</option>
                    <option value="Customer.CustomerStatusID" data-fieldtype="string" data-source="statuses">Status</option>
                    <option value="Customer.CreatedDate" data-fieldtype="date">Joined Date</option>
                </select>
            </div>

            <div class="gridreporttablewrapper">
                <table class="table gridreporttable" id="gridreporttable">
                    <tr class="table-headers">
                        <th><a class="sortable" data-field="Customer.LastName">Name</a></th>
                        <th>Contact Info</th>
                        <th><a class="sortable" data-field="Customer.CreatedDate">Joined Date</a></th>
                        <th class="column-alignright">Actions</th>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>

