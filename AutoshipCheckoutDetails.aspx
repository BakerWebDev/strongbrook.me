<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="AutoshipCheckoutDetails.aspx.cs" Inherits="AutoshipCheckoutDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'autoships'
        };
    </script>


    <link href="Assets/Themes/ui-lightness/jquery-ui.custom.css" rel="stylesheet" />
    <link href="Assets/Styles/shopping.min.css" rel="stylesheet" type="text/css" />
    <link href="Assets/Plugins/jquery.validationEngine/css/validationEngine.jquery.css" rel="stylesheet" type="text/css" />
    <script src="Assets/Scripts/jquery-ui.min.js"></script>
    <script src="Assets/Plugins/jquery.validationEngine/js/languages/jquery.validationEngine-en.js" type="text/javascript"></script>
    <script src="Assets/Plugins/jquery.validationEngine/js/jquery.validationEngine.js" type="text/javascript"></script>
    <script src="Assets/Plugins/jquery.validationEngine/js/contrib/other-validations.js" type="text/javascript"></script>



    <script type="text/javascript" language="javascript">

        function SaveChanges() {
            var isValid = $("form").first().validationEngine('validate');
            if (isValid) {
                <%=Page.ClientScript.GetPostBackEventReference(this, "SaveChanges")%>
            }
        }


        function invalidDate(date) {
            var m = date.getMonth(), d = date.getDate(), y = date.getFullYear(), day = date.getDay();

            // Disable all days 28 and up
            if (d > 28) {
                return [false];
            }

            return [true];
        }
        function noWeekendsOrHolidays(date) {
            var noWeekend = $.datepicker.noWeekends(date);
            return invalidDate(date);
        }

        $(function () {
            $('#txtStartDate').datepicker({
                dateFormat: 'DD, MM d, yy',
                numberOfMonths: 2,
                showButtonPanel: true,
                minDate: "<%=(Autoship.PropertyBag.ExistingAutoshipID == 0) ? "0" : "+1D" %>",
                maxDate: "+1Y",
                constrainInput: true,
                beforeShowDay: noWeekendsOrHolidays
            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />

    <h1><%=Resources.Shopping.Autoships %></h1>

    <div class="sidebar">
        <navigation:Autoships ID="SideNavigation" ActiveNavigation="cart" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            <div id="shopping">
                <div id="shoppingcheckout">

                    <h2><%=Resources.Shopping.ConfigureYourAutoship %></h2>

                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="fieldlabel">
                                <%=(Autoship.PropertyBag.ExistingAutoshipID == 0) ? Resources.Shopping.WhenShouldYourAutoshipStart : Resources.Shopping.NextProcessingDate %>
                            </td>
                            <td class="fields">
                                <div class="fieldwrapper width255">
                                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="validate[required] text-input" ClientIDMode="Static" />
                                    <div class="FieldInstructions">
                                        <%=Resources.Shopping.Note_AutoshipProcessingMessage %>
                                    </div>
                                    <br />
                                </div>
                            </td>
                        </tr>

                        <% if(Autoship.AutoshipSettings.AvailableFrequencyTypes.Count > 1)
                           { %>
                        <tr>
                            <td class="fieldlabel"><%=Resources.Shopping.YourAutoshipFrequency %>:
                            </td>
                            <td class="fields">
                                <div class="fieldwrapper width255">
                                    <asp:DropDownList ID="lstFrequency" runat="server" CssClass="validate[required]" />

                                    <div class="fieldinstructions">
                                        <%=Resources.Shopping.HowOftenDoYouWantThisAutoship %>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <% } %>

                        <tr>
                            <td class="fieldlabel">&nbsp;
                            </td>
                            <td class="fields">
                                <br />
                                <a href="javascript:SaveChanges();" class="btn btn-success next"><%=Resources.Shopping.Continue %></a>
                            </td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
