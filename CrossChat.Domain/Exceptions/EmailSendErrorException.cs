using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossChat.Domain.Exceptions
{
    public class EmailSendErrorException:Exception
    {
        public EmailSendErrorException(string message)
        {        
            Message = message;
        }
               
        public string Message { get; set; }
    }
}
