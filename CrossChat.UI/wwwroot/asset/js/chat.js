// variables
var currentPage;
var currentMessages = [];
var messageList = [];
var memberList = [];
var chatMessageListIndex = 0;

//signalr connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

/// document Ready
$(document).ready(function () {
    // set avatar click event inside select user avatar modal
    $("img[name=avatar]").on('click', function (e) {
        selectAvatarFromModal(e,false);
    });
    // set avatar click event inside select channel avatar modal
    $("img[name=avatarChannel]").on('click', function (e) {
        selectAvatarFromModal(e,true);
    });
    // signalr restart when sesstion is finished. For that always connect
    connection.onclose(async () => {
        await start();
    });
     // live recive messages from other users
    connection.on("ReceiveMessage", (message) => {
        reciveMessageAddToChatPage(message);
    });
    // live result of send message
    connection.on("ResultOfSendMessage", (message) => {
        selfMessageAddToChatPage(message);
    });
    // live result of send message
    connection.on("DeleteChannel", (message) => {
        $('li[id="' + message + '"]').remove();
        debugger;
        if ($('#channelId').val() == message) {
          
            $('#chatSection').attr("style", "display:none!important;");
        }

    });

    // start signalr connection
    start();

    // send button click trigger
    $(document).on("keypress", "#messageInput", function (e) {
        if (e.keyCode == 13 || e.which == '13') {
            sendMessage();
        }
    });
    // fill user active chats and channels
    syncChatList();
    $('#userId').val('');
});
// live recive messages from other users
function reciveMessageAddToChatPage(message) {
    if (message.destinationUserId == null) {
       
        if (message.channelId == $('#channelId').val()) {


            if ($('#chatListUl li[name="chat"]').length>0 && $('#chatListUl li[name="chat"]').last()[0].getAttribute("class") == null) {
                var dt = new Date(message.dateCreated);
                var html = "";
                if (message.messageType.code == 1) {
                    html += `<div class="ctext-wrap"  id='` + message.id + `'>
                                    <div class="ctext-wrap-content" style="width: 100%;text-align: left;">
                                        <p class="mb-0">
                                            `+ message.messageBody + `
                                        </p>
                                        <p class="chat-time mb-0"><i class="ri-time-line align-middle"></i> <span class="align-middle">`+ convertShortDate(dt) + `</span></p>
                                    </div>
                              </div>
                                `;
                }
                else {
                    html += `<div class="ctext-wrap"  id='` + message.id + `'>
                               <div class="ctext-wrap-content">
                                    <ul class="list-inline message-img  mb-0">
                                       <li class="list-inline-item message-img-list">
                                            <div>
                                                <a class="popup-img d-inline-block m-1" href="/messageimages/`+ message.messageBody + `" target="_blank" title="Project 1">
                                                    <img src="/messageimages/`+ message.messageBody + `" alt="" class="rounded border">
                                                </a>
                                            </div>
                                            <p class="chat-time mb-0"><i class="ri-time-line align-middle"></i> <span class="align-middle">`+ convertShortDate(dt) + `</span></p>      
                                        </li>
                                    </ul>
                                </div>
                            </div>`;
                }
                $('#chatListUl li[name="chat"]').last()[0].getElementsByClassName("user-chat-content")[0].innerHTML = $('#chatListUl li[name="chat"]').last()[0].getElementsByClassName("user-chat-content")[0].innerHTML + html;
                var a = $('#chatListUl li').last().find('.user-chat-content div[class="ctext-wrap"]').last();
                var b = $('#chatListUl li').last().find('.user-chat-content div[class="conversation-name"]').last();
                b.insertAfter(a)

            }
            else {
                var selfMessage = [];
                selfMessage.push(message);
                var html = getChatMessageHtml(selfMessage, false);
                $('#chatListUl').html($('#chatListUl').html() + html);
            }
            playSound(true);


        }
        else if ($('#recentList li[id="' + message.channelId + '"]').length > 0) {
            var currentCount = $('#recentList').find('#' + message.channelId).find('span')[1].innerText;
            if (currentCount != "") {
                var oneCount = parseInt(currentCount) + 1;

                $('#recentList').find('#' + message.channelId).find('span')[1].innerText = oneCount;
            }
            else {
                $('#recentList').find('#' + message.channelId).find('span')[1].innerText = 1;
            }

            playSound(false);
        }
        else {
            syncChatList();
            var chatListItem = $('#recentList').find('#' + message.channelId);
            if (chatListItem != null) {
                chatListItem.find('span')[1].innerText = 1;
            }
            playSound(false);
        }



    }
    else {
        if (message.sourceUserId == $('#userId').val()) {
            // userPageIsOpen
            if ($('#chatListUl li[name="chat"]').last()[0].getAttribute("class") !=null && !$('#chatListUl li[name="chat"]').last()[0].getAttribute("class").includes('right')) {
                var dt = new Date(message.dateCreated);
                var html = "";
                if (message.messageType.code == 1) {
                    html += `<div class="ctext-wrap"  id='` + message.id + `'>
                                    <div class="ctext-wrap-content" style="width: 100%;text-align: left;">
                                        <p class="mb-0">
                                            `+ message.messageBody + `
                                        </p>
                                        <p class="chat-time mb-0"><i class="ri-time-line align-middle"></i> <span class="align-middle">`+ convertShortDate(dt) + `</span></p>
                                    </div>
                              </div>
                                `;
                }
                else {
                    html += `<div class="ctext-wrap"  id='` + message.id + `'>
                               <div class="ctext-wrap-content">
                                    <ul class="list-inline message-img  mb-0">
                                       <li class="list-inline-item message-img-list">
                                            <div>
                                                <a class="popup-img d-inline-block m-1" href="/messageimages/`+ message.messageBody + `" target="_blank" title="Project 1">
                                                    <img src="/messageimages/`+ message.messageBody + `" alt="" class="rounded border">
                                                </a>
                                            </div>
                                            <p class="chat-time mb-0"><i class="ri-time-line align-middle"></i> <span class="align-middle">`+ convertShortDate(dt) + `</span></p>      
                                        </li>
                                    </ul>
                                </div>
                            </div>`;
                }
                $('#chatListUl li[name="chat"]').last()[0].getElementsByClassName("user-chat-content")[0].innerHTML = $('#chatListUl li[name="chat"]').last()[0].getElementsByClassName("user-chat-content")[0].innerHTML + html;
                var a = $('#chatListUl li[name="chat"]').last().find('.user-chat-content div[class="ctext-wrap"]').last();
                var b = $('#chatListUl li[name="chat"]').last().find('.user-chat-content div[class="conversation-name"]').last();
                b.insertAfter(a)

            }
            else {
                var selfMessage = [];
                selfMessage.push(message);
                var html = getChatMessageHtml(selfMessage, false);
                $('#chatListUl').html($('#chatListUl').html() + html);
            }
            playSound(true);
        }
        else if ($('#recentList li[id="' + message.sourceUserId + '"]').length > 0) {
            var currentCount = $('#recentList').find('#' + message.sourceUserId).find('span')[1].innerText;
            if (currentCount != "") {
                var oneCount = parseInt(currentCount) + 1;

                $('#recentList').find('#' + message.sourceUserId).find('span')[1].innerText = oneCount;
            }
            else {
                $('#recentList').find('#' + message.sourceUserId).find('span')[1].innerText = 1;
            }

            playSound(false);
        }
        else {
            syncChatList();
            var chatListItem = $('#recentList').find('#' + message.sourceUserId);
            if (chatListItem != null) {
                chatListItem.find('span')[1].innerText = 1;
            }
            playSound(false);
        }

    }
    //transform to top of left list
    if (message.destinationUserId == null) {
        var lis = $('#recentList li')
        var resivedli = $('#recentList li[id="' + message.channelId + '"]')
        if (lis.length > 0 && resivedli.length > 0 && lis[0].getAttribute("id") != resivedli[0].getAttribute("id"))
            resivedli.first().insertBefore(lis.first());
    }
    else {
        var lis = $('#recentList li')
        var resivedli = $('#recentList li[id="' + message.sourceUserId + '"]')
        if (lis.length > 0 && resivedli.length > 0 && lis[0].getAttribute("id") != resivedli[0].getAttribute("id"))
            resivedli.first().insertBefore(lis.first());
    }
}
// live result of send message
function selfMessageAddToChatPage(message) {
    debugger;
    if ($('#chatListUl li[name="chat"]').length > 0 && $('#chatListUl li[name="chat"]').last()[0].getAttribute("class") != null && $('#chatListUl li[name="chat"]').last()[0].getAttribute("class").includes('right')) {
        var dt = new Date(message.dateCreated);
        var html = "";
        if (message.messageType.code == 1) {
            html += `<div class="ctext-wrap">
                                    <div class="ctext-wrap-content" style="width: 100%;text-align: left;">
                                        <p class="mb-0">
                                            `+ message.messageBody + `
                                        </p>
                                        <p class="chat-time mb-0"><i class="ri-time-line align-middle"></i> <span class="align-middle">`+ convertShortDate(dt) + `</span></p>
                                    </div>
                              </div>
                                `;
        }
        else {
            html += `<div class="ctext-wrap">
                               <div class="ctext-wrap-content">
                                    <ul class="list-inline message-img  mb-0">
                                       <li class="list-inline-item message-img-list">
                                            <div>
                                                <a class="popup-img d-inline-block m-1" href="/messageimages/`+ message.messageBody + `" target="_blank" title="Project 1">
                                                    <img src="/messageimages/`+ message.messageBody + `" alt="" class="rounded border">
                                                </a>
                                            </div>
                                            <p class="chat-time mb-0"><i class="ri-time-line align-middle"></i> <span class="align-middle">`+ convertShortDate(dt) + `</span></p>      
                                        </li>
                                    </ul>
                                </div>
                            </div>`;
        }
        $('#chatListUl li[name="chat"]').last()[0].getElementsByClassName("user-chat-content")[0].innerHTML = $('#chatListUl li[name="chat"]').last()[0].getElementsByClassName("user-chat-content")[0].innerHTML + html;
        var a = $('#chatListUl li[name="chat"]').last().find('.user-chat-content div[class="ctext-wrap"]').last();
        var b = $('#chatListUl li[name="chat"]').last().find('.user-chat-content div[class="conversation-name"]').last();
        b.insertAfter(a)


    }
    else {
        var selfMessage = [];
        selfMessage.push(message);
        var html = getChatMessageHtml(selfMessage, true);
        $('#chatListUl').html($('#chatListUl').html() + html);
    }
    if ($('#recentList li[id="' + message.sourceUserId + '"]').length == 0) {
        syncChatList();
    }
}

// start signalr connection
async function start() {
    try {
        await connection.start();
        console.log("connected");
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};
// when user avatar clicked inside modal
function selectAvatarFromModal(e,isChannel) {
    if (isChannel == true) {
        $('#selectedAvatarChannel').attr("src", e.target.attributes['src'].value);
        $('#avatarChannel').val(e.target.attributes['src'].value);
    }
    else {
        $('#selectedAvatarEditProfile').attr("src", e.target.attributes['src'].value);
        $('#avatarEditProfile').val(e.target.attributes['src'].value);
    }
    $('#avatarModal').modal('toggle');
}
// when channel avatar clicked inside modal
function selectAvatar(isChannel) {
    if (isChannel != undefined && isChannel != null && isChannel == true) {
        $('#channelAvatar').show();
        $('#userAvatar').hide();
    }
    else {
        $('#channelAvatar').hide();
        $('#userAvatar').show();
    }
    
    $('#avatarModal').modal('toggle');
}
// open edit profile modal
function loadProfileModal() {
    var result = ajaxRequest('post', '/Account/GetCurrentUser', null);
    if (result.success) {

        $('#emailEditProfile').val(result.data.email);
        $('#nameEditProfile').val(result.data.name);
        $('#surnameEditProfile').val(result.data.surname);
        $('#selectedAvatarEditProfile').attr("src", result.data.avatar);
        $('#avatarEditProfile').val(result.data.avatar);

        $('#editUserModal').modal('toggle');
    }
    else {
        ShowMessage(result.message, "Error", false);
    }

}
// request for edit profile
function editProfile() {
    var request = {
        Name: $('#nameEditProfile').val(),
        Surname: $('#surnameEditProfile').val(),
        Avatar: $('#avatarEditProfile').val(),
        Email: $('#emailEditProfile').val(),
    }
    var result = ajaxRequest('post', '/Account/EditProfile', request);
    if (result.success) {

        $('#userNameBox').text(result.data.name + ' ' + result.data.surname);
        $('#currentUserAvatarImage').attr("src",result.data.avatar);
        ShowMessage("Your prodile successfully update", "Success", true);
        $('#editUserModal').modal('toggle');
    }
    else {
        ShowMessage(result.message, "Error", false);
    }
    
}
// request for change password
function changePassword() {
    
    var request = {
        oldPassword: $('#oldPasswordEditProfile').val(),
        newPassword: $('#newPasswordEditProfile').val(),
    }
    if (request.newPassword != $('#confirmNewPasswordEditProfile').val()) {
        ShowMessage("New Password not matched!", "Error", false);
        return;
    }
    if (request.oldPassword.length < 3) {
        ShowMessage("Old Password is not have correct format!", "Error", false);
        return;
    }
    var result = ajaxRequest('post', '/Account/ChangePassword', request);
    if (result.success) {
        ShowMessage("Your Password successfully update", "Success", true);
        $('#editUserModal').modal('toggle');
    }
    else {
        ShowMessage(result.message, "Error", false);
    }
}
// open new channel modal
function addChannelModal() {
    $('#addChannelModal').modal('toggle');
}
// request for add channel
function AddNewChannel() {
    debugger;
    var request = {
        ChannelName: $('#channelName').val(),
        Description: $('#descriptionChannel').val(),
        Avatar: $('#avatarChannel').val(),
        ///IsPrivate: $('#isPrivateChannel').val(),
        IsReadOnly: $("#isReadonlyChannel:checked").length === 1,
    }
    var result = ajaxRequest('post', '/Home/AddNewChannel', request);
    if (result.success) {

        syncChatList(result.data.Id);
        ShowMessage("Your prodile successfully update", "Success", true);
        $('#addChannelModal').modal('toggle');
    }
    else {
        ShowMessage(result.message, "Error", false);
    }
}
// left nav menu item select
function showOnlyChats() {
    syncChatList(null, 'chat');
}
function showOnlyChannels() {
    syncChatList(null, 'channel');
}
function showChannelsAndMessages() {
    syncChatList(null, '');
}

// request fill user active chats and channels
function syncChatList(id,order) {
    var result = ajaxRequest('post', '/Home/GetUserAllChannelsAndMessages', null);
    if (result.success) {
        messageList = [];
        for (var i = 0; i < result.data.length;i++) {
            var chat = {
                title: result.data[i].title,
                subTitle: result.data[i].subTitle,
                avatar: result.data[i].avatar,
                isChannel: result.data[i].isChannel,
                lastMofified: result.data[i].lastMofified,
                id: result.data[i].id
            }
            if (order == 'chat') {
                if (!chat.isChannel) {
                    messageList.push(chat);
                }
            }
            else if (order == 'channel') {
                if (chat.isChannel) {
                    messageList.push(chat);
                }
            }
            else {
                messageList.push(chat)
            }
            
        }
        //show recive date in the left side column
        UpdateMessageListView(id);
    }
    else {
        ShowMessage(result.message, "Error", false);
    }
}
 //show recive date in the left side column (generate html and bind to del left column)
function UpdateMessageListView(id) {
    messageList.sort(compareMessage);
    var recentHtml="";
    for (var i = 0; i < messageList.length; i++) {
        recentHtml += getResentItem(messageList[i]);
    }
    $('#recentList').html(recentHtml);
    if (id != null && id != undefined)
        $('#' + id).trigger("click");

}
//generate a item html of left column 
function getResentItem(item) {
    var onclick = `viewChat('` + item.id + `')`;
    if (item.isChannel) {
        onclick = `viewChat('` + item.id + `',true)`;
    }
    return `<li id="` + item.id + `" onclick="` + onclick +`">
                    <a href="#">
                        <div class="media">
                            <div class="chat-user-img online align-self-center mr-3">
                                <img src="`+ item.avatar + `" class="rounded-circle avatar-xs" alt="">
                                <span class="user-status" style="background-color:#979e9c;"></span>
                            </div>
                            <div class="media-body overflow-hidden">
                                <h5 class="text-truncate font-size-15 mb-1">`+ item.title + `</h5>
                                <p class="chat-user-message text-truncate mb-0">`+ item.subTitle + `</p>
                            </div>
                            <div class="font-size-11">`+ item.lastMofified.substring(0, 10) + `</div>
                            <div class="unread-message" atyle="display:none" id="newMessageCount">
                                <span class="badge badge-soft-danger badge-pill"></span>
                            </div>
                        </div>
                    </a>
                </li>`;
}
//generate a item html of search bar 
function getSearchItem(item) {
    if (item.isChannel) {
        return `<div class="owl-item" style="width: 71px; margin-right: 16px;"  style="margin-right:3px;">
                    <div class="item" onclick="viewChat('` + item.id + `',true);">
                        <a href="#" class="user-status-box">
                            <div class="avatar-xs mx-auto d-block chat-user-img online">
                                <img src="`+ item.avatar + `" alt="user-img" class="img-fluid rounded-circle">
                                <span class="user-status"></span>
                            </div>

                            <h5 class="font-size-13 text-truncate mt-3 mb-1">"`+ item.title + `"</h5>
                        </a>
                    </div>
                </div>`;
    }
    else {
        return `<div class="owl-item" style="width: 71px; margin-right: 16px;"  style="margin-right:3px;">
                    <div class="item" onclick="viewChat('` + item.id + `',false);">
                        <a href="#" class="user-status-box">
                            <div class="avatar-xs mx-auto d-block chat-user-img online">
                                <img src="`+ item.avatar + `" alt="user-img" class="img-fluid rounded-circle">
                                <span class="user-status"></span>
                            </div>

                            <h5 class="font-size-13 text-truncate mt-3 mb-1">"`+ item.title + `"</h5>
                        </a>
                    </div>
                </div>`;
    }
}
// compare for sort array of object
function compareMessage(a, b) {
    if (a.lastMofified > b.lastMofified) {
        return -1;
    }
    if (a.lastMofified < b.lastMofified) {
        return 1;
    }
    return 0;
}
// request search of the user or channels
function searchChannelsOrUsers(e) {
    if (e.value.length == 0) {
        $('#searchDiv').attr('style','display:none;');
    }
    else {
        var request = { key: e.value}
        var result = ajaxRequest('post', '/Home/SearchChannelsOrUsers', request);
        if (result.success) {
            if (result.data.length == 0) {
                $('#searchDiv').attr('style', 'display:none;');
                return;
            }
            var html = "";
            $('.owl-stage').html('');
            for (var i = 0; i < result.data.length; i++) {
                var chat = {
                    title: result.data[i].title,
                    avatar: result.data[i].avatar,
                    isChannel: result.data[i].isChannel,
                    id: result.data[i].id
                }
                html += getSearchItem(chat);
                
                
            }
            $('.owl-stage').html(``);
            $('.owl-stage').html(html);

            var $owl = $('.owl-carousel');
            $owl.trigger('destroy.owl.carousel');
            $owl.owlCarousel({ items: 3, loop: !1, margin: 16, nav: !1, dots: !1 });
        }
        else {
            ShowMessage(result.message, "Error", false);
        }
        $('#searchDiv').removeAttr('style');
    }
}
// requst for user join to channel
function subScribe(channelId) {
   
    
    var request = { channelId: channelId}
    var result = ajaxRequest('post', '/Home/SubscribeToChannel', request);
        if (result.success) {
            showChannel(channelId);
            debugger;
            syncChatList(channelId, '');
        }
        else {
            ShowMessage(result.message, "Error", false);
        }

    
}
// Show realted the chat box when click the left column item
function viewChat(id, isChannel) {
   
    currentMessages = [];
    if (isChannel != null && isChannel) {
        showChannel(id);
        
    }
    else {
        showChat(id)
    }
    $('#chatSection').show();
}
// Request of User chat history
function showChat(id, newPage) {
    $('#membershipList').hide();
    if (newPage == null || newPage == false || newPage == undefined) {
        currentPage = 1;
    }
    else {
        currentPage++;
    }
    if ($('li[id=' + id + ']').find('span')[1] != undefined)
        $('li[id=' + id + ']').find('span')[1].innerText = "";
    var request = { userId: id, page: currentPage,pageSize:30 }
    var result = ajaxRequest('post', '/Home/getChatData', request);
    if (result.success) {
        if (result.data.user.id != $('#userId').val()) {
            var dt = new Date(result.data.user.dateCreated);
            $('#chatPageAvatarImage').attr("src", result.data.user.avatar);
            $('#chatPageSideAvatar').attr("src", result.data.user.avatar);
            $('#chatPageTitle').text(result.data.user.name + ' ' + result.data.user.surname);
            $('#chatPageSideTitle').text(result.data.user.name + ' ' + result.data.user.surname);


            $('#chatPageSideNameTitle').text("Name");
            $('#chatPageSideSurnameTitle').text("Surname");
            $('#chatPageSideEmailTitle').text("Email");
            $('#chatPageSideDateTitle').text("Join Date");
            $('#chatPageSideName').text(result.data.user.name);
            $('#chatPageSideSurname').text(result.data.user.surname);
            $('#chatPageSideEmail').text(result.data.user.email);
            $('#chatPageSideDate').text(convertShortDate(dt));
            $('#userId').val(result.data.user.id);
            $('#channelId').val('');

            $('#newMessageBar').html(`<div class="col">
                                <div>
                                    <input type="text" class="form-control form-control-lg bg-light border-light" placeholder="Enter Message..." id="messageInput">
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="chat-input-links ml-md-2">
                                    <ul class="list-inline mb-0">
                                       <li class="list-inline-item">
                                            <button type="button" class="btn btn-link text-decoration-none font-size-16 btn-lg waves-effect" data-toggle="tooltip" id="sendImageButton" data-placement="top" title="" data-original-title="Attached File" onclick="sendImageButton()">
                                                <i class="ri-attachment-line"></i>
                                            </button>
                                        </li>
                                        <li class="list-inline-item">
                                            <button type="submit" class="btn btn-primary font-size-16 btn-lg chat-send waves-effect waves-light" id="sendMessageButton" onclick="sendMessage()">
                                                <i class="ri-send-plane-2-fill"></i>
                                            </button>
                                        </li>
                                    </ul>
                                </div>

                            </div>`);
        }
        else {

        }


        if (newPage == null || newPage == false || newPage == undefined) {
            messageList = [];
            for (var i = 0; i < result.data.messages.length; i++) {
                messageList.push(result.data.messages[i]);
            }

            showChatMesaage()
        }
        else {
            var newMessagesList = [];
            for (var i = 0; i < result.data.messages.length; i++) {
                newMessagesList.push(result.data.messages[i]);
            }
            messageList = messageList.concat(newMessagesList);
            showChatMesaage(true);
        }

        
        
       
    }
    else {
        console.log(result);
    }
}
// Generate Html for the messages
function getChatMessageHtml(message, isRight) {
    
    var name = $('#userNameBox').text();
    var avatar = $('#currentUserAvatarImage').attr('src');
    var rightClass = "class='right'";
    if (!isRight) {
        name = $('#currentUserAvatarImage').text();
        avatar = $('#chatPageAvatarImage').attr('src');
        rightClass = "";
    }
    if (message[0].sourceUser != null) {
        name = message[0].sourceUser.name + ' ' + message[0].sourceUser.surname;
        avatar = message[0].sourceUser.avatar;
    }
    var messageItem = `<li name="chat" data-item="` + chatMessageListIndex + `" tabindex="` + chatMessageListIndex + `" ` + rightClass +` style="outline:none;">
                        <div class="conversation-list">
                            <div class="chat-avatar">
                                <img src="`+ avatar + `" alt="">
                            </div>
                            <div class="user-chat-content">`;

    for (var i = 0; i < message.length; i++) {
        
        var dt = new Date(message[i].dateCreated);

        if (message[i].messageType.code == 1) {
            messageItem += `<div class="ctext-wrap" id='` + message[i].id +`'>
                                    <div class="ctext-wrap-content" style="width: 100%;text-align: left;">
                                        <p class="mb-0">
                                            `+ message[i].messageBody + `
                                        </p>
                                        <p class="chat-time mb-0"><i class="ri-time-line align-middle"></i> <span class="align-middle">`+ convertShortDate(dt)+ `</span></p>
                                    </div>
                              </div>
                                `;
        }
        else {
            messageItem += `<div class="ctext-wrap"  id='` + message[i].id +`'>
                               <div class="ctext-wrap-content">
                                    <ul class="list-inline message-img  mb-0">
                                       <li class="list-inline-item message-img-list">
                                            <div>
                                                <a class="popup-img d-inline-block m-1" href="/messageimages/`+ message[i].messageBody  +`" target="_blank" title="Project 1">
                                                    <img src="/messageimages/`+ message[i].messageBody   +`" alt="" class="rounded border">
                                                </a>
                                            </div>
                                            <p class="chat-time mb-0"><i class="ri-time-line align-middle"></i> <span class="align-middle">`+ convertShortDate(dt) + `</span></p>      
                                        </li>
                                    </ul>
                                </div>
                            </div>`;
        }
    }
    messageItem += `<div class="conversation-name">`+ name +`</div>
                    </div>
                   </div>
            </li>`;
    chatMessageListIndex++;
    return messageItem;
}
// bind Generated html to the chat section
function showChatMesaage(isPageFetch,beforeItemIdToScroll) {
    chatMessageListIndex = 0;
    var html = "";
    for (var i = messageList.length - 1; i >= 0; i--) {
        if (messageList[i].sourceUserId == selfId) {
            var msg = [];
            if (messageList[i].messageType.code != 3) {
                msg.push(messageList[i]);

                while ((i - 1) >= 0 && messageList[i - 1].sourceUserId == selfId) {
                    i = i - 1;
                    msg.push(messageList[i]);
                }

                html += getChatMessageHtml(msg, true);
            }
        }
        else {
            var msg = [];
            if (messageList[i].messageType.code != 3) {
                msg.push(messageList[i]);

                while ((i - 1) >= 0 && messageList[i - 1].sourceUserId == msg[0].sourceUserId) {
                    i = i - 1;
                    msg.push(messageList[i]);
                }

                html += getChatMessageHtml(msg, false);
            }
        }
    }
    $('#chatListUl').html('');
    $('#chatListUl').html(html);
    if (isPageFetch == null || isPageFetch == undefined || isPageFetch == false) {
        setTimeout(
            function () {
                $('li[name="chat"]').last().addClass('active-li').focus();
            }, 100);

    }
    else {
        setTimeout(
            function () {
                debugger;
                $('#' + beforeItemIdToScroll).parent().parent().parent().addClass('active-li').focus();
        }, 50);
    }
        
}
// Request of channel history
function showChannel(id, newPage, beforeItemIdToScroll) {
    $('#membershipList').show();
    if (newPage == null || newPage == false || newPage == undefined) {
        currentPage = 1;
    }
    else {
        currentPage++;
    }
    if ($('li[id=' + id + ']').find('span')[1] != undefined)
        $('li[id=' + id + ']').find('span')[1].innerText = "";
    var request = { channelId: id, page: currentPage, pageSize: 30 }
    var result = ajaxRequest('post', '/Home/getChannelData', request);
    console.log(result);
    if (result.success) {
        if (result.data.user.id != $('#channelId').val()) {

            var dt = new Date(result.data.channel.dateCreated);

            $('#chatPageAvatarImage').attr("src", result.data.channel.avatar);
            $('#chatPageSideAvatar').attr("src", result.data.channel.avatar);
            $('#chatPageTitle').text(result.data.channel.channelName);
            $('#chatPageSideTitle').text(result.data.channel.channelName);

            $('#chatPageSideNameTitle').text("Channel Name");
            $('#chatPageSideName').text(result.data.channel.channelName);
            $('#chatPageSideSurnameTitle').text("Channel Owner");
            $('#chatPageSideSurname').text(result.data.user.name + ' ' + result.data.user.surname);
            $('#chatPageSideEmailTitle').text("Memeber count");
            $('#chatPageSideEmail').text(result.data.membershipCount);
            $('#chatPageSideDateTitle').text("Create Date");
            $('#chatPageSideDate').text(convertShortDate(dt));
            $('#channelId').val(result.data.channel.id);
            $('#userId').val('');



            if (result.data.requestedUserIsMemberShip) {
                if (result.data.requestedUserIsAdmin) {
                    $('#deleteChannelIcon').show();
                }
                else {
                    $('#deleteChannelIcon').attr("style", "display:none!important;");
                }
                debugger;
                if (!result.data.channel.isReadOnly || result.data.requestedUserIsAdmin) {
                    $('#newMessageBar').html(`<div class="col">
                                <div>
                                    <input type="text" class="form-control form-control-lg bg-light border-light" placeholder="Enter Message..." id="messageInput">
                                </div>
                            </div>
                            <div class="col-auto">
                                <div class="chat-input-links ml-md-2">
                                    <ul class="list-inline mb-0">
                                       <li class="list-inline-item">
                                            <button type="button" class="btn btn-link text-decoration-none font-size-16 btn-lg waves-effect" data-toggle="tooltip" id="sendImageButton" data-placement="top" title="" data-original-title="Attached File" onclick="sendImageButton()">
                                                <i class="ri-attachment-line"></i>
                                            </button>
                                        </li>
                                        <li class="list-inline-item">
                                            <button type="submit" class="btn btn-primary font-size-16 btn-lg chat-send waves-effect waves-light" id="sendMessageButton" onclick="sendMessage()">
                                                <i class="ri-send-plane-2-fill"></i>
                                            </button>
                                        </li>
                                    </ul>
                                </div>

                            </div>`);
                }
                else {
                    $('#newMessageBar').html(`<div class="col"><p>This Channel is readonly and you can just see Channel Admin messages!!</p><\div>`);

                }
            }
            else {
                if (result.data.channel.isReadOnly) {
                    $('#newMessageBar').html(`<div class="col"><p>This Channel is readonly and you can just see Channel Admin messages!! <a class="btn btn-primary " onclick="subScribe('` + result.data.channel.id +`')">SubScribe</a></p><\div>`);
                }
                else {
                    $('#newMessageBar').html(`<div class="col"><p>You do not subscribe on this channel, please subscribe  <a class="btn btn-primary " onclick="subScribe('` + result.data.channel.id +`')">SubScribe</a></p><\div>`);

                }
                //if (result.data.channel.isPrivate) {
                //    $('#newMessageBar').html(`<div class="col"><p>This Channel is Private and only Channel Admin can be add you on this Channel !!</p><\div>`);

                //}
            }
            var requestMember = { channelId: id}
            var members = ajaxRequest('post', '/Home/GetChannelMember', requestMember);
            if (members.success) {
                memberList = [];
                for (var j = 0; j < members.data.length; j++) {
                    var chat = {
                        title: members.data[j].title,
                        subTitle: members.data[j].subTitle,
                        avatar: members.data[j].avatar,
                        isChannel: members.data[j].isChannel,
                        lastMofified: members.data[j].lastMofified,
                        id: members.data[j].id
                    }
                    memberList.push(chat);
                }
                //show MemberShip
                var memberHtml = "";
                for (var i = 0; i < memberList.length; i++) {
                    memberHtml +=`<div class="mt-4">` + getResentItem(memberList[i]) + `</div>`;
                }
                $('#collapseTow .card-body').html(memberHtml);
            }
            else {
                ShowMessage(result.message, "Error", false);
            }
        }
        else {

        }
        if (newPage == null || newPage == false || newPage == undefined) {
            messageList = [];

            for (var i = 0; i < result.data.messages.length; i++) {
                messageList.push(result.data.messages[i]);
            }
            showChatMesaage()
        }
        else {
            var newMessagesList = [];
            for (var i = 0; i < result.data.messages.length; i++) {
                messageList.push(result.data.messages[i]);
            }
           // messageList = newMessagesList.concat(messageList);
            showChatMesaage(true, beforeItemIdToScroll);
        }
    }
    else {
        console.log(result);
    }
}
// attachment icon click event
function sendImageButton() {
    $('#imageMessage').trigger('click');
}
// request of send file to group or chat
function sendChatImage() {
    var fd = new FormData();
    debugger;
    fd.append('image', $('#imageMessage')[0].files[0]);
    fd.append('userId', $("#userId").val());
    fd.append('channelId', $("#channelId").val());

    var result = ajaxFileRequest('POST', "/Home/ImageMessage", fd);
    if (result.success) {
       // selfMessageAddToChatPage(result.data);
    }
    else {
        ShowMessage(result.message, "Error", false);
    }
}

// send text message by signalr button click event
function sendMessage() {
    var message = $("#messageInput").val();
    if (message.length > 0) {
        if ($('#userId').val() != null && $('#userId').val() != "" && $('#userId').val() != undefined) {
            connection.invoke("SendMessage", $("#userId").val(), message).catch(err => console.log(err));
            $("#messageInput").val("");
        }
        else {
            connection.invoke("SendChannelMessage", $("#channelId").val(), message).catch(err => console.log(err));
            $("#messageInput").val("");
        }
    }
    
}

// request load next page of messages scoll index is 0
function messageScroll(e) {
    if (e.scrollTop == 0) {
        console.log("tt = "+e.scrollTop);
        if ($('#channelId').val() != "" && $('#channelId').val() != null && $('#channelId').val() != undefined) {
            showChannel($('#channelId').val(), true, e.getElementsByTagName('li')[0].getElementsByClassName('ctext-wrap')[0].getAttribute('id'));
        }
        if ($('#userId').val() != "" && $('#userId').val() != null && $('#userId').val() != undefined) {
            showChat($('#userId').val(), true, e.getElementsByTagName('li')[0].getElementsByClassName('ctext-wrap')[0].getAttribute('id'));
        }
    }

}


// Play sound when recive new message
function playSound(isNewMessage) {
    if (isNewMessage) {
        var audio = new Audio('/asset/sounds/newmessage.mp3');
        audio.play();
    }
    else {
        var audio2 = new Audio('/asset/sounds/popcorn.mp3');
        audio2.play();
    }
}


function deleteChannel() {
    var request = {
        channelId: $('#channelId').val(),
    }
    var result = ajaxRequest('post', '/Home/DeleteChannel', request);
    if (result.success) {

        syncChatList(result.data.Id);
        ShowMessage("Your channel successfully deleted", "Success", true);
        $('#deletChannelModal').modal('toggle');
    }
    else {
        ShowMessage(result.message, "Error", false);
    }
}