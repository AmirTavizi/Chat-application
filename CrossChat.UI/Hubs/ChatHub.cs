using CrossChat.Domain.Common;
using CrossChat.Domain.Common.CustomClass;
using CrossChat.Domain.Common.Extentions;
using CrossChat.Domain.DBModel;
using CrossChat.Domain.ViewModel;
using CrossChat.Services.ChannelService;
using CrossChat.Services.MessageService;
using CrossChat.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossChat.UI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        IMessageService _messageService;
        IChannelService _channelService;
        IUserService _userService;
        public ChatHub(IMessageService messageService, IUserService userService, IChannelService channelService)
        {
            _messageService = messageService;
            _channelService = channelService;
            _userService = userService;
        }
        public override Task OnConnectedAsync()
        {
            try
            {
                var userId =Guid.Parse( Context.User.Claims.First(t => t.Type == "UserId").Value);
                if (ChatOnlineUsers.OnlineUsers == null)
                    ChatOnlineUsers.OnlineUsers = new List<ChatOnlineUserModel>();

                var user = ChatOnlineUsers.OnlineUsers.FirstOrDefault(a => a.UserId == userId && a.clientId==Context.ConnectionId);
                if (user == null)
                {
                    ChatOnlineUsers.OnlineUsers.Add(new ChatOnlineUserModel()
                    {
                        clientId = Context.ConnectionId,
                        UserId= userId
                    }) ;
                }
            }
            catch (Exception e) { 
            
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var userId = Guid.Parse(Context.User.Claims.First(t => t.Type == "UserId").Value);
                if (ChatOnlineUsers.OnlineUsers == null)
                    ChatOnlineUsers.OnlineUsers = new List<ChatOnlineUserModel>();

                var user = ChatOnlineUsers.OnlineUsers.FirstOrDefault(a => a.UserId == userId && a.clientId== Context.ConnectionId);
                if (user != null)
                {
                    ChatOnlineUsers.OnlineUsers.Remove(user);
                }
            }
            catch (Exception e)
            {

            }
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string user, string message)
        {
            try
            {
                var userId = Guid.Parse(Context.User.Claims.First(t => t.Type == "UserId").Value);
                Message newMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    SourceUserId = userId,
                    ChannelId=null,
                    DestinationUserId = Guid.Parse(user),
                    MessageBody = message,
                    MessageTypeId = Guid.Parse(CrossChat.Domain.Enums.MessageType.Text.Description())
                };
               var result = _messageService.addMessage(newMessage);
                if (result.Success)
                {
                    var chatUser = ChatOnlineUsers.OnlineUsers
                        .Where(a=>a.UserId==result.Data.DestinationUserId && a.clientId!= Context.ConnectionId)
                        .Select(a=>a.clientId).ToList();
                    var selfClientId = ChatOnlineUsers.OnlineUsers
                        .Where(a=>a.UserId== userId)
                        .Select(a=>a.clientId).ToList();
                    if(chatUser!=null && chatUser.Count>0)
                        await Clients.Clients(chatUser).SendAsync("ReceiveMessage", result.Data);
                    if (selfClientId != null && selfClientId.Count > 0)
                        await Clients.Clients(selfClientId).SendAsync("ResultOfSendMessage", result.Data);
                }
            }
            catch (Exception)
            {
                // todo log
            }


        }
        public async Task SendChannelMessage(string channelId, string message)
        {
            try
            {
                var userId = Guid.Parse(Context.User.Claims.First(t => t.Type == "UserId").Value);
                var user = _userService.GetUserRegisterdUser();
                Message newMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    SourceUserId = userId,
                    
                    ChannelId = Guid.Parse(channelId),
                    DestinationUserId = null,
                    MessageBody = message,
                    MessageTypeId = Guid.Parse(CrossChat.Domain.Enums.MessageType.Text.Description())
                };
               var result = _messageService.addMessage(newMessage);
                if (result.Success)
                {
                    Result<List<Guid>> memberShipIds = _channelService.GetChannelMemberShipUserIds((Guid)result.Data.ChannelId);
                    var channelUsers = ChatOnlineUsers.OnlineUsers.Where(a=> memberShipIds.Data.Contains( a.UserId) && a.UserId != userId).Select(a=>a.clientId).ToList();
                    result.Data.SourceUser = new User()
                    {
                        Avatar = user.Data.Avatar,
                        Name = user.Data.Name,
                        Surname = user.Data.Surname
                    };

                    var selfClientId = ChatOnlineUsers.OnlineUsers
                        .Where(a => a.UserId == userId)
                        .Select(a => a.clientId).ToList();
                    if (channelUsers != null && channelUsers.Count>0)
                        await Clients.Clients(channelUsers).SendAsync("ReceiveMessage", result.Data);
                    if (selfClientId != null && selfClientId.Count > 0)
                        await Clients.Clients(selfClientId).SendAsync("ResultOfSendMessage", result.Data);
                }
            }
            catch (Exception)
            {
                // todo log
                //throw;
            }
           
            
        }
    }
}
