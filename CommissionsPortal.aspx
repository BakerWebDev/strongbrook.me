<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="CommissionsPortal.aspx.cs" Inherits="CommissionsPortal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'commissions'
        };
    </script>
    <link href="Assets/Styles/commissioncheck.min.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Commissions</h1>

    <div class="sidebar">
        <navigation:Commissions ID="SideNav" ActiveNavigation="summary" runat="server" />
    </div>
    <div class="maincontent">
        <div style="width: 800px;">
            <div class="commissioncheck">
                <div class="name">
                    <%if(Identity.Current.PayableToName != null){ %>
                    <%=Identity.Current.PayableToName %>, ID# <%=Identity.Current.CustomerID %>
                    <%} else{ %>
                    <%=Identity.Current.FirstName + " " + Identity.Current.LastName %>, ID# <%=Identity.Current.CustomerID %>
                    <%} %>
                </div>
                <div class="date">
                    <%=DateTime.Now.ToString("M/d/yyyy") %>
                </div>
                <div class="amount">
                    <%=CommissionCheckDetails.CommissionTotal.ToString("N2") %>
                </div>
                <div class="amountstring">
                    <% RenderCommissionCheckAmountInEnglish(CommissionCheckDetails.CommissionTotal); %>
                </div>
                <div class="for">
                    <%=CommissionCheckDetails.PeriodDescription %>
                </div>
            </div>
            <div id="PageDescription" class="well white" style="margin-top: 20px; font-weight:bold">
                <p>
                    This check image reflects your projected potential earnings in the current MONTHLY commission run.  This check image does NOT reflect any weekly, quarterly, or annual commissions.
                </p>
                <p>
                    Please be advised that this commission amount is subject to change based on your Active status, final Rank qualifications by you and your downline team members, as well as any potential refunds within your sales team.
                </p> 
                <p>
                    The final commission amount will not be determined until the commission run is approved and accepted prior to commission checks being mailed on the 15th of the following month.
                </p>
            </div>
        </div>
    </div>
</asp:Content>

