using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrossChat.Domain.DBModel;

namespace CrossChat.DataAccess
{
    public class SeedDB
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<CrossChatContext>();
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                List<User> user = new List<User>()
                {
                    new User
                   {
                   Email = "amir.tavizi@gmail.com",
                   Name="Amir",
                   Surname="Tavizi",
                   Password="123", // TODO Implement Hash Password
                   Avatar= "/asset/avatar/avatar-25.png",
                   Id=Guid.NewGuid(),
                   
                   
                    },


                };
                context.AddRange(user);

                List<MessageType> messageType = new List<MessageType>()
                {
                    new MessageType
                    {
                        Id=Guid.Parse("a99daa15-7805-4bd7-80f3-5cba3d6b6c39"),
                        code=1,
                        TypeName = "Text Message",
                    },
                    new MessageType
                    {
                        Id=Guid.Parse("59a4920f-5154-47f8-86f6-648835050083"),
                        code=2,
                        TypeName = "Image File"
                    },
                    new MessageType
                    {
                        Id=Guid.Parse("03781bb0-1492-4c38-bd9e-092751851446"),
                        code=3,
                        TypeName = "Info Message"
                    },


                };
                context.AddRange(messageType);
            }


            context.SaveChanges();

        }
    }
}
