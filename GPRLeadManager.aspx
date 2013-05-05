<%@ Page Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="GPRLeadManager.aspx.cs" Inherits="GPRLeadManager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" src="Assets/Scripts/jquery.min.js"></script>
    <script type="text/javascript" src="Assets/Scripts/exigo.util.js"></script>
    <script type="text/javascript" src="Assets/Scripts/exigo.report.js"></script>
    <script type="text/javascript" src="Assets/Scripts/exigo.report.searching.js"></script>


    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />




    <script type="text/javascript">
        // Set page variables
        var page = {
            activenavigation: 'organization'
        };

        $(function () {
            // Report
            setInitialSort('IndentedSort', report.sortOrderTypes.ASCENDING);
            initializeReport();
        });
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            if ($('INPUT[type="hidden"][id*="ShowMessage"]').val() != '') {
                $(function () {
                    $('DIV#Message').children('p').hide();
                    $('DIV#Message DIV.Close').children('p').hide();
                    $('DIV#Message').show();
                    $('DIV#Message').animate({ width: '400px' }, 400, function () {
                        $('DIV#Message DIV.Close').children('p').fadeIn('fast');
                    });
                    $('DIV#Message').children('p').fadeIn('slow');
                    setTimeout(function () {
                        $('DIV#Message DIV.Close').children('p').fadeOut("fast", function () {
                            $('DIV#Message').children('p').fadeOut("fast");
                            $('DIV#Message').animate({ width: '0px' }, 400);
                            $('DIV#Message').fadeOut("fast");
                        });
                    }, 5000);
                });
            }

            $('DIV#Message DIV.Close').hover(function () {
                $(this).css('background-color', '#315885');
            }).click(function () {
                $(this).css('background-color', '#5aa1f3');
                $(this).delay(200).fadeOut("fast", function () {
                    $('DIV#Message').children('p').fadeOut("fast");
                    $('DIV#Message').animate({ width: '0px' }, 400);
                    $('DIV#Message').fadeOut("fast");
                });

            });
        });
    </script>
    <asp:HiddenField ID="ShowMessage" runat="server" />

    <h1>GPR Manager</h1>

    <div class="sidebar">
        <navigation:Organization ID="SideNavigation" ActiveNavigation="leads" runat="server" />
    </div>

    <div class="maincontent">
        <div class="well well-white list">

            <div class="gridreporttablewrapper">
                <table class="table gridreporttable" id="gridreporttable">
                    <tr class="table-headers">

                        <th style="border-top:0px">Date (received)</th>
                        <th style="border-top:0px">Name</th>
                        <th style="border-top:0px">Phone #</th>
                        <th style="border-top:0px">Email</th>

                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
