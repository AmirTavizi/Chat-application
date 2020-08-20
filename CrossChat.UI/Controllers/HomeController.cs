using CrossChat.Domain.Common;
using CrossChat.Domain.Common.CustomClass;
using CrossChat.Domain.Common.Extentions;
using CrossChat.Domain.DBModel;
using CrossChat.Domain.Enums;
using CrossChat.Domain.ViewModel;
using CrossChat.Services;
using CrossChat.Services.ChannelService;
using CrossChat.Services.MessageService;
using CrossChat.Services.UserService;
using jsreport.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrossChat.UI.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        
        IUserService _userService;
        IMessageService _messageService;
        IChannelService _channelService;
        IHostingEnvironment _hostEnvt;
        private readonly IHubContext<CrossChat.UI.Hubs.ChatHub> _hubContext;
        public HomeController(IHubContext<CrossChat.UI.Hubs.ChatHub> hubContext,IUserService userService, IMessageService messageService, IChannelService channelService, IHostingEnvironment hostEnvt)
        {
            _userService = userService;
            _channelService = channelService;
            _messageService = messageService;
            _hostEnvt = hostEnvt;
            _hubContext = hubContext;
        }
        
        public IActionResult Index()
        {
            MessangerLoadViewModel model = new MessangerLoadViewModel();
            var result = _userService.GetUserRegisterdUser();
            if (result.Success)
            {
                model.user = result.Data;
            }
            else {
                return Redirect("/Account/Login");
            }
            return View(model);
        }

        /// <summary>
        /// Add new Channel
        /// </summary>
        /// <param name="model">object of Channel</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddNewChannel(Channel model)
        {
            if (model != null)
            {
               return new JsonResult( _channelService.Add(model));
            }
            else {
                Result result = new Result();
                result.Success = false;
                result.Message = "Is not valid data!";
                return new JsonResult(result);
            }

        }
        /// <summary>
        /// Get Active User All Channels And Messages
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserAllChannelsAndMessages()
        {
            Result<List<ChatListViewModel>> result = new Result<List<ChatListViewModel>>();
            var selfId = new Guid(User.Claims.First(t => t.Type == "UserId").Value);
            var channels = _channelService.GetChannelsByUserIsMembership();
            var chats = _channelService.GetChatByUser();
            List<ChatListViewModel> model = new List<ChatListViewModel>();
            if (channels.Success)
            {
                foreach (var item in channels.Data)
                {
                    ChatListViewModel temp = new ChatListViewModel()
                    {
                        Id = item.Id,
                        IsChannel = true,
                        LastMofified =(DateTime) item.Messages.First().DateCreated,
                        avatar = item.Avatar,
                        SubTitle = "by " +item.MemberShips.FirstOrDefault(a=>a.isCreator).User.Name +" "+ item.MemberShips.FirstOrDefault(a => a.isCreator).User.Surname,
                        Title=item.ChannelName
                        
                    };
                    model.Add(temp);
                }
            }
            else { 
            
            }
            if (chats.Success)
            {
                foreach (var item in chats.Data)
                {

                    ChatListViewModel temp = new ChatListViewModel()
                    {
                        Id = item.Id,
                        IsChannel = false,
                        LastMofified = (DateTime)item.LatestMessageDateTime,
                        avatar = item.Avatar,
                        SubTitle = item.Email,
                        Title=item.Name + " "+item.Surname
                       
                    };

                    model.Add(temp);
                }
            }
            else { 
            
            }
            //var Chats = _messageService.GetChannelsByUserIsMembership();
            result.Success = true;
            result.Data = model.OrderByDescending(a=>a.LastMofified).ToList();
            return new JsonResult(result);

        }

        /// <summary>
        /// Get Search Channels Or Users
        /// </summary>
        /// <param name="key">search value</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchChannelsOrUsers(string key)
        {
            Result<List<SearchViewModel>> result = new Result<List<SearchViewModel>>();
            var channels = _channelService.SearchChannels(key);
            List<SearchViewModel> model = new List<SearchViewModel>();
            if (channels.Success)
            {
                foreach (var item in channels.Data)
                {
                    SearchViewModel temp = new SearchViewModel()
                    {
                        Id = item.Id,
                        IsChannel = true,
                       
                        avatar = item.Avatar,
                        Title=item.ChannelName
                    };
                    model.Add(temp);
                }
            }
            else { 
            
            }
            var users = _userService.SearchUsers(key);
            if (users.Success)
            {
                foreach (var item in users.Data)
                {
                    SearchViewModel temp = new SearchViewModel()
                    {
                        Id = item.Id,
                        IsChannel = false,

                        avatar = item.Avatar,
                        Title = item.Name+" "+item.Surname
                    };
                    model.Add(temp);
                }
            }
            else
            {

            }
            //var Chats = _messageService.GetChannelsByUserIsMembership();
            result.Success = true;
            result.Data = model;
            return new JsonResult(result);

        }
        /// <summary>
        /// Get history of a conversation
        /// </summary>
        /// <param name="userId">Destination user Id</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getChatData(Guid userId,int page,int pageSize)
        {
            Result<ChatViewModel> result = new Result<ChatViewModel>();
            var user = _userService.GetUserById(userId);

            var selfId = new Guid(User.Claims.First(t => t.Type == "UserId").Value);

            var messages = _messageService.getChatMessage(page,pageSize,userId,selfId);

            result.Data = new ChatViewModel()
            {
                page = page,
                pageSize = pageSize,
                messages = messages.Data,
                user = new User() { 
                    Id= user.Data.Id,
                    Avatar= user.Data.Avatar,
                    Name = user.Data.Name,
                    Surname = user.Data.Surname,
                    Email = user.Data.Email,
                    DateCreated = user.Data.DateCreated
                }
            };
            result.Success = true;
            return new JsonResult(result);

        }
        /// <summary>
        /// Get history of a channel
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult getChannelData(Guid channelId, int page, int pageSize)
        {
            Result<ChannelViewModel> result = new Result<ChannelViewModel>();
            var channel = _channelService.GetById(channelId);
            var user = channel.Data.MemberShips.FirstOrDefault(a => a.isCreator).User;

            var selfId = new Guid(User.Claims.First(t => t.Type == "UserId").Value);
            Result<List<Message>> messages = new Result<List<Message>>();
            if (channel.Data.MemberShips.Any(a => a.UserId == selfId))
                messages = _messageService.getChannelMessage(page, pageSize, channelId);
            else
                messages.Data = new List<Message>();

            var mes = messages.Data.Select(a => new Message()
            {
                SourceUser = new User()
                {
                    Avatar = a.SourceUser.Avatar,
                    Name = a.SourceUser.Name,
                    Surname = a.SourceUser.Surname
                },
                MessageTypeId = a.MessageTypeId,
                MessageType = a.MessageType,
                MessageBody = a.MessageBody,
                Id=a.Id,
                SourceUserId = a.SourceUserId,
                DateCreated = a.DateCreated
            }).ToList();


            result.Data = new ChannelViewModel()
            {
                page = page,
                pageSize = pageSize,
                messages = mes,
                user = new User()
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email
                },
                channel=new Channel() { 
                    ChannelName=channel.Data.ChannelName,
                    Avatar= channel.Data.Avatar,
                    Id = channel.Data.Id,
                   
                    IsReadOnly = channel.Data.IsReadOnly,
                    DateCreated = channel.Data.DateCreated,
                    Description = channel.Data.Description,
                },
                membershipCount= channel.Data.MemberShips.Count,
                requestedUserIsMemberShip = channel.Data.MemberShips.Any(a=>a.UserId== selfId),
                requestedUserIsAdmin = (user.Id == selfId)
                

            };
            result.Success = true;
            return new JsonResult(result);

        }
        /// <summary>
        /// Add image message
        /// </summary>
        /// <param name="userId">destination user id (null when sended image to the channel)</param>
        /// <param name="channelId">Channel id (null when sended image to the user)</param>
        /// <param name="image">Image file</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ImageMessage(string userId, string channelId, Microsoft.AspNetCore.Http.IFormFile image)
        {
            Guid? userIdGuid = null;
            Guid? channelIdGuid = null;
            if (userId != null && userId != "")
            {
                userIdGuid = Guid.Parse(userId);
            }
            else {
                channelIdGuid = Guid.Parse(channelId);
            }
            Result result = new Result();
            var selfId = new Guid(User.Claims.First(t => t.Type == "UserId").Value);
            var currentUser = _userService.GetUserRegisterdUser();
            if (image != null && image.Length > 0)
            {
                var messageId = Guid.NewGuid();
                var uniqueFileName = messageId.ToString()+ Path.GetExtension(image.FileName);
                var uploads = Path.Combine(_hostEnvt.WebRootPath, "messageimages");
                var filePath = Path.Combine(uploads, uniqueFileName);
                image.CopyTo(new FileStream(filePath, FileMode.Create));
                Message newMessage = new Message()
                {
                    Id=messageId,
                    SourceUserId=selfId,
                    DestinationUserId= userIdGuid,
                    ChannelId= channelIdGuid,
                    MessageBody=uniqueFileName,
                    MessageTypeId = Guid.Parse(CrossChat.Domain.Enums.MessageType.Image.Description())
                };

                var message=_messageService.addMessage(newMessage);
                message.Data.SourceUser = currentUser.Data;
                if (channelId == null)
                {
                    if (ChatOnlineUsers.OnlineUsers == null)
                        ChatOnlineUsers.OnlineUsers = new List<ChatOnlineUserModel>();
                    var chatUser = ChatOnlineUsers.OnlineUsers//.FirstOrDefault(a => a.UserId == userIdGuid);
                        .Where(a => a.UserId == userIdGuid )
                        .Select(a => a.clientId).ToList();
                    var sendedUser = ChatOnlineUsers.OnlineUsers.Where(a => a.UserId == selfId).Select(a => a.clientId).ToList();
                    if (chatUser != null)
                        await _hubContext.Clients.Clients(chatUser ).SendAsync("ReceiveMessage", message.Data);
                    await _hubContext.Clients.Clients(sendedUser).SendAsync("ResultOfSendMessage", message.Data);
                }
                else {
                    var user = _userService.GetUserRegisterdUser();
                    Result<List<Guid>> memberShipIds = _channelService.GetChannelMemberShipUserIds((Guid)channelIdGuid);
                    var channelUsers = ChatOnlineUsers.OnlineUsers.Where(a => memberShipIds.Data.Contains(a.UserId) && a.UserId != selfId).Select(a => a.clientId).ToList();
                    message.Data.SourceUser = new User()
                    {
                        Avatar = user.Data.Avatar,
                        Name = user.Data.Name,
                        Surname = user.Data.Surname
                    };
                    try
                    {
                        //var sendedUser = ChatOnlineUsers.OnlineUsers.FirstOrDefault(a => a.UserId == selfId);
                        var selfClientId = ChatOnlineUsers.OnlineUsers
                       .Where(a => a.UserId == selfId)
                       .Select(a => a.clientId).ToList();
                        if (channelUsers != null && channelUsers.Count > 0)
                            await _hubContext.Clients.Clients(channelUsers).SendAsync("ReceiveMessage", message.Data);
                        await _hubContext.Clients.Clients(selfClientId).SendAsync("ResultOfSendMessage", message.Data);
                    }
                    catch (Exception e)
                    {
                    }
                    
                }
               
                
                result = message;
            }
            else {
                result.Success = false;
                result.Message = "File is not exist!";
            }

            return new JsonResult(result);

        }
        /// <summary>
        /// Join active user to channel
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubscribeToChannel(Guid channelId)
        {
            
            var selfId = new Guid(User.Claims.First(t => t.Type == "UserId").Value);
            MemberShip newMembership = new MemberShip()
            {
                Id = Guid.NewGuid(),
                UserId = selfId,
                ChannelId = channelId,
                isCreator = false
            };
            var result = _channelService.addMemberShip(newMembership);
            return new JsonResult(result);

        }
        /// <summary>
        /// Delete channel
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> DeleteChannel(Guid channelId)
        {
            var result = _channelService.DeleteChannel(channelId);
            if(result.Success)
                await _hubContext.Clients.All.SendAsync("DeleteChannel", channelId);
            return new JsonResult(result);

        }

        /// <summary>
        /// Get Channel Members
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetChannelMember(Guid channelId)
        {
            var result = _channelService.GetChannelMemberShip(channelId);
            
            return new JsonResult(result);

        }
    }
}
