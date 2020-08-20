using System;
using System.Collections.Generic;
using System.Text;
using CrossChat.Domain.DBModel;

namespace CrossChat.Services.JwtService
{
    public interface IJwtService
    {
        string Generate(User user);
    }
}
