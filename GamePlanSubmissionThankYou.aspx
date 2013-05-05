<%@ Page Language="C#" MasterPageFile="~/MasterPages/PopUp.Master" AutoEventWireup="true" CodeFile="GamePlanSubmissionThankYou.aspx.cs" Inherits="GamePlanSubmissionThankYou" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" type="text/css" href="Assets/Styles/schedule.css" />
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="Content">
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


    <div class="main-wrapper">
        <div class="scheduler panel">
            <%ShowThisMessage(); %>
        </div>
    </div>
</asp:Content>
