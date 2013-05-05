/* UTILITIES */
function raisePostBackEvent(argument) {
    __doPostBack('__Page', argument);
}
function getQueryStringParameter(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)')
                    .exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}
var folderid = getQueryStringParameter('f');




/* MESSAGES LIST */
var messagelist = {};
messagelist.settings = {
    messagelistselector: '.messageslist',
    deletemessagesmodalid: 'deleteMessagesModal',
    currentmessageid: null
};
messagelist.init = function () {
    // If message list isn't found, don't run the rest of this initializer.
    if ($(messagelist.settings.messagelistselector).length == 0) return;

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

        messagelist.updateListOptions();
    });
    
    // Ensure that we have the right options displaying by default.
    messagelist.updateListOptions();

    // Handle the clicking of clickable items in the messages list
    $(messagelist.settings.messagelistselector + ' .clickable').on('click', function (event) {
        var $link = $(event.target);
        var messageid = $link.parents('[data-id]').first().attr('data-id');

        raisePostBackEvent('viewmessage|' + messageid);
    });
};
messagelist.selectAll = function () {
    $(messagelist.settings.messagelistselector + ' .options input:checkbox').each(function () {
        $(this).attr('checked', true).triggerHandler('change');
    });
};
messagelist.deselectAll = function () {
    $(messagelist.settings.messagelistselector + ' .options input:checkbox').each(function () {
        $(this).attr('checked', false).triggerHandler('change');
    });
};
messagelist.selectRead = function () {
    $(messagelist.settings.messagelistselector + ' tr.status-unread .options input:checkbox').each(function () {
        $(this).attr('checked', false).triggerHandler('change');
    });
    $(messagelist.settings.messagelistselector + ' tr.status-read .options input:checkbox').each(function () {
        $(this).attr('checked', true).triggerHandler('change');
    });
};
messagelist.selectUnread = function () {
    $(messagelist.settings.messagelistselector + ' tr.status-unread .options input:checkbox').each(function () {
        $(this).attr('checked', true).triggerHandler('change');
    });
    $(messagelist.settings.messagelistselector + ' tr.status-read .options input:checkbox').each(function () {
        $(this).attr('checked', false).triggerHandler('change');
    });
};
messagelist.updateListOptions = function () {
    var selectedCount = $(messagelist.settings.messagelistselector + ' .options input:checkbox:checked').length;

    if (selectedCount == 0) {
        $('[data-hidewhennoneselected="true"]').hide();
    }
    else {
        $('[data-hidewhennoneselected="true"]').show();
    }

    // Show the empty trash button if we are in the deleted items page.
    if (folderid == 4) {
        $('a[data-showindeleteditems="true"]').show();
    }
};
messagelist.markAllAsRead = function () {
    raisePostBackEvent("markallasread|");
};
messagelist.markSelectedAsRead = function (ids) {
    var list = helper.getIDList(ids);

    // Raise the postback
    raisePostBackEvent("markselectedasread|" + list);
};
messagelist.markSelectedAsUnread = function (ids) {
    var list = helper.getIDList(ids);

    // Raise the postback
    raisePostBackEvent("markselectedasunread|" + list);
};
messagelist.getAllSelectedIDs = function () {
    var selectedIDs = new Array();
    var selectedRows = $(messagelist.settings.messagelistselector).find('input:checkbox:checked').parents('tr[data-id]');

    for (i = 0; i < selectedRows.length; i++) {
        selectedIDs.push(parseInt($(selectedRows[i]).attr('data-id')));
    }

    return selectedIDs;
};
messagelist.getReadSelectedIDs = function () {
    var selectedIDs = new Array();
    var selectedRows = $(messagelist.settings.messagelistselector).find('tr[class*="status-read"] input:checkbox:checked').parents('tr[data-id]');

    for (i = 0; i < selectedRows.length; i++) {
        selectedIDs.push(parseInt($(selectedRows[i]).attr('data-id')));
    }

    return selectedIDs;
};
messagelist.getUnreadSelectedIDs = function () {
    var selectedIDs = new Array();
    var selectedRows = $(messagelist.settings.messagelistselector).find('tr[class*="status-unread"] input:checkbox:checked').parents('tr[data-id]');

    for (i = 0; i < selectedRows.length; i++) {
        selectedIDs.push(parseInt($(selectedRows[i]).attr('data-id')));
    }

    return selectedIDs;
};
messagelist.moveSelectedIDsToDeletedFolder = function (ids) {
    // If we have some specific ID's provided, move them.
    if (ids) {
        raisePostBackEvent("movetodeleted|" + ids);
        return;
    }

    // If we have a current message ID, use that.
    if (messagelist.settings.currentmessageid) {
        raisePostBackEvent("movetodeleted|" + messagelist.settings.currentmessageid);
        return;
    }

    // Delete drafts permanently if we are in the drafts folder.
    if (folderid == 3) {
        raisePostBackEvent("deletedrafts|" + messagelist.getAllSelectedIDs().join(','));
    }

    // Delete emails permanently if we are in the deleted items folder.
    if (folderid == 4) {
        raisePostBackEvent("deleteemails|" + messagelist.getAllSelectedIDs().join(','));
    }
        
    // All other folders, just move the deleted items to the deleted items folder.
    else {
        raisePostBackEvent("movetodeleted|" + messagelist.getAllSelectedIDs().join(','));
    }
};
messagelist.deleteDrafts = function (ids) {
    var list = helper.getIDList(ids);

    // Raise the postback
    raisePostBackEvent("deletedrafts|" + list);
};
messagelist.emptyTrash = function () {
    raisePostBackEvent("emptytrash|");
};
messagelist.moveSelectedIDsToFolder = function (folderID, ids) {
    var list = helper.getIDList(ids);

    // Raise the postback
    raisePostBackEvent("movetofolder|" + folderID + "|" + list);
};
messagelist.moveSelectedIDsToNewFolder = function (ids) {
    var newfoldername = $('#txtNewFolderName').val();
    var list = helper.getIDList(ids);

    // Raise the postback
    raisePostBackEvent("movetonewfolder|" + newfoldername + "|" + list);
};





/* HELPERS */
var helper = {};
helper.getIDList = function (ids) {
    // Assemble the list of IDs to use
    var list;
    if (ids) list = ids; // If a specific list of ID's was provided
    else if (messagelist.settings.currentmessageid) list = messagelist.settings.currentmessageid; // If we are looking at a specific ID
    else list = messagelist.getAllSelectedIDs().join(','); // Otherwise, we are dealing with a selected list from a folder view

    return list;
};





/* FOLDERS */
var folders = {};
folders.settings = {
    createfoldermodalid: 'createfolderModal'
};
folders.init = function () {
    // Create the "Create New Folder" modal
    $('body').append(
        "<div id='" + folders.settings.createfoldermodalid + "' class='modal hide fade' tabindex='-1' role='dialog' aria-labelledby='" + folders.settings.createfoldermodalid + "Label' aria-hidden='true'>" +
            "<div class='modal-header'>" + 
                "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>×</button>" + 
                "<h2 id='" + folders.settings.createfoldermodalid + "Label'>Create New Messages Folder</h2>" +
            "</div>" + 
            "<div class='modal-body'>" + 
                "<p>" + 
                    "<label for='txtNewFolderName'>Folder Name:</label>" + 
                    "<input type='text' id='txtNewFolderName' class='input-xlarge' placeholder='Choose a name for your folder' />" + 
                    "</p>" + 
            "</div>" + 
            "<div class='modal-footer'>" + 
                "<button class='btn' data-dismiss='modal' aria-hidden='true'>Cancel</button>" + 
                "<button class='btn btn-primary' onclick='messagelist.moveSelectedIDsToNewFolder();'>Create</button>" +
            "</div>" + 
        "</div>"
    );
};
folders.openCreateModal = function () {
    $("#" + folders.settings.createfoldermodalid).modal();
};





/* MESSAGE */
var message = {};
message.settings = {
    deletemessagemodalid: 'deleteMessageModal',
    choosemessagerecipientmodalid: 'chooseMessageRecipientsModal'
};
message.init = function () {
    // Create the "Delete Message" modal
    $('body').append(
        "<div id='" + message.settings.deletemessagemodalid + "' class='modal hide fade' tabindex='-1' role='dialog' aria-labelledby='" + message.settings.deletemessagemodalid + "Label' aria-hidden='true'>" +
            "<div class='modal-header'>" +
                "<button type='button' class='close' data-dismiss='modal' aria-hidden='true'>×</button>" +
                "<h2 id='" + message.settings.deletemessagemodalid + "Label'>Delete Message</h2>" +
            "</div>" +
            "<div class='modal-body'>" +
                "<p>Are you sure you want to delete this message?</p>" +
            "</div>" +
            "<div class='modal-footer'>" +
                "<button class='btn' data-dismiss='modal' aria-hidden='true'>Cancel</button>" +
                "<button class='btn btn-primary'>Delete</button>" +
            "</div>" +
        "</div>"
    );
};
message.openDeleteMessageModal = function () {
    $("#" + message.settings.deletemessagemodalid).modal();
};
message.chooseMessageRecipients = function () {
    $("#" + message.settings.choosemessagerecipientmodalid).modal();
};