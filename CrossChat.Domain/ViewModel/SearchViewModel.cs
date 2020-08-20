using System;
using System.Collections.Generic;
using System.Text;

namespace CrossChat.Domain.ViewModel
{
    public class SearchViewModel
    {
        public string Title { get; set; }
        public string avatar { get; set; }
        public bool IsChannel { get; set; }
        public Guid Id { get; set; }

    }
}
