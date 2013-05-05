<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="EmailNotifications.aspx.cs" Inherits="EmailNotifications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'myaccount'
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>My Account</h1>

    <div class="sidebar">
        <navigation:MyAccount ID="SideNavigation" ActiveNavigation="notifications" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Email Notifications</h2>
        <div class="well well-large well-white">


            <exigo:ErrorManager ID="Error" runat="server" />


            <asp:Panel ID="Panel_OptIn" runat="server">
                <script type="text/javascript" language="javascript">

                    var currentOptInStatus = <%=Customer.IsSubscribedToBroadcasts.ToString().ToLower() %>;
                    var currentEmail = '<%=Customer.Email %>';

                    // Validates the page and calls the PostBack event that fires the email server-side.
                    function SubmitForm() {
                        // First check to see if the page is valid. 
                        // If the page does not have any .NET field validators, Page_ClientValidate() will not exist. We check to see if it exists, and act appropriately.
                        var PageIsValid = true;
                        if(typeof(Page_ClientValidate) == 'function') PageIsValid = !Page_ClientValidate();

                        // If the page is valid, hit the PostBack and fire the email server-side.
                        if (PageIsValid) {
                            if(currentOptInStatus) {
                                if(currentEmail != $('#txtEmail').val()) {
                                    $.msgbox("Are you sure you want your email notifications sent to '" + $('#txtEmail').val() + "' instead of '<%=Customer.Email %>'?", {
                                        type: "confirm",
                                        buttons: [
                                    { type: "submit", value: "Yes" },
                                    { type: "cancel", value: "No" }
                                        ]
                                    }, function (result) {
                                        if(result == "Yes") {
                                            <%=Page.ClientScript.GetPostBackEventReference(this, "SubmitForm") %>
                                        }
                                    });
                                }
                                else {
                                    $.msgbox("You are already subscribed to email notifications with this email address.", { type: "error" });
                                }
                            }
                            else {
                                <%=Page.ClientScript.GetPostBackEventReference(this, "SubmitForm") %>
                            }
                        }
                    }

                    function ResendEmail() {
                        <%=Page.ClientScript.GetPostBackEventReference(this, "ResendEmail") %>
                    }

                </script>
                <div class="well">
                    <% RenderOptInStatus(); %>
                </div>
                <p>To subscribe to company emails, click the "Subscribe" button below. An email will
                    be sent to the email address on your account to verify your email address.
                </p>
                <p>Email Address<br />
                    <asp:TextBox ID="txtEmail" runat="server" Width="250" ClientIDMode="Static" />
                    <asp:RequiredFieldValidator ID="rtxtEmail" runat="server" Display="Dynamic" ErrorMessage="<br />Please enter the email address you want to receive email notifications with."
                        ControlToValidate="txtEmail" SetFocusOnError="true" />
                    <asp:RegularExpressionValidator ID="retxtEmail" runat="server" ControlToValidate="txtEmail"
                        ErrorMessage="<br />The email address you entered does not appear to be a valid email address. Please check your spelling and try again."
                        Enabled="true" SetFocusOnError="true" ValidationExpression="^(?:[a-zA-Z0-9_'^&amp;/+-])+(?:\.(?:[a-zA-Z0-9_'^&amp;/+-])+)*@(?:(?:\[?(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))\.){3}(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\]?)|(?:[a-zA-Z0-9-]+\.)+(?:[a-zA-Z]){2,}\.?)$" />
                </p>
                <div class="Navigation">
                    <a href="javascript:;" onclick="SubmitForm()" class="btn btn-success">Subscribe</a>
                </div>
            </asp:Panel>


            <asp:Panel ID="Panel_Complete" runat="server" Visible="false">
                <h2>Almost done!</h2>
                <p>Thank you! Please check your email inbox, and click the link found in the confirmation
                    email.
                </p>
                <% if(Customer.IsSubscribedToBroadcasts)
                   { %>
                <p>You will continue to receive email notifications from Exigo at '<%=Customer.Email %>'
                    until you confirm your new email address.
                </p>
                <% }
                   else
                   { %>
                <p>You will not receive email notifications from Exigo until you confirm your address.
                </p>
                <% } %>
            </asp:Panel>
            <div class="clearfix"></div>
        </div>
    </div>
</asp:Content>

