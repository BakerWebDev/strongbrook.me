<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeFile="ShoppingCheckoutShippingAddress.aspx.cs" Inherits="ShoppingCheckoutShippingAddress" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'orders'
        };
    </script>

    <link href="Assets/Styles/shopping.min.css" rel="stylesheet" type="text/css" />
    <link href="Assets/Plugins/jquery.validationEngine/css/validationEngine.jquery.css" rel="stylesheet" type="text/css" />
    <script src="Assets/Plugins/jquery.validationEngine/js/languages/jquery.validationEngine-<%=Resources.Shopping.LanguageCodePrefix %>.js" type="text/javascript"></script>
    <script src="Assets/Plugins/jquery.validationEngine/js/jquery.validationEngine.js" type="text/javascript"></script>
    <script src="Assets/Plugins/jquery.validationEngine/js/contrib/other-validations.js" type="text/javascript"></script>



    <script type="text/javascript">
        $(function () {
            $("form").first().validationEngine();

            $('#lstCountry').bind('change', function (event) {
                var $toggleElement = $(event.target),
                    $address2 = $('[data-role="address2"]');

                if ($toggleElement.val() == "US") $address2.hide();
                else $address2.show();
            }).trigger('change');
        });


        function ShipToNewAddress() {
            if ($('[validationgroup="NewShippingAddress"]').validationEngine('validate')) {
                <%=Page.ClientScript.GetPostBackEventReference(this, "ShipToAddress|" + ShoppingCartPropertyBag.AddressType.New)%>
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    
    <h1><%=Resources.Shopping.Orders %></h1>

    <div class="sidebar">
        <navigation:Orders ID="SideNavigation" ActiveNavigation="summary" runat="server" />
    </div>
    <div class="maincontent">
        <div class="well well-large well-white">
            <div id="shopping">
                <div id="shoppingcheckout">
                    <% if(HasOneOrMoreValidAddressesOnFile())
                       { %>
                    <h2><%=Resources.Shopping.SelectAShippingAddress %></h2>
                    <p><%=Resources.Shopping.ShippingAddressOnFileConfirmationMessage %></p>
                    <br />
                    <table width="100%">
                        <tr>
                            <% RenderAddress(ShoppingCartPropertyBag.AddressType.Main); %>
                            <% RenderAddress(ShoppingCartPropertyBag.AddressType.Mailing); %>
                            <% RenderAddress(ShoppingCartPropertyBag.AddressType.Other); %>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <hr />
                    <br />
                    <h2><%=Resources.Shopping.OrEnterNewAddress %></h2>
                    <% }
                       else
                       { %>
                    <h2><%=Resources.Shopping.EnterYourShipAddress %></h2>
                    <% } %>
                    <p><%=Resources.Shopping.ClickShipToThisAddressWhenDone %>
                    </p>
                    <br />
                    <div validationgroup="NewShippingAddress">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td class="fieldlabel"><%=Resources.Shopping.YourName %>
                                </td>
                                <td class="fields">
                                    <div class="fieldwrapper width120">
                                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="validate[required] text-input" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.FirstName %>
                                        </div>
                                    </div>
                                    <div class="fieldwrapper width120">
                                        <asp:TextBox ID="txtLastName" runat="server" CssClass="validate[required] text-input" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.LastName %>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="fieldlabel"><%=Resources.Shopping.YourEmail %>
                                </td>
                                <td class="fields">
                                    <div class="fieldwrapper width255">
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="validate[required,custom[email]] text-input" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.EmailAddress %>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="fieldlabel"><%=Resources.Shopping.YourPhoneNumber %>
                                </td>
                                <td class="fields">
                                    <div class="fieldwrapper width255">
                                        <asp:TextBox ID="txtPhone" runat="server" CssClass="validate[required] text-input" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.Primary %>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="fieldlabel"><%=Resources.Shopping.YourAddress %>
                                </td>
                                <td class="fields">
                                    <div class="fieldwrapper width255">
                                        <asp:TextBox ID="txtAddress1" runat="server" CssClass="validate[required] text-input" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.StreetAddress %>
                                        </div>
                                    </div>
                                    <div class="clearfix">
                                    </div>
                                    <div class="fieldwrapper width255" data-role="address2">
                                        <asp:TextBox ID="txtAddress2" runat="server" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.ApartmentSuiteNumber %>
                                        </div>
                                    </div>
                                    <div class="clearfix">
                                    </div>
                                    <div class="fieldwrapper width120">
                                        <asp:TextBox ID="txtCity" runat="server" CssClass="validate[required] text-input" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.City %>
                                        </div>
                                    </div>
                                    <div class="fieldwrapper width120">
                                        <asp:UpdatePanel ID="Update_State" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="lstState" runat="server" CssClass="validate[required]" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="lstCountry" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.State %>
                                        </div>
                                    </div>
                                    <div class="clearfix">
                                    </div>
                                    <div class="fieldwrapper width120">
                                        <asp:TextBox ID="txtZip" runat="server" CssClass="validate[required] text-input" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.ZipCode %>
                                        </div>
                                    </div>
                                    <div class="fieldwrapper width120">
                                        <asp:DropDownList ID="lstCountry" runat="server" CssClass="validate[required]" OnSelectedIndexChanged="PopulateRegions_SelectedIndexChanged"
                                            AutoPostBack="true" ClientIDMode="Static" />
                                        <div class="fieldinstructions">
                                            <%=Resources.Shopping.Country %>
                                        </div>
                                    </div>
                                    <div class="clearfix">
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;
                                </td>
                                <td class="fields">
                                    <br />
                                    <a href="javascript:ShipToNewAddress();" class="btn btn-success"><%=Resources.Shopping.ShipToThisAddress %></a>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
