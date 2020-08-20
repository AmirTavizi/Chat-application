using CrossChat.Domain.Common;
using CrossChat.Domain.DBModel;
using CrossChat.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrossChat.Services.ChannelService
{
    public interface IChannelService
    {
        /// <summary>
        /// Add new Channel Service
        /// </summary>
        /// <param name="model">Object of Channel</param>
        /// <returns></returns>
        Result<Channel> Add(Channel model);
        /// <summary>
        /// Edit a Channel Service
        /// </summary>
        /// <param name="model">Object of Channel</param>
        /// <returns></returns>
        Result<Channel> Edit(Channel model);
        /// <summary>
        /// Get a Channel Service
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <returns></returns>
        Result<Channel> GetById(Guid channelId);
        /// <summary>
        /// Get Channels that active user is membership it
        /// </summary>
        /// <returns></returns>
        Result<List<Channel>> GetChannelsByUserIsMembership();
        /// <summary>
        /// Get Channels that active user is owner it
        /// </summary>
        /// <returns></returns>
        Result<List<Channel>> GetChannelsByUserIsAdmin();
        /// <summary>
        /// Search channels
        /// </summary>
        /// <param name="key">serch value</param>
        /// <returns></returns>
        Result<List<Channel>> SearchChannels(string key);
        Result<List<User>> GetChatByUser();
        /// <summary>
        /// add new member ship to channel
        /// </summary>
        /// <param name="newMembership">object of membership</param>
        /// <returns></returns>
        Result addMemberShip(MemberShip newMembership);
        /// <summary>
        /// get Channel memberships Id's
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <returns></returns>
        Result<List<Guid>> GetChannelMemberShipUserIds(Guid channelId);
        /// <summary>
        /// Soft Delete Channel 
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <returns></returns>
        Result<Channel> DeleteChannel(Guid channelId);
        Result<List<ChatListViewModel>> GetChannelMemberShip(Guid channelId);
    }
}
