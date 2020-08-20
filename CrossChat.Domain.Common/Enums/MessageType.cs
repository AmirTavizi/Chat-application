using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossChat.Domain.Enums
{
    public enum MessageType
    {
        [Description("a99daa15-7805-4bd7-80f3-5cba3d6b6c39")]
        Text = 1,
        [Description( "59a4920f-5154-47f8-86f6-648835050083")]
        Image =2,
        [Description( "03781bb0-1492-4c38-bd9e-092751851446")]
        Info = 3

    }

}
