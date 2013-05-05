<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="OrganizationExplorerTemp.aspx.cs" Inherits="OrganizationExplorerTemp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/reports.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/jquery.min.js" type="text/javascript"></script>
    <script src="Assets/Plugins/jquery.jstree/jquery.jstree.js" type="text/javascript"></script>
    <script type="text/javascript">

        var LoadingImageURL = '<center><img src="Assets/Images/imgLoading.gif" alt="Loading..." title=Loading"" /></center>';

        function LoadAreas(id) {
            LoadArea('SummaryWrapper', 'Summary.aspx', id);
            LoadArea('RecentOrdersWrapper', 'RecentOrders.aspx', id);
            //LoadArea('AutoshipsWrapper', 'Autoships.aspx', id);
            //LoadArea('BusinessSnapshotWrapper', 'BusinessSnapshot.aspx', id);
        }

        function LoadArea(wrapperID, dataUrl, id) {
            $('DIV[id="' + wrapperID + '"]').html(LoadingImageURL);
            $.ajax({
                url: 'DataStores/DownlineDetailReport/' + dataUrl + '?id=' + id,
                cache: false,
                success: function (data) {
                    $('DIV[id="' + wrapperID + '"]').html(data);
                },
                error: function (data, status, error) {
                    alert('error: ' + error);
                },
                dataType: "html"
            });
        }



        $(document).ready(function () {
            $('DIV#NavigationTree').jstree({
                "core": { "initially_open": ["root"] },
                "json_data": {
                    "progressive_render": true,
                    "ajax": {
                        "url": "DataStores/DownlineDetailReport/NavigationTree.aspx?enrollerid=<%=CustomerID %>&sponsorid=<%=CustomerID %>&level=0"
                    }
                },
                "ui": {
                    "select_limit": 1,
                    "selected_parent_close": "select_parent"
                },
                "types": {
                    "use_data": true,
                    "type_attr": "rank",
                    "types": {
                        "default": {
                            "hover_node": true
                        },
                        "root": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank0": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank1": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank2": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank3": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank4": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank5": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank6": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank7": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank8": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank9": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank10": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank11": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank12": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank13": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } },
                        "rank14": { "icon": { "image": "Assets/Images/Icons/16x16/user-white.png" } }
                    }
                },
                "plugins": ["themes", "json_data", "ui", "types", "crrm"]
            }).bind('loaded.jstree', function (e, data) {
                LoadAreas($('DIV#NavigationTree UL LI:first').attr('customerid'));
                $('DIV#NavigationTree').jstree('select_node', $('DIV#NavigationTree UL LI:first'));
            }).bind("select_node.jstree", function (e, data) {
                var enrollerid = data.rslt.obj.attr("customerid");
                var sponsorid = data.rslt.obj.attr("customerid");
                var level = 1;

                if ($('DIV#NavigationTree').jstree('is_leaf', data.rslt.obj)) {
                    $.ajax({
                        url: "DataStores/DownlineDetailReport/NavigationTree.aspx?enrollerid=" + enrollerid + "&sponsorid=" + sponsorid + "&level=" + level,
                        cache: true,
                        dataType: "json",
                        success: function (result) {
                            var newNodeCounter = 0;
                            for (i = 0; i < result.length; i++) {
                                if (result[i].attr.customerid != enrollerid) {
                                    $('DIV#NavigationTree').jstree('create_node', null, "inside", { "data": result[i].data, "attr": { "rank": result[i].attr.rank, "title": result[i].attr.title, "customerid": result[i].attr.customerid } }, null, true);
                                    newNodeCounter++;
                                }
                            }
                            if (newNodeCounter > 0) {
                                $('DIV#NavigationTree').jstree('open_node', data.rslt.obj);
                            }
                        },
                        error: function (error) {
                            alert('error: ' + error);
                        }
                    });
                }

                LoadAreas(data.rslt.obj.attr('customerid'));
            });

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

<%--

        <div class="btn-toolbar">
            <a href="javascript:;" class="btn"><i class="icon-plus"></i>&nbsp;Enroll new</a>
        </div>
--%>

        <div class="well well-white">


            <div id="organizationdetailreport">


                <div class="row-fluid">
                    <span class="span7">
                        <h2>My Organization</h2>
                        <div id="downlinelistwrapper" class="well well-small">
                            <table id="downlinelist" style="width: 180px; margin-top: 0;">
                                <tbody>
                                </tbody>
                            </table>
                            <div id="NavigationTree">
                            </div>
                        </div>
                    </span>
                    <span class="span3">
                        <div id="Profile" class="row-fluid">
                            <span class="span11">
                                <h2>Profile</h2>
                                <div id="summarywrapper" class="well">
                                    <table cellpadding="0" cellspacing="0" style="WIDTH: 35%; margin:auto;">
                                        <tr>
                                            <td valign="top">
                                                <div class="FeatureWrapper">
                                                    <div id="SummaryWrapper">
                                                    </div>
                                                </div>

<%--
                                                <div class="ColumnWrapper Left w50">
                                                    <div class="ColumnContent Left">
                                                        <div class="FeatureWrapper DownlineDetail BusinessSnapshot">
                                                            <h2>Business Snapshot</h2>
                                                            <div class="FeatureContent Padded" id="BusinessSnapshotWrapper">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
--%>

                                                <div class="ColumnWrapper Right w50">
                                                    <div class="ColumnContent Right">

<%--
                                                        <div class="FeatureWrapper DownlineDetail Websites">
                                                            <h2>
                                                                Autoships</h2>
                                                            <div class="FeatureContent Padded" id="AutoshipsWrapper">
                                                            </div>
                                                        </div>
                                                        

                                                        <hr />

                                                        <div class="FeatureWrapper DownlineDetail RecentOrders">
                                                            <h2>Recent Orders</h2>
                                                            <div class="FeatureContent Padded" id="RecentOrdersWrapper">
                                                            </div>
                                                        </div>
--%>

                                                    </div>
                                                </div>
                                                <div class="ClearAllFloats">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Label ID="lblApplicationSettingsXML" runat="server" />
                                </div>
                            </span>
                        </div>
                    </span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

