using CrossChat.Domain.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrossChat.Domain.ViewModel
{
    public class ChatViewModel
    {
        public User user { get; set; }
        public List<Message> messages { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}
