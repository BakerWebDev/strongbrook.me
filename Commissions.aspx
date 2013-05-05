<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="Commissions.aspx.cs" Inherits="Commissions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/commissions.min.css" rel="stylesheet" />
    <script>
        // Set page variables
        var page = {
            activenavigation: 'commissions'
        };


        var LoadingImageURL = '<div class="preloader"><img src="Assets/Images/imgLoading.gif" alt="Loading..." title="Loading" /></div>';

        function LoadCustomerSections(id, type) {
            LoadSection('#summarywrapper', 'summary', 'id=' + id + '&type=' + type);
            LoadSection('#detailswrapper', 'details', 'id=' + id + '&type=' + type);
            LoadSection('#volumeswrapper', 'volumes', 'id=' + id + '&type=' + type);
            LoadSection('#bonuseswrapper', 'bonuses', 'id=' + id + '&type=' + type);
        }

        function LoadSection(selector, dataKey, query) {
            $(selector).html(LoadingImageURL);
            $.ajax({
                url: '<%=Request.Url.AbsolutePath %>?datakey=' + dataKey + '&' + query,
                cache: false,
                method: 'GET',
                success: function (data) {
                    $(selector).hide().html(data).fadeIn('fast');
                },
                error: function (data, status, error) {
                    alert('error: ' + error);
                }
            });
        }

        $(function () {
            $('#commissionperiods').on('change', function (event) {
                var $triggerElement = $(event.target);
                var $selectedOption = $triggerElement.find('option:selected');
                LoadCustomerSections($triggerElement.val(), $selectedOption.attr('data-type'));
            }).trigger('change').focus();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Commissions</h1>

    <div class="sidebar">
        <navigation:Commissions ID="SideNav" ActiveNavigation="commissions" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Commissions</h2>
        <div class="well well-large well-white">
            <h3>Choose a period:</h3>
                <% RenderAvailablePeriodsDropdown(); %>
            <hr />
            <div id="commissionswrapper">
                <div class="row-fluid">
                    <span class="span12">
                        <div id="summarywrapper" class="section">
                        </div>
                    </span>
                </div>
                <div class="row-fluid">
                    <span class="span6">
                        <div id="detailswrapper" class="section">
                        </div>
                    </span>
                    <span class="span6">
                        <div id="bonuseswrapper" class="section">
                        </div>
                        <div id="volumeswrapper" class="section">
                        </div>
                    </span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

