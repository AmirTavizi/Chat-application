using System;
using System.Collections.Generic;
using System.Text;

namespace CrossChat.Domain.ViewModel
{
    public class ChatListViewModel
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string avatar { get; set; }
        public DateTime latesMessage { get; set; }
        public bool IsChannel { get; set; }
        public DateTime LastMofified { get; set; }
        public Guid Id { get; set; }

    }
}
