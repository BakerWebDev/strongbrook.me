<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="Profile.aspx.cs" Inherits="CustomerProfile" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=10" />
    <title></title>

    <link href="Assets/Styles/site.min.css" rel="stylesheet" />
    <link href="Assets/Styles/themes.min.css" rel="stylesheet" />
    <link href="Assets/Plugins/twitter.bootstrap/css/bootstrap.css" rel="stylesheet" />
    <link href="Assets/Icons/glyphicons/glyphicons.css" rel="stylesheet" />
    <link href="Assets/Styles/profile.min.css" rel="stylesheet" />

    <script src="Assets/Scripts/jquery.min.js"></script>
    <script src="Assets/Plugins/jquery-ui/js/jquery-ui.custom.min.js"></script>
    <script src="Assets/Plugins/twitter.bootstrap/js/bootstrap.js"></script>
    <!--[if lt IE 9]>  
    <script src="http://html5shiv.googlecode.com/svn/trunk/html5.js"></script>  
    <![endif]-->

    <script>
        $(function () {
            $('[title]:not(.btn)').tooltip();

            $("#customerdetailtabs").tabs({
                cache: true,
                ajaxOptions: {
                    error: function (xhr, status, index, anchor) {
                        $(anchor.hash).html(
						"We were unable to load this report at this time.");
                    },
                    success: function (data) {
                    }
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="profile">
            <div id="contentwrapper">

                <div id="pageheader">
                    <h1><%=FullName(Customer) %></h1>
                    <h2>Current Rank: <%=(Volumes != null) ? Volumes.PaidRank.RankDescription : "Unavailable" %></h2>
                    <h3>ID# <%=Customer.CustomerID %></h3>
                </div>

                <div id="content">
                    <div id="customerdetails">
                        <div id="customerdetailtabs">
                            <div class="tabbable">

                                <ul class="nav nav-tabs">
                                    <li class="active"><a href="#tab1" data-toggle="tab">Overview</a></li>
                                </ul>

                                <div class="tab-content">
                                    <div class="tab-pane active" id="tab1">

                                        <h2>Details for <%=FullName(Customer) %>, ID# <%=Customer.CustomerID %></h2>

                                        <table class="table">
                                            <tr>
                                                <td class="fieldlabel">ID
                                                </td>
                                                <td>
                                                    <%=Customer.CustomerID %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Name
                                                </td>
                                                <td>
                                                    <%=Customer.FirstName + " " + Customer.LastName %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Company
                                                </td>
                                                <td>
                                                    <%=Customer.Company %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Current Rank
                                                </td>
                                                <td>
                                                    <%=(Volumes != null) ? Volumes.PaidRank.RankDescription : "Unavailable" %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Highest Rank Achieved
                                                </td>
                                                <td>
                                                    <%=(Volumes != null) ? Volumes.Rank.RankDescription : "Unavailable" %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Distributor since
                                                </td>
                                                <td>
                                                    <%=Customer.CreatedDate.ToString("M/d/yyyy h:mm tt") %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Website
                                                </td>
                                                <td>
                                                    <% if (!string.IsNullOrEmpty(Customer.LoginName))
                                                       { %>
                                                    <a href="<%=string.Format("http://" + Customer.LoginName + ".strongbrook.com") %>"
                                                        target="_blank"
                                                        title="View <%=FullName(Customer) %>'s website in a new window"><%=string.Format(Customer.LoginName + ".strongbrook.com") %></a>
                                                    <% }
                                                       else
                                                       { %>
                                ---
                                <% } %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Enroller
                                                </td>
                                                <td>
                                                    <% if (Enroller.CustomerID > 0)
                                                       { %>
                                                    <a href="javascript:;" onclick="OpenNewWindow('CustomerDetails.aspx?id=<%=Enroller.CustomerID%>', 'CustomerDetails_<%=Enroller.CustomerID%>')"
                                                        title="View <%=FullName(Enroller) %>'s details">
                                                        <%=FullName(Enroller)%>, ID#
                                    <%=Enroller.CustomerID%></a>
                                                    <% }
                                                       else
                                                       { %>
                                ---
                                <% } %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Sponsor
                                                </td>
                                                <td>
                                                    <% if (Sponsor.CustomerID > 0)
                                                       { %>
                                                    <a href="javascript:;" onclick="OpenNewWindow('CustomerDetails.aspx?id=<%=Sponsor.CustomerID%>', 'CustomerDetails_<%=Enroller.CustomerID%>')"
                                                        title="View <%=FullName(Sponsor) %>'s details">
                                                        <%=FullName(Sponsor)%>, ID#
                                    <%=Sponsor.CustomerID%></a>
                                                    <% }
                                                       else
                                                       { %>
                                ---
                                <% } %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Email
                                                </td>
                                                <td>
                                                    <% if (IsPersonallyEnrolled && !string.IsNullOrEmpty(Customer.Email))
                                                       { %>
                                                    <a href="mailto:<%=Customer.Email %>" title="Send <%=FullName(Customer) %> an email">
                                                        <%=Customer.Email %></a>
                                                    <% }
                                                       else
                                                       { %>
                                ---
                                <% } %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Daytime Phone
                                                </td>
                                                <td>
                                                    <%=(IsPersonallyEnrolled) ? Customer.Phone : "---" %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Evening Phone
                                                </td>
                                                <td>
                                                    <%=(IsPersonallyEnrolled) ? Customer.Phone2 : "---" %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Mobile Phone
                                                </td>
                                                <td>
                                                    <%=(IsPersonallyEnrolled) ? Customer.MobilePhone : "---" %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="fieldlabel">Fax
                                                </td>
                                                <td>
                                                    <%=(IsPersonallyEnrolled) ? Customer.Fax : "---"%>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
