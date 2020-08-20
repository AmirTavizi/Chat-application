using CrossChat.DataAccess;
using CrossChat.Domain.Common;
using CrossChat.Domain.Common.Extentions;
using CrossChat.Domain.DBModel;
using CrossChat.Domain.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrossChat.Services.ChannelService
{
    public class ChannelService : IChannelService
    {
        IRepository<Channel> _repositoryChannel;
        IRepository<MemberShip> _repositoryMembership;
       
        CrossChatContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public ChannelService(IRepository<MemberShip> repositoryMembership, CrossChatContext context, IHttpContextAccessor httpContextAccessor, IRepository<Channel> repositoryChannel)
        {
            _context = context;
            _repositoryChannel = repositoryChannel;
            _httpContextAccessor = httpContextAccessor;
            _repositoryMembership = repositoryMembership;
        }
        public Result<Channel> Add(Channel model)
        {
            Result<Channel> result = new Result<Channel>();
            try
            {
                model.Id = new Guid();
                var OwnerId = Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);
                result.Data= _repositoryChannel.Add(model);
                Message message = new Message() { 
                    SourceUserId=OwnerId,
                    ChannelId=result.Data.Id,
                    MessageTypeId= Guid.Parse(CrossChat.Domain.Enums.MessageType.Info.Description()),
                    MessageBody="Channel created.",
                    Id=Guid.NewGuid()
                    
                };
                _context.Message.Add(message);
                if (result.Data != null)
                {
                    _repositoryMembership.Add(new MemberShip()
                    {
                        Id = Guid.NewGuid(),
                        UserId = OwnerId,
                        isCreator = true,
                        ChannelId = result.Data.Id
                    }) ;
                    result.Success = true;
                }
                else {
                    result.Success = false;
                    result.Message = "Internal Server Error. Please try agan later!";
                }
            }
            catch (Exception e)
            {
                // todo log
                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }

        public Result addMemberShip(MemberShip newMembership)
        {
            Result result = new Result();
            try
            {
                if (_context.MemberShip.Any(a => a.UserId == newMembership.UserId && a.ChannelId == newMembership.ChannelId))
                {
                    result.Success = false;
                    result.Message = "you are already Join this Channel";
                }
                else {
                    _repositoryMembership.Add(newMembership);
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = "Internal Server error. Please try again later!";
            }
            return result;
        }

        public Result<Channel> DeleteChannel(Guid channelId)
        {
            Result<Channel> result = new Result<Channel>();
            
            try
            {
                var OwnerId = Guid.Parse(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);
                var channel = _context.Channels.Include(a=>a.MemberShips).FirstOrDefault(a =>!a.IsDeleted && a.Id == channelId);
                if (channel.MemberShips.Any(a => a.UserId == OwnerId && a.isCreator))
                {
                    channel.IsDeleted = true;
                    result.Data= _context.Channels.Update(channel).Entity;
                    _context.SaveChanges();
                    result.Success = true;
                }
                else {
                    result.Success = false;
                    result.Message = "This channel 's not exist or You can not delete this channel. becuse only channel owner can be delete channels!";
                }


            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Internal server error!";
            }
            return result;
        }

        public Result<Channel> Edit(Channel model)
        {
            Result<Channel> result = new Result<Channel>();
            try
            {
                result.Success = _repositoryChannel.Edit(model);
                if (result.Success)
                {
                    result.Data = model;
                }
                else
                {
                    result.Message = "Internal Server Error. Please try agan later!";
                }
            }
            catch (Exception e)
            {
                // todo log
                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }

        public Result<Channel> GetById(Guid channelId)
        {
            Result<Channel> result = new Result<Channel>();
            try
            {
                result.Data = _context.Channels.Include(a => a.MemberShips).ThenInclude(a => a.User).FirstOrDefault(a =>!a.IsDeleted && a.Id == channelId);
                //result.Data = _repositoryChannel.GetByID(channelId);
                if (result.Data != null)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Internal Server Error. Please try agan later!";
                }
            }
            catch (Exception e)
            {
                // todo log
                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }

        public Result<List<ChatListViewModel>> GetChannelMemberShip(Guid channelId)
        {
            Result<List<ChatListViewModel>> result = new Result<List<ChatListViewModel>>();
            try
            {
                result.Data = _context.MemberShip.Include(a => a.Channel).Include(a => a.User).Where(a => !a.Channel.IsDeleted && a.ChannelId == channelId)
                    .Select(a => new ChatListViewModel()
                    {
                        Id = a.User.Id,
                        IsChannel = false,
                        LastMofified =(DateTime) a.User.DateCreated,
                        avatar = a.User.Avatar,
                        SubTitle = a.User.Email,
                        Title = a.User.Name + " " + a.User.Surname
                    }).ToList();
                result.Success = true;
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }

        public Result<List<Guid>> GetChannelMemberShipUserIds(Guid channelId)
        {
            Result<List<Guid>> result = new Result<List<Guid>>();
            try
            {
                result.Data = _context.MemberShip.Where(a => a.ChannelId == channelId).Select(a => a.UserId).ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }

        public Result<List<Channel>> GetChannelsByUserIsAdmin()
        {
            Result<List<Channel>> result = new Result<List<Channel>>();
            try
            {
                if (_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId") != null)
                {
                    var userId = new Guid(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);

                    result.Data = _context.Channels.Where(a =>!a.IsDeleted && a.MemberShips.Any(c=>c.UserId == userId && c.isCreator)).ToList();
                    if (result.Data != null)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Internal Server Error. Please try agan later!";
                    }
                }
                else {
                    result.Success = false;
                    result.Message = "Not Authorized!";
                }
            }
            catch (Exception e)
            {
                // todo log
                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }

        public Result<List<Channel>> GetChannelsByUserIsMembership()
        {
            Result<List<Channel>> result = new Result<List<Channel>>();
            try
            {
                if (_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId") != null)
                {
                    var userId = new Guid(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);

                    result.Data = _context.Channels
                        
                        .Include(a=>a.MemberShips).ThenInclude(a=>a.User).Include(a=>a.Messages)
                        .Where(a =>!a.IsDeleted && a.MemberShips.Any(c=>c.UserId == userId)).Select(a=>new Channel() { 
                            Id= a.Id,
                            MemberShips=a.MemberShips,
                            ChannelName=a.ChannelName,
                            Avatar=a.Avatar,
                            DateCreated=a.DateCreated,
                            Description=a.Description,
                            IsReadOnly=a.IsReadOnly,
                            Messages= new List<Message>(){ a.Messages.OrderByDescending(q => q.DateCreated).FirstOrDefault() }
                            
                        }).ToList();

                    if (result.Data != null)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Internal Server Error. Please try agan later!";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "Not Authorized!";
                }
            }
            catch (Exception e)
            {
                // todo log
                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }

        public Result<List<User>> GetChatByUser()
        {
            Result<List<User>> result = new Result<List<User>>();
            try
            {
                var userId = new Guid(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);
                var users = _context.Message.Include(a=>a.SourceUser).Include(a=>a.DestinationUser)
                    .Where(a => (a.SourceUserId == userId || a.DestinationUserId == userId) && a.DestinationUserId != null)
                    .Select(a => a.SourceUserId == userId ? a.DestinationUser : a.SourceUser)
                    .Distinct().ToList();
                users = users.Distinct(new UserComparer()).ToList();
                foreach (var item in users)
                {
                    
                    item.LatestMessageDateTime = _context.Message.Where(a => a.SourceUserId == item.Id || a.DestinationUserId == item.Id).OrderByDescending(a => a.DateCreated).First().DateCreated;
                }
                //result.Data = _context.Message
                //    .Where(a => (a.SourceUserId == userId || a.DestinationUserId == userId) && a.DestinationUserId!=null).Select(a => (a.SourceUserId == userId) ?
                //     new User() { 
                //         Id= a.DestinationUser.Id,
                //         Email= a.DestinationUser.Email,
                //         Avatar=a.DestinationUser.Avatar,
                //         Name = a.DestinationUser.Name,
                //         Surname= a.DestinationUser.Surname,
                //         LatestMessage =  new Message() { 
                //            Id=a.Id,
                //            DateCreated=a.DateCreated,
                //            MessageBody=a.MessageBody
                            
                //         } 
                         
                //     }  : new User()
                //     {
                //         Id = a.SourceUser.Id,
                //         Email = a.SourceUser.Email,
                //         Avatar = a.SourceUser.Avatar,
                //         Name = a.SourceUser.Name,
                //         Surname = a.SourceUser.Surname,
                //         LatestMessage =  new Message() {
                //            Id=a.Id,
                //            DateCreated=a.DateCreated,
                //            MessageBody=a.MessageBody

                //         } 

                //     } ).Distinct()
                //    .ToList();
                result.Data = users;// result.Data.Distinct().ToList();
                result.Success = true;       
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }

        public Result<List<Channel>> SearchChannels(string key)
        {
            Result<List<Channel>> result = new Result<List<Channel>>();
            try
            {

                    result.Data = _context.Channels
                        .Where(a=>!a.IsDeleted && a.ChannelName.Contains(key))
                        .ToList();

                    if (result.Data != null)
                    {
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Internal Server Error. Please try agan later!";
                    }
                
            }
            catch (Exception e)
            {
                // todo log
                result.Success = false;
                result.Message = "Internal Server Error. Please try agan later!";
            }
            return result;
        }
    }


    public class UserComparer : IEqualityComparer<User>
    {
        #region IEqualityComparer<Message> Messages

        public bool Equals(User x, User y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(User obj)
        {
            return obj.Id.GetHashCode();
        }

        #endregion
    }
}
