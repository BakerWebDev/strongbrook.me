<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="UnilevelWaitingRoom.aspx.cs" Inherits="UnilevelWaitingRoom" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <script>
        // Set page variables
        var page = {
            activenavigation: 'organization'
        };
    </script>

    <link href="Assets/Styles/waitingroom.min.css" rel="stylesheet" type="text/css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Organization</h1>

    <div class="sidebar">
        <navigation:Organization ID="SideNavigation" ActiveNavigation="unilevelwaitingroom" runat="server" />
    </div>
    <div class="maincontent">
        <h2>Unilevel Waiting Room</h2>

        <div class="well well-large well-white">


            <asp:Panel ID="Panel_List" runat="server">
                <script>

                    $(function () {
                        if ($('#placementsuccessfulmessage').length > 0) {
                            setTimeout(function () {
                                $('#placementsuccessfulmessage').slideUp('slow');
                            }, 4000);
                        }
                    });

                </script>
                <p>Review the distributors in your waiting room, and strategically place them into
                    your organization.
                </p>
                <% RenderSuccessfulPlacementMessage(); %>

                <table class="table">
                    <tr>
                        <th class="options">&nbsp; </th>
                        <th>ID </th>
                        <th>Name </th>
                        <th>Enroller </th>
                        <th>Sponsor </th>
                        <th>Start Date </th>
                        <th>Placement Opportunity Expires </th>
                    </tr>
                    <% if(WaitingRoomCustomers.Count == 0)
                       { %>
                    <tr>
                        <td colspan="50" style="text-align: center;">Your waiting room is empty. </td>
                    </tr>
                    <% }
                       else
                       { %>
                    <% foreach(var customer in WaitingRoomCustomers)
                       { %>
                    <tr>
                        <td class="options"><a href="<%=Request.Url.AbsolutePath %>?mode=place&id=<%=customer.CustomerID %>"
                            class="ExigoButton Place">Place</a></td>
                        <td><a href="javascript:OpenNewWindow('CustomerDetails.aspx?id=<%=customer.CustomerID %>', 'CustomerDetails_<%=customer.CustomerID %>')"
                            title="View <%=customer.FirstName %>'s details">
                            <%=customer.CustomerID %></a></td>
                        <td>
                            <%=customer.FirstName %>
                            <%=customer.LastName %>
                        </td>
                        <td><a href="javascript:OpenNewWindow('CustomerDetails.aspx?id=<%=customer.EnrollerID %>', 'CustomerDetails_<%=customer.EnrollerID %>')"
                            title="View <%=customer.FirstName %>'s enroller's details">
                            <%=customer.EnrollerID %></a></td>
                        <td><a href="javascript:OpenNewWindow('CustomerDetails.aspx?id=<%=customer.SponsorID %>', 'CustomerDetails_<%=customer.SponsorID %>')"
                            title="View <%=customer.FirstName %>'s sponsor's details">
                            <%=customer.SponsorID %></a></td>
                        <td>
                            <%=customer.CreatedDate.ToString("MMMM d, yyyy") %>
                        </td>
                        <td>
                            <%=GetLastPlacementOpportunityDateDisplay(customer.CreatedDate) %>
                        </td>
                    </tr>
                    <% } %>
                    <% } %>
                </table>

            </asp:Panel>
            <asp:Panel ID="Panel_Place" runat="server" Visible="false">
                <script>

                    $(function () {
                        $('#txtParentID').focus();
                        $('#PlaceNodeButton').bind('click', DisablePlaceNodeButton);
                    });

                    function ValidateParentID() {
                        if ($('#txtParentID').val() != "") {
                            $.ajax({
                                url: '<%=Request.Url.AbsolutePath %>?action=validateparent&id=<%=CustomerIDToBePlaced %>&parent=' + $('#txtParentID').val(),
                                success: function (data) {
                                    var args = data.split('|');
                                    var status = args[0];
                                    var message = args[1];
                                    $('#requestedparentidstatus').html(message);

                                    if (status == "0") $('#PlaceNodeButton').bind('click', DisablePlaceNodeButton);
                                    else $('#PlaceNodeButton').unbind('click', DisablePlaceNodeButton);
                                },
                                error: function (xhr, status, error) {
                                    var args = data.split('|');
                                    var status = args[0];
                                    var message = args[1];
                                    $('#requestedparentidstatus').html(message);

                                    $('#PlaceNodeButton').bind('click', DisablePlaceNodeButton);
                                }
                            });
                        }
                        else {
                            $('#PlaceNodeButton').bind('click', DisablePlaceNodeButton);
                            $('#requestedparentidstatus').html("<span>Required</span>");
                            return false;
                        }
                    }

                    function DisablePlaceNodeButton(e) {
                        e.preventDefault();
                        return false;
                    }


                    function PlaceNode() {
                        if (window.confirm("Are you sure you want to place " + $.trim($('#requestedparentidstatus').text()) + " underneath " + $.trim($('#NewParentDescription').text()) + "?\r\n\nPlease remember that all placements are final.")) {
                            <%=Page.ClientScript.GetPostBackEventReference(this, "PlaceNode") %>
                        }
                    }

                </script>

                <p>Select the sponsor you wish to place this customer under, and click "Place" to confirm
                    your selection.
                </p>
                <hr />
                <div class="row-fluid">
                    <span class="span3">You are placing: </span>
                    <span class="span9"><span id="NewParentDescription"><b><a href="javascript:OpenNewWindow('CustomerDetails.aspx?id=<%=CustomerToBePlaced.CustomerID %>', 'CustomerDetails_<%=CustomerToBePlaced.CustomerID %>')"
                        title="View <%=CustomerToBePlaced.FirstName %>'s details">
                        <%=CustomerToBePlaced.FirstName %>
                        <%=CustomerToBePlaced.LastName %>, ID#
                            <%=CustomerToBePlaced.CustomerID %></a></b></span></span>
                </div>
                <div class="row-fluid">
                    <span class="span3">Underneath ID#: </span>
                    <span class="span9">
                        <asp:TextBox ID="txtParentID" runat="server" onblur="ValidateParentID()" ClientIDMode="Static" />
                        <span id="requestedparentidstatus"></span></span>
                </div>
                <div class="row-fluid">
                    <span class="offset3 span9"><a href="javascript:PlaceNode()" class="ExigoButton Place" id="PlaceNodeButton">Confirm Placement</a>&nbsp;<a href="<%=Request.Url.AbsolutePath %>">Cancel</a>
                    </span>
                </div>
            </asp:Panel>
        </div>
    </div>
</asp:Content>

