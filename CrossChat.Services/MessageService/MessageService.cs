using CrossChat.DataAccess;
using CrossChat.Domain.Common;
using CrossChat.Domain.DBModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrossChat.Services.MessageService
{
    public class MessageService: IMessageService
    {
        CrossChatContext _context;
        IRepository<Message> _repositoryMessage;
        IHttpContextAccessor _httpContextAccessor;
        public MessageService(IHttpContextAccessor httpContextAccessor, CrossChatContext context, IRepository<Message> repositoryMessage)
        {
            _context = context;
            _repositoryMessage = repositoryMessage;
            _httpContextAccessor = httpContextAccessor;
        }

        public Result<Message> addMessage(Message message)
        {
            Result<Message> result = new Result<Message>();
            try
            {
                result.Data = _repositoryMessage.Add(message);
                result.Data = _context.Message.Include(a=>a.MessageType).FirstOrDefault(a=>a.Id ==  result.Data.Id);
                if (result.Data != null)
                {
                    result.Success = true;
                }
                else {
                    result.Success = false;
                    result.Message = "Internal server error!";
                }
            }
            catch (Exception e)
            {
                // todo log
                result.Success = false;
                result.Message = "Internal server error!";
            }
            return result;
        }

        public Result<List<Message>> getChatMessage(int page, int PageSize, Guid sourceId, Guid destinationId)
        {
            Result<List<Message>> result = new Result<List<Message>>();
            var skip = (page - 1) * PageSize;
            try
            {
                var userId = new Guid(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);
                if (sourceId == userId || destinationId == userId)
                {
                    result.Data = _context.Message
                        .Include(a => a.MessageType).Include(a=>a.SourceUser).Include(a=>a.DestinationUser)
                        .Where(a => (a.SourceUserId == sourceId && a.DestinationUserId == destinationId)|| (a.SourceUserId == destinationId && a.DestinationUserId == sourceId)).OrderByDescending(a=>a.DateCreated).Skip(skip).Take(PageSize).ToList();
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Access is denied!";
                }
            }
            catch (Exception e)
            {

                result.Success = false;
                result.Message = "Internal server error!";
            }
            return result;
        }
    
        public Result<List<Message>> getChannelMessage(int page, int PageSize, Guid channelId, Guid? afterId=null)
        {
            Result<List<Message>> result = new Result<List<Message>>();
            var skip = (page - 1) * PageSize;

 

            try
            {

                    result.Data = _context.Message
                    .Include(a => a.MessageType)
                    .Include(a=>a.SourceUser)
                    .Where(a =>a.ChannelId == channelId && !a.Channel.IsDeleted)
                    .OrderByDescending(a=>a.DateCreated)
                    .Skip(skip).Take(PageSize).ToList();
                    result.Success = true;

            }
            catch (Exception e)
            {

                result.Success = false;
                result.Message = "Internal server error!";
            }
            return result;
        }
    }
}
