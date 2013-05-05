<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="CommissionBonusDetails.aspx.cs" Inherits="CommissionBonusDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/commissions.min.css" rel="stylesheet" />
    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/exigo.util.js"></script>
    <script src="Assets/Scripts/exigo.report.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'commissions'
        };


        $(function () {
            // Report
            setInitialSort('CommissionAmount', report.sortOrderTypes.DESCENDING);
            report.settings.requestUrl = "<%=Request.Url.AbsolutePath%>?action=fetch&<%=Request.Url.Query.Replace("?", "")%>";
            initializeReport();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Commissions</h1>

    <div class="sidebar">
        <navigation:Commissions ID="SideNav" ActiveNavigation="commissions" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Bonus Breakdown</h2>

        <div class="well well-white list">
            <div class="gridreporttablewrapper">
                <table class="table gridreporttable" id="gridreporttable">
                    <tr class="table-headers">
                        <th><a class="sortable" data-field="FromCustomerID">ID</a></th>
                        <th><a class="sortable" data-field="FromCustomer.LastName">Name</a></th>
                        <th><a class="sortable" data-field="OrderID">Order</a></th>
                        <th><a class="sortable" data-field="Level">Level</a></th>
                        <th><a class="sortable" data-field="PaidLevel">Paid Level</a></th>
                        <th><a class="sortable" data-field="SourceAmount">Source Amount</a></th>
                        <th><a class="sortable" data-field="CommissionAmount">Commission</a></th>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>

