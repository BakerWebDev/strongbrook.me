<%@ Page Language="C#" MasterPageFile="~/MasterPages/PopUp.Master" AutoEventWireup="true"
    CodeFile="GamePlanSubmissionForm.aspx.cs" Inherits="GamePlanSubmissionForm" %>

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
            <div class="panels" id="panels" style="position: relative;">
                <div class="j" id="introduction" style="display: block; position: absolute; width: 100%;">
                    <h1 class="heading">Request Your Free Personalized Game Plan Report</h1>
                    <table id="grid1">
                        <tbody>
                            <tr style="margin-top: 0px; vertical-align: top;">
                                <td colspan="2" class="leftSide">
                                    <div class="notes panel">
                                        Congratulations on taking your first step to requesting a one-on-one custom Game
                                        Plan Report. Your Game Plan Report will show all of your financial options moving
                                        you closer to achieving your retirement goals and dreams. Receiving this free report
                                        only takes a few minutes after we get a hold of you.
                                        <br />
                                        <br />
                                        Please complete the information to the right to make your request and we will attempt
                                        to contact you in the next business day.
                                        <br />
                                        <br />
                                        <i style="font-size: 80%">NOTICE: The time you select for your Game Plan is simply a
                                            request. We will do our best to accommodate your requested time, but we cannot guarantee
                                            the availability of a Strongbrook Representative at that exact time. Thank you for
                                            your understanding and we look forward to speaking with you!</i>
                                    </div>
                                </td>
                                <td class="rightSide">
                                    <table class="record">
                                        <tbody>
                                            <tr class="recordrow">
                                                <td class="recordlabel">
                                                    <div class="fieldlabel">
                                                        First Name
                                                    </div>
                                                </td>
                                                <td class="recordvalue">
                                                    <div class="fieldvalue">
                                                        <asp:TextBox CssClass="input textfield" ID="txtFirstName" name="FirstName"
                                                            runat="server" Data="First Name" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr class="recordrow">
                                                <td class="recordlabel">
                                                    <div class="fieldlabel">
                                                        Last Name
                                                    </div>
                                                </td>
                                                <td class="recordvalue">
                                                    <div class="fieldvalue">
                                                        <asp:TextBox CssClass="input textfield" ID="txtLastName" name="LastName"
                                                            runat="server" Data="Last Name" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr class="recordrow">
                                                <td class="recordlabel">
                                                    <div class="fieldlabel">
                                                        Phone 1
                                                    </div>
                                                </td>
                                                <td class="recordvalue full">
                                                    <div class="fieldvalue">
                                                        <asp:TextBox CssClass="input textfield" ID="txtPhone1" name="homephone" runat="server"
                                                            Data="Phone" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr class="recordrow">
                                                <td class="recordlabel">
                                                    <div class="fieldlabel">
                                                        Phone 2
                                                    </div>
                                                </td>
                                                <td class="recordvalue">
                                                    <div class="fieldvalue">
                                                        <asp:TextBox CssClass="input textfield" ID="txtPhone2" name="cellphone" runat="server"
                                                            Data="Phone" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr class="recordrow">
                                                <td class="recordlabel">
                                                    <div class="fieldlabel">
                                                        Email
                                                    </div>
                                                </td>
                                                <td class="recordvalue">
                                                    <div class="fieldvalue">
                                                        <asp:TextBox CssClass="input textfield" ID="txtEmail"
                                                            name="email" runat="server" Data="email" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr class="recordrow">
                                                <td class="recordlabel">
                                                    <div class="fieldlabel">
                                                        Likely Available
                                                    </div>
                                                </td>
                                                <td class="dropdowns">
                                                    <div class="time">
                                                        <asp:DropDownList ID="lstAvailableTime" ClientIDMode="Static" runat="server" />
                                                    </div>
                                                    <div class="zone">
                                                        <asp:DropDownList ID="timeZones" ClientIDMode="Static" runat="server" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr class="recordrow">
                                                <td class="recordlabel">
                                                    <div class="fieldlabel">
                                                        Comments
                                                    </div>
                                                </td>
                                                <td class="recordvalue">
                                                    <div class="fieldvalue">
                                                        <div class="border line" id="commentsborder" style="display: inline-block;">
                                                            <div class="textareabounds" id="commentsbody" style="width: 200px; min-height: 50px;">
                                                                <textarea id="txtComments" class="input textfield" cols="20" rows="5" runat="server" ></textarea>
                                                            </div>
                                                            <div class="border-resize" id="commentsresize">
                                                            </div>
                                                            <div class="topleft">
                                                            </div>
                                                            <div class="topright">
                                                            </div>
                                                            <div class="bottomleft">
                                                            </div>
                                                            <div class="bottomright">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <input class="input hidden" name="ibdemail" id="ibdemail" value="" type="hidden">
                    <input class="input hidden" name="ibdphone" id="ibdphone" value="" type="hidden">
                    <input class="input hidden" name="ibdname" id="ibdname" value=" " type="hidden">
                    <input class="input hidden" name="ibd" id="ibd" value="" type="hidden">
                </div>
            </div>
            <div class="panelRight" id="button">
                <div class="schedulerbuttons panel">
                    <table class="toolbar">
                        <tbody>
                            <tr>
                                <td class="toolbar-expand"></td>
                                <td class="toolbar-expand"></td>
                                <td class="toolbar-cell toolbar-right toolbar-last">
                                    <div id="loginButton" class="login-button">
                                        <asp:Button ID="submitButton" runat="server" Text="Submit" CausesValidation="true"
                                            OnClientClick="foop();" />
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
