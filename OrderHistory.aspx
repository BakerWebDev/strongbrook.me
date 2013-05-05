<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="OrderHistory.aspx.cs" Inherits="OrderHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/exigo.util.js"></script>
    <script src="Assets/Scripts/exigo.report.js"></script>
    <script src="Assets/Scripts/exigo.report.searching.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'orders'
        };
        
        $(function () {
            // Report
            setInitialSort('OrderID', report.sortOrderTypes.DESCENDING);
            initializeReport();

            // Searching in report
            searching.listdatasources.statuses = <%=SqlReportSearchListJsonDataSource.OrderStatuses() %>;
            searching.listdatasources.types = <%=SqlReportSearchListJsonDataSource.OrderTypes() %>;
            initializeReportSearch();    
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Orders</h1>

    <div class="sidebar">
        <navigation:Orders id="SideNavigation" ActiveNavigation="orderhistory" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Order History</h2>

        <div class="well well-white list">
            <div id="searchwrapper">
                <h3>Search:</h3>
                <select id="lstSearchField" class="input-medium">
                    <option value="OrderID" data-fieldtype="int">Order</option>
                    <option value="OrderStatusID" data-fieldtype="int" data-source="statuses">Status</option>
                    <option value="OrderTypeID" data-fieldtype="int" data-source="types">Type</option>
                    <option value="OrderDate" data-fieldtype="date">Date</option>
                    <option value="Total" data-fieldtype="int">Total</option>
                    <option value="BusinessVolumeTotal" data-fieldtype="int">BV</option>
                    <option value="TrackingNumber1" data-fieldtype="string">Tracking</option>
                </select>
            </div>

            <div class="gridreporttablewrapper">
                <table class="table gridreporttable" id="gridreporttable">
                    <tr class="table-headers">
                        <th><a class="sortable" data-field="OrderID">Order</a></th>
                        <th><a class="sortable" data-field="OrderStatusID">Status</a></th>
                        <th><a class="sortable" data-field="OrderTypeID">Type</a></th>
                        <th><a class="sortable" data-field="OrderDate">Date</a></th>
                        <th><a class="sortable" data-field="Total">Total</a></th>
                        <th><a class="sortable" data-field="BusinessVolumeTotal">BV</a></th>
                        <th class="column-alignright">Tracking</th>
                        <th class="column-alignright">View Invoice</th>
                    </tr>
                </table>
            </div>

        </div>
    </div>
</asp:Content>

