using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossChat.Domain.Exceptions
{
    public class ParameterExceptionDetail
    {
        public ParameterExceptionDetail(string parameterName, string message)
        {
            ParameterName = parameterName;
            Message = message;
        }

        public string ParameterName { get; set; }
        public string Message { get; set; }
    }
}
