using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrossChat.Domain.Common
{
    public class Result
    {
        public Result()
        {
            Messages = new List<string>();

        }

        public string Message
        {
            get
            {
                return Messages.FirstOrDefault();
            }
            set
            {
                Messages.Add(value);
            }
        }

        public bool Success { get; set; }

        public List<string> Messages { get; set; }


    }

    public class Result<T> : Result
    {
        public T Data { get; set; }
    }

}
