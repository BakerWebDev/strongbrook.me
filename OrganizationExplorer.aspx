<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="OrganizationExplorer.aspx.cs" Inherits="OrganizationExplorer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />

    <script>
        // Set page variables
        var page = {
            activenavigation: 'organization'
        };



        var settings = {
            loading: '<p style="text-align: center"><img src="Assets/Images/imgLoading.gif" /></p>',
            currentpage: 1
        };


        function initializeReport(id) {
            loadSection('#downlinelist tbody', 'downlinelist', 'id=' + id + '&page=' + settings.currentpage);
            loadCustomerSections(id);
        }

        function loadCustomerSections(id) {
            loadSection('#summarywrapper', 'summary', 'id=' + id);
            loadSection('#optionswrapper', 'options', 'id=' + id);
            loadSection('#businesssnapshotwrapper', 'businesssnapshot', 'id=' + id);
            loadSection('#customerwallwrapper', 'wall', 'id=' + id);
            loadSection('#recentorderswrapper', 'orders', 'id=' + id);
            loadSection('#autoshipswrapper', 'autoships', 'id=' + id);
        }

        function loadSection(selector, dataKey, query) {
            $(selector).html(settings.loading);

            $.ajax({
                url: '<%=Request.Url.AbsolutePath%>?datakey=' + dataKey + '&' + query,
                cache: false,
                success: function (data) {
                    $(selector).html(data);
                },
                error: function (data, status, error) {
                    alert('error: ' + error);
                },
                dataType: "html"
            });
        }



        function isScrolledIntoView(elem) {
            var docViewTop = $(window).scrollTop();
            var docViewBottom = docViewTop + $(window).height();

            var elemTop = $(elem).offset().top;
            var elemBottom = elemTop + $(elem).height();

            return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
        }


        function DisableScrollBehavior(e) {
            e.preventDefault();
            return false;
        }


        $(function () {

            $('#downlinelistwrapper').scroll(function () {
                if (isScrolledIntoView($('#loadmoreajaxloader')) && settings.currentpage > 0) {
                    settings.currentpage++;

                    $('#loadmoreajaxloader').css('display', 'none');

                    $.ajax({
                        url: '<%=Request.Url.AbsolutePath%>?datakey=downlinelist&page=' + settings.currentpage,
                        success: function (html) {
                            if (html) {
                                $("#downlinelist").append(html);
                                $('#loadmoreajaxloader').css('display', 'block');
                            } else {
                                settings.currentpage = 0;
                                $('#loadmoreajaxloader').remove();
                                $('#downlinelistwrapper').unbind('scroll');
                            }
                        }
                    });
                }
            });


            initializeReport(<%=Identity.Current.CustomerID %>);
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Organization</h1>

    <div class="sidebar">
        <navigation:Organization ID="SideNavigation" ActiveNavigation="organizationexplorer" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Organization Explorer</h2>

        <div class="btn-toolbar">
            <a href="javascript:;" class="btn"><i class="icon-plus"></i>&nbsp;Enroll new</a>
        </div>

        <div class="well well-white">


            <div id="organizationdetailreport">


                <div class="row-fluid">
                    <span class="span3">
                        <h2>My Organization</h2>
                        <div id="downlinelistwrapper" class="well well-small">
                            <table id="downlinelist" style="width: 180px; margin-top: 0;">
                                <tbody>
                                </tbody>
                            </table>
                            <div id="loadmoreajaxloader" style="visibility: hidden;">
                                <p style="text-align: center">
                                    <img src="Assets/Images/imgLoading.gif" /></p>
                            </div>
                        </div>
                    </span>

                    <span class="span9">
                        <div class="row-fluid">
                            <span class="span9">
                                <h2>Profile</h2>
                                <div id="summarywrapper" class="well"></div>
                            </span>

                            <span class="span3">
                                <h2>&nbsp;</h2>
                                <ul class="nav nav-tabs nav-stacked" id="optionswrapper">
                                </ul>
                            </span>
                        </div>


                        <div class="row-fluid">
                            <span class="span6">
                                <h2>Rank Promotion Goals</h2>
                                <div id="businesssnapshotwrapper" class="well well-white"></div>
                            </span>
                            <span class="span6">
                                <h2>Recent Activity</h2>
                                <div id="customerwallwrapper" class="well well-white"></div>
                            </span>
                        </div>

                        <div class="row-fluid">
                            <span class="span12">
                                <h2>Recent Orders</h2>
                                <div id="recentorderswrapper" class="well well-white"></div>
                            </span>
                        </div>

                        <div class="row-fluid">
                            <span class="span12">
                                <h2>Auto-Refills</h2>
                                <div id="autoshipswrapper" class="well well-white"></div>
                            </span>
                        </div>
                    </span>
                </div>
            </div>


        </div>
    </div>
</asp:Content>

