﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="DownlineAutoships.aspx.cs" Inherits="DownlineAutoships" %>

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
            setInitialSort('ao.NextRunDate', report.sortOrderTypes.DESCENDING);
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
        <navigation:Organization ID="SideNavigation" ActiveNavigation="downlineautoships" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Downline Autoships</h2>

        <div class="well well-white list">
            <div id="searchwrapper">
                <h3>Search:</h3>
                <select id="lstSearchField" class="input-medium">
                    <option value="ao.CustomerID" data-fieldtype="int">ID</option>
                    <option value="c.FirstName" data-fieldtype="string">First Name</option>
                    <option value="c.LastName" data-fieldtype="string">Last Name</option>
                    <option value="c.Company" data-fieldtype="string">Company</option>
                    <option value="c.MainCountry" data-fieldtype="string" data-source="countries">Country</option>
                    <option value="c.MainState" data-fieldtype="string" data-source="states">State</option>
                    <option value="c.CustomerTypeID" data-fieldtype="string" data-source="types">Type</option>
                    <option value="c.CustomerStatusID" data-fieldtype="string" data-source="statuses">Status</option>
                    <option value="c.CreatedDate" data-fieldtype="date">Joined Date</option>
                    <option value="ao.NextRunDate" data-fieldtype="int">Next Run Date</option>
                    <option value="ao.Total" data-fieldtype="int">Total</option>
                    <option value="ao.BusinessVolumeTotal" data-fieldtype="int">BV</option>
                </select>
            </div>

            <div class="gridreporttablewrapper">
                <table class="table gridreporttable" id="gridreporttable">
                    <tr class="table-headers">
                        <th><a class="sortable" data-field="ao.CustomerID">ID</a></th>
                        <th><a class="sortable" data-field="c.LastName">Name</a></th>
                        <th><a class="sortable" data-field="ao.NextRunDate">Next Run Date</a></th>
                        <th><a class="sortable" data-field="ao.Total">Total</a></th>
                        <th><a class="sortable" data-field="ao.BusinessVolumeTotal">BV</a></th>
                        <th class="column-alignright">Actions</th>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>

