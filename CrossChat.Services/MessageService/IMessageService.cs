using CrossChat.Domain.Common;
using CrossChat.Domain.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrossChat.Services.MessageService
{
    public interface IMessageService
    {
        /// <summary>
        /// Add new message
        /// </summary>
        /// <param name="message">object of message</param>
        /// <returns></returns>
        Result<Message> addMessage(Message message);
        /// <summary>
        /// get messages from a chat
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="PageSize">PageSize</param>
        /// <param name="sourceId">sended user Id</param>
        /// <param name="destinationId">recive user Id</param>
        /// <returns></returns>
        Result<List<Message>> getChatMessage(int page,int PageSize,Guid sourceId,Guid destinationId);
        /// <summary>
        /// get messages from a channel
        /// </summary>
        /// <param name="page"></param>
        /// <param name="PageSize"></param>
        /// <param name="channelId"></param>
        /// <param name="afterId"></param>
        /// <returns></returns>
        Result<List<Message>> getChannelMessage(int page,int PageSize,Guid channelId, Guid? afterId = null);
    }
}
