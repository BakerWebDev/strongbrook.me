<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.master" AutoEventWireup="true" CodeFile="CreateMessage.aspx.cs" Inherits="CreateMessage" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="Server">
    <link href="Assets/Styles/messages.min.css" rel="stylesheet" />
    <script src="Assets/Scripts/exigo.messages.js"></script>
    <script src="Assets/Plugins/tinymce/jscripts/tiny_mce/jquery.tinymce.js"></script>

    <script>
        // Set page variables
        var page = {
            activenavigation: 'messages'
        };

        function initializeWysiwyg() {
            $('textarea.tinymce').tinymce({
                // Location of TinyMCE script
                script_url: 'Assets/Plugins/tinymce/jscripts/tiny_mce/tiny_mce.js',

                // General options
                mode: "textareas",
                theme: "advanced",
                plugins: "table,inlinepopups",

                // Theme options
                theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,|,table,removeformat",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",
                theme_advanced_statusbar_location: "none",
                theme_advanced_resizing: true,
                width: '100%',
                height: '450px',

                // Example content CSS (should be your site CSS)
                content_css: "Assets/Plugins/tinymce/jscripts/tiny_mce/themes/advanced/skins/default/custom.css"
            });
        }

        $(function () {
            // Handles highlighting of a row upon selection
            $('td.options input:checkbox').on('change', function (event) {
                var $checkbox = $(event.target);
                var ischecked = $checkbox.is(':checked');

                if (ischecked) {
                    $checkbox.parents('tr').first().addClass('status-highlighted');
                }
                else {
                    $checkbox.parents('tr').first().removeClass('status-highlighted');
                }
            });


            // Handle events triggered by the attachment file uploads
            $('input:file[data-fileuploadindex]').hide();
            $('input:file[data-fileuploadindex]').on('change', function (event) {
                var index = parseInt($(event.target).attr('data-fileuploadindex'));
                var maxindexwithvalue = 0;
                var maxattachmentcount = $('input:file[data-fileuploadindex]').length;

                for (x = 0; x < maxattachmentcount; x++) {
                    if ($('input:file[data-fileuploadindex=' + x + ']').val() != '') {
                        maxindexwithvalue = x + 1;
                    }
                }

                for (x = 0; x <= maxindexwithvalue; x++) {
                    $('input:file[data-fileuploadindex=' + x + ']').show();
                }
            }).filter('[data-fileuploadindex=0]').triggerHandler('change');


            // Add the WYSIWYG to our text areas
            initializeWysiwyg();
            folders.init();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <h1>Messages</h1>


    <div class="sidebar">
        <a href="CreateMessage.aspx" class="btn btn-block"><i class="icon-edit"></i>&nbsp;Compose</a>
        <br />
        <% RenderEmailFolders(); %>
    </div>
    <div class="maincontent">
        <div class="btn-toolbar">
            <asp:LinkButton ID="cmdSendMessage" Text="<i class='icon-envelope'></i>&nbsp;Send</a>" CssClass="btn" OnClick="SendMessage_Click" runat="server" />

            <a href="javascript:;" class="btn">Save</a>

            <a href="Messages.aspx" class="btn">Discard</a>

            <div class="clearfix"></div>
        </div>

        <div class="createmessage">
            <div class="row-fluid">
                <span class="span2"><label for="lstFrom">From</label></span>
                <span class="span10">
                    <asp:DropDownList ID="lstFrom" CssClass="span12" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span2"><label for="txtTo">To:</label></span>
                <span class="span10">
                    <asp:TextBox ID="txtTo" CssClass="span12" Rows="3" TextMode="MultiLine" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span2"><label for="txtSubject">Subject</label></span>
                <span class="span10">
                    <asp:TextBox ID="txtSubject" CssClass="span12" runat="server" ClientIDMode="Static" />
                </span>
            </div>
            <div class="row-fluid">
                <span class="span2">&nbsp;</span>
                <span class="span10 attachments">
                    <asp:FileUpload ID="uploadAttachment1" runat="server" ClientIDMode="Static" data-fileuploadindex="0" />
                    <asp:FileUpload ID="uploadAttachment2" runat="server" ClientIDMode="Static" data-fileuploadindex="1" />
                    <asp:FileUpload ID="uploadAttachment3" runat="server" ClientIDMode="Static" data-fileuploadindex="2" />
                    <asp:FileUpload ID="uploadAttachment4" runat="server" ClientIDMode="Static" data-fileuploadindex="3" />
                    <asp:FileUpload ID="uploadAttachment5" runat="server" ClientIDMode="Static" data-fileuploadindex="4" />
                    <br />
                    <% RenderForwardedAttachments(); %>
                    <br />
                </span>
            </div>
            
            <div class="clearfix"></div>
            <asp:TextBox ID="txtMessage" TextMode="MultiLine" CssClass="tinymce span12" runat="server" ClientIDMode="Static" />
        </div>
    </div>








    <div id="chooseMessageRecipientsModal" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="chooseMessageRecipientsModalLabel" aria-hidden="true">
        <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
            <h2 id="chooseMessageRecipientsModalLabel">Select Recipient(s)</h2>
        </div>
        <div class="modal-body">
            <ul class="nav nav-tabs nav-seamless" id="recipientTabs">
                <li class="active"><a href="#team" data-toggle="tab">My Team</a></li>
                <li><a href="#groups" data-toggle="tab">My Groups</a></li>
            </ul>

            <div class="tab-content">
                <div class="tab-pane active" id="team">
                    <div class="recipientlist">
                        <table class="table table-condensed">
                            <tr>
                                <th class="options"></th>
                                <th class="description">Name</th>
                                <th class="id">ID</th>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(1) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="2">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(2) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">2</td>
                            </tr>
                            <tr data-id="3">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(3) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">3</td>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(1) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="2">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(2) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">2</td>
                            </tr>
                            <tr data-id="3">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(3) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">3</td>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(1) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="2">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(2) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">2</td>
                            </tr>
                            <tr data-id="3">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(3) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">3</td>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(1) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="2">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(2) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">2</td>
                            </tr>
                            <tr data-id="3">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description">
                                    <img src="<%=GlobalUtilities.GetCustomerTinyAvatarUrl(3) %>" class="avatar" />
                                    <span class="name"><%=Greeking.FullName%> &lt;<%=Greeking.Email%>&gt;</span>
                                    <div class="clearfix"></div>
                                </td>
                                <td class="id">3</td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="tab-pane" id="groups">
                    <div class="recipientlist">
                        <table class="table table-condensed">
                            <tr>
                                <th class="options"></th>
                                <th class="description">Group</th>
                                <th class="id">Count</th>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description"><span class="name"><%=Greeking.FullName%></span></td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description"><span class="name"><%=Greeking.FullName%></span></td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description"><span class="name"><%=Greeking.FullName%></span></td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description"><span class="name"><%=Greeking.FullName%></span></td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description"><span class="name"><%=Greeking.FullName%></span></td>
                                <td class="id">1</td>
                            </tr>
                            <tr data-id="1">
                                <td class="options">
                                    <input type="checkbox" /></td>
                                <td class="description"><span class="name"><%=Greeking.FullName%></span></td>
                                <td class="id">1</td>
                            </tr>
                        </table>
                    </div>

                </div>
            </div>

            <div class="clearfix"></div>

            <div class="selectedrecipients">
                <label for="txtSelectedRecipients">Selected recipients:</label>
                <input type="text" id="txtSelectedRecipients" placeholder="Choose your recipients" />
            </div>

            <script>
                // Initialize the tabs
                $('#recipientTabs a').click(function (e) {
                    e.preventDefault();
                    $(this).tab('show');
                });
            </script>
        </div>
        <div class="modal-footer">
            <button class="btn" data-dismiss="modal" aria-hidden="true">Done</button>
        </div>
    </div>
</asp:Content>

