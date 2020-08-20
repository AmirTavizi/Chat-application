using CrossChat.Domain.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrossChat.Domain.ViewModel
{
    public class ChannelViewModel
    {
        public Channel channel { get; set; }
        public User user { get; set; }
        public List<Message> messages { get; set; }
        public int membershipCount { get; set; }
        public int pageSize { get; set; }
        public int page { get; set; }
        public bool requestedUserIsMemberShip { get; set; }
        public bool requestedUserIsAdmin { get; set; }
    }
}
