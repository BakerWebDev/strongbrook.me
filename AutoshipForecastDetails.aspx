<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="AutoshipForecastDetails.aspx.cs" Inherits="AutoshipForecastDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />

    <script>
        // Set page variables
        var page = {
            activenavigation: 'autoships'
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1><%=Resources.AutoshipForecast.Autoships %></h1>

    <div class="sidebar">
        <navigation:Autoships ID="SideNavigation" ActiveNavigation="forecast" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Autoship Forecast Details - <%=Period.PeriodDescription %><br />
            (<% if(LegFilter == LegFilterType.All)
                       { %> <%=Resources.AutoshipForecast.AllLegs %> <% } %>
            <% if(LegFilter == LegFilterType.Left)
               { %> <%=Resources.AutoshipForecast.LeftLegOnly %> <% } %>
            <% if(LegFilter == LegFilterType.Right)
               { %> <%=Resources.AutoshipForecast.RightLegOnly %> <% } %>)
        </h2>

        <div class="well well-white">

            <label for="lstLegFilter"><%=Resources.AutoshipForecast.FilterByLeg %>: </label>
                <asp:DropDownList ID="lstLegFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ChangeLegFilter_SelectedIndexChanged" ClientIDMode="Static" />

            <table class="table">
                <tr class="table-headers">
                    <th><%=Resources.AutoshipForecast.Distributors %></th>
                    <th><%=Resources.AutoshipForecast.Shipped %></th>
                    <th><%=Resources.AutoshipForecast.Volume %></th>
                    <th><%=Resources.AutoshipForecast.ShipDate %></th>
                    <th><%=Resources.AutoshipForecast.Leg %></th>
                </tr>

                <% foreach(var node in ViewModel) { %>
                    <tr>
                        <td>
                            <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(node.CustomerID) %>" style="float: left; margin-right: 10px;" />
                            <strong><%=node.DisplayName %></strong><br />
                            <%=Resources.AutoshipForecast.ID %># <%=node.CustomerID %>
                        </td>
                        <td><%=(node.Shipped) ? "<i class='icon-ok'></i>" : "" %></td>
                        <td><%=node.AutoshipVolume.ToString("N0") %></td>
                        <td><%=node.NextShipDate.ToString("M/d/yyyy") %></td>
                        <td><%=node.Leg %></td>
                    </tr>
                <% } %>

            </table>
        </div>
    </div>
</asp:Content>

