using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossChat.UI.ViewModels
{
    public class BaseViewModel
    {
        public Guid TestID { get; set; }
        
        public bool IsExternalSiteUser { get; set; } = false;
    }
}
