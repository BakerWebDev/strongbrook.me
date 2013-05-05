<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" MasterPageFile="~/MasterPages/Site.master" %>

<asp:Content ID="Head1" runat="server" ContentPlaceHolderID="Head">
    <script src="Assets/Plugins/jquery.masonry/jquery.masonry.min.js"></script>
    <script>
        // Set page variables
        var page = {
            activenavigation: 'home'
        };


        function bounceIcons(element) {
            $(element).find('.animated').effect("bounce", { times: 3, distance: 7 }, 1000);
        }

        var ajaxCount = 0;
        var ajaxMaxTries = 10;
        var ajaxDelay = 3000;
        function loadRankAdvancement(rankID) {
            if(!rankID) rankID = <%=Identity.Current.Ranks.CurrentPeriodRankID%>;

            $.ajax({
                url: '<%=Request.Url.AbsolutePath%>?datakey=qualifications',
                cache: false,
                type: 'GET',
                data: {
                    rid: rankID
                },
                success: function(data) {
                    if(data == "0") {
                        ajaxCount++;

                        if(ajaxCount < ajaxMaxTries) {
                            setTimeout('loadRankAdvancement(' + rankID + ')', ajaxDelay);
                        }
                        else {
                            $('.widget-rankadvancement').html("We are unable to load your status at this time. Please try again later.");
                        }
                    }
                    else {
                        $('.widget-rankadvancement').html(data);
                        $('.widgets').masonry('reload');
                    }
                }
            });
        }

        $(function () {
            $('.tiles').masonry({
                itemSelector: '.tile',
                columnWidth: 100,
                gutterWidth: 15,
                isAnimated: true,
                isResizable: true
            });
            $('.widgets').masonry({
                itemSelector: '.widget',
                columnWidth: 100,
                gutterWidth: 15,
                isAnimated: true,
                isResizable: true
            });


            $(".tile-animated").each(function () {
                $(this).on('mouseenter', function () {
                    bounceIcons($(this));
                }).triggerHandler('mouseenter');
            });

            setInterval("bounceIcons($('.tile-animated'));", 5000);

            loadRankAdvancement();
        });

    </script>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Content">
    <h1><%=Identity.Current.FirstName + " " + Identity.Current.LastName %></h1>
    <div class="row-fluid" style="min-width: 740px;">
        <span id="Left_Column" class="span3">
            <div class="tiles">
                <a href="Messages.aspx" title="Use this convenient messaging system to email your downline.">
                    <% RenderMessagesTile(); %>
                </a>
                <a href="Calendar.aspx">
                    <div class="tile tile-icon size-1x1 theme-amethyst2" title="Direct access to the Strongbrook national events calendar.">
                        <img style="max-height:48px;" src="Assets/Images/icnCalendar.png" />
                        <h4 style="padding-right:4px;"><%=Resources.Dashboard.Events %></h4>
                    </div>
                </a>
            </div>
            <div class="tiles" style="min-width: 300px; max-width: 301px;">
                <div id="Actual_Months_Qualified_Rank" class="tile tile-icon size-2x1andAhalf theme-skyblue" title="Your highest Rank title achieved in the compensation plan. You always carry the title of the highest Rank reached.">
                    <h2><%=currentMonthsQualifiedRank %></h2>
                    <h4><%=Resources.Dashboard.ActualRankTitle %></h4>
                </div>
                <div id="Current_Months_Qualified_Rank" class="tile tile-icon size-2x1andAhalf theme-peach2" title="The Rank you have achieved in the current month to date based on your 3 month OCV and the compensation plan qualification criteria.
You always carry the title of the highest rank you reach, but are “Paid As” the rank for which you qualify each month.">
                    <h2><%=currentMonthsQualifiedRank %></h2>
                    <h4><%=Resources.Dashboard.CurrentMonthsQualifiedRank %></h4>
                </div>
                <div id="Guaranteed_Minimum_PaidAs_Rank" class="tile tile-icon size-2x1andAhalf theme-amethyst2" title="Your qualified rank in any given month prequalifies you at that “Paid As” level for the next two months in the future. You are guaranteed to be “Paid As” no less than this Rank.">
                    <h2><%=guaranteedMinPaidAsRank %></h2>
                    <h4><%=Resources.Dashboard.GuaranteedMinPaidAsRank %></h4>
                </div>
            </div>
        </span>
        <span id="Middle_Column" class="span5">
            <div class="tiles" style="min-width: 399px; max-width: 400px;">
                <div id="Row_1">
                    <a id="CurrentMonthPCV" href="Volumes.aspx" >
                        <div class="tile tile-icon size-1x1 theme-aqua2" title="Your current month to date PCV.">
                            <h2><%=Volumes.Volume1.ToString("N0") %></h2>
                            <h4 style="font-size:10px;"><%=Resources.Dashboard.PersonalVolume %></h4>
                        </div>
                    </a>
                    <a id="LastMonthPCV" href="Volumes.aspx">
                        <div class="tile tile-icon size-1x1 theme-amethyst2" title="Your total PCV for the previous month.">
                            <h2><%=VolumesLastMonth.Volume1.ToString("N0") %></h2>
                            <h4><%=Resources.Dashboard.LastMonthPCV %></h4>
                        </div>
                    </a>
                    <a id="ThreeMonthPCV" href="Volumes.aspx">
                        <div class="tile tile-icon size-1x1 theme-skyblue" title="Your total PCV for the current month, added together with your total PCV for the previous 2 months. This 3 month PCV amount is used to determine your highest possible “Paid As” Rank in the current qualification period.">
                            <h2><%=Volumes.Volume75.ToString("N0") %></h2>
                            <h4><%=Resources.Dashboard.ThreeMonthPCV %></h4>
                        </div>
                    </a>
                </div>
                <div id="Row_2">
                    <a id="CurrentMonthOCV" href="Volumes.aspx">
                        <div class="tile tile-icon size-1x1 theme-aqua2" title="Your current month to date OCV.">
                            <h2><%=Volumes.Volume3.ToString("N0") %></h2>
                            <h4 style="font-size:10px;"><%=Resources.Dashboard.GroupVolume %></h4>
                        </div>
                    </a>
                    <a id="LastMonthOCV" href="Volumes.aspx">
                        <div class="tile tile-icon size-1x1 theme-amethyst2" title="Your total OCV for the previous month.">
                            <h2><%=VolumesLastMonth.Volume3.ToString("N0") %></h2>
                            <h4><%=Resources.Dashboard.LastMonthOCV %></h4>
                        </div>
                    </a>
                    <a id="ThreeMonthOCV" href="Volumes.aspx">
                        <div class="tile tile-icon size-1x1 theme-skyblue" title="Your total OCV for the current month, added together with your total OCV for the previous 2 months. This 3 month OCV amount is used to determine your highest possible “Paid As” Rank in the current qualification period.">
                            <h2><%=Volumes.Volume79.ToString("N0") %></h2>
                            <h4><%=Resources.Dashboard.ThreeMonthOCV %></h4>
                        </div>
                    </a>
<%--
                    <a id="TeamTotalGPRweekly" href="GamePlanReport_Weekly.aspx" title="Definition">
                        <% Render1WeekAvgGPRs(); %>
                    </a>

--%>
               </div>
                <div id="Row_3">
                    <div id="Strongbrook_News" class="tile size-3x1 theme-peach2">
                        <h2><%=GlobalSettings.Company.Name %> <%=Resources.Dashboard.News %></h2>
                        <p>
                            Sent each Friday to the email address
                            listed on your Strongbrook account. 
                        </p>
                        <% RenderCompanyNews(); %>
                    </div>
                </div>
            </div>
        </span>
        <span id="Right_Column" class="span4">
            <div class="tiles">

                <div id="GPR_Values">
                    <a id="PersonalWeeklyGPRs" href="GPR_WeeklyDetails.aspx">
                        <div class="tile tile-icon size-1x1 theme-peach2" 
                            title="The total number of Game Plan Report requests (GPRs) submitted by you and your entire sales team this week to date.">
                            <h2><% Render_Personal_GPR_Count_PeriodType_Weekly(); %></h2>
                            <h4>This Week GPRs</h4>
                        </div>
                    </a>

                    <a id="DownlineWeeklyAverage" href="GPR_MonthlyDetails.aspx">
                        <div class="tile tile-icon size-1x1 theme-peach2" 
                            title="The average number of Game Plan Report requests (GPRs) per Active IBD in your sales team during the past 4 weeks.">
                            <h2><% Render_UniLevelDownline_GPR_Average_Per_Month(); %></h2>
                            <h4>Ave GPRs per IBD</h4>
                        </div>
                    </a>
                </div>

                <div id="HomeTransactionCredits" class="tile tile-icon size-2x1 theme-aqua2" title="The total number of eligible home transaction credits you have earned towards the $35,000 Portfolio Builder Bonus.">
                    <h2><%=Volumes.Volume56.ToString("N0") %></h2>
                    <h4><%=Resources.Dashboard.HomeTransactionCredits %></h4>
                </div>

                <%--

                                <div id="CurrentEarnings" class="tile tile-icon size-2x1 theme-aqua2" title="The amount of your most recent commission check.">


                                    <table style="margin-left:50px; width:110px; height:50px">
                                        <%Render3MonthsOfEarnings(); %>
                                    </table>
                                    <h2><% RenderCurrentCheckReceivedAmount(); %></h2>
                                    <h4>Current Earnings for <% RenderCurrentCheckReceivedPeriodDescription(); %></h4>          
                                </div>

                --%>

                <div id="CorporateInfo" class="tile tile-icon size-2x4 theme-skyblue" style="width:205px; padding:5px;" title="Corporate Information">
                    <h2 style="margin-bottom:0px;">National Team Call</h2>
                    <p>
                        Every Monday morning at<br />
                        7a.m. PT / 8a.m. MT / 9a.m. CT / 10a.m. ET
                    </p>
                    <p>
                        LIVE CALL: 712-432-0075<br />
                        Access code: 358204#
                    </p>
                    <p>
                        PLAYBACK: 712-432-1085<br />
                        Access code: 358204#<br />
                        (only available for 1 week)
                    </p>
                    <p>
                        <a style="text-decoration:underline" href="http://christinegraham.wix.com/trainingcallarchive" target="_blank">Team Call Archives</a>
                    </p>
                </div>

                <div id="InfoCall" class="tile tile-icon size-2x4 theme-amethyst2" style="width:205px; padding:5px;" title="Info Call">
                    <h2 style="margin-bottom:10px;">Intro Call</h2>
                    <p>
                        Call 800-631-2131 for an 8 minute pre-recorded Investor and
                        Business Opportunity intro with Kris Krohn
                    </p>
                </div>

                <div id="ContactInfo" class="tile tile-icon size-2x4 theme-peach2" style="width:205px; padding:5px;" title="Info Call">
                    <h2 style="margin-bottom:10px;">Support Services</h2>
                    <p style="text-align:left;">
                        Mon. – Fri., 8:00am – 5:00pm MTN<br />
                        Phone: 801-691-0375<br />
                        <a href="mailto:support@strongbrookdirect.com?Subject=Support%20question">support@strongbrookdirect.com</a>

                    </p>
                </div>

            </div>
        </span>
    </div>
</asp:Content>
