using CrossChat.Domain.DBModel;
using CrossChat.Services.UserService;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
//using Xunit;
using CrossChat.Domain.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrossChat.UnitTest
{
    public class UserTest
    {
        IUserService _mockUserService;
        User singleUser;
        public UserTest()
        {
            List<User> users = new List<User>
                {
                  new User()
                 {
                       Email = "amir.tavizi@gmail.com",
                       Name="Amir",
                       Surname="Tavizi",
                       Password="123", // TODO Implement Hash Password
                       Avatar= "/asset/avatar/avatar-25.png",
                        Id=Guid.Parse("70214c91-68c5-47ca-a6ba-86430dc6711b"),
                         IsActive=true,
                       IsDeleted=false,
                       DateModified = DateTime.Parse("2020-08-16 14:14:44.5504190"),
                       DateCreated=DateTime.Parse("2020-08-16 14:14:44.5504190"),
                       UserCreated=Guid.Parse("70214c91-68c5-47ca-a6ba-86430dc6711b"),
                       UserModified=Guid.Parse("70214c91-68c5-47ca-a6ba-86430dc6711b")
                    },
                   new User()
                    {
                        Email = "selin@gmail.com",
                       Name="selin",
                       Surname="akin",
                       Password="123", // TODO Implement Hash Password
                       Avatar= "/asset/avatar/avatar-25.png",
                        Id=Guid.Parse("80214c91-68c5-47ca-a6ba-86430dc6711b"),
                         IsActive=true,
                       IsDeleted=false,
                       DateModified = DateTime.Parse("2020-08-16 14:14:44.5504190"),
                       DateCreated=DateTime.Parse("2020-08-16 14:14:44.5504190"),
                       UserCreated=Guid.Parse("80214c91-68c5-47ca-a6ba-86430dc6711b"),
                       UserModified=Guid.Parse("80214c91-68c5-47ca-a6ba-86430dc6711b")
                },
                   new User()
                {
                       Email = "osman@gmail.com",
                       Name="osman",
                       Surname="nurallah",
                       Password="123", // TODO Implement Hash Password
                       Avatar= "/asset/avatar/avatar-25.png",
                       Id=Guid.Parse("90214c91-68c5-47ca-a6ba-86430dc6711b"),
                       IsActive=true,
                       IsDeleted=false,
                       DateModified = DateTime.Parse("2020-08-16 14:14:44.5504190"),
                       DateCreated=DateTime.Parse("2020-08-16 14:14:44.5504190"),
                       UserCreated=Guid.Parse("90214c91-68c5-47ca-a6ba-86430dc6711b"),
                       UserModified=Guid.Parse("90214c91-68c5-47ca-a6ba-86430dc6711b")
                }
                };
            Mock<IUserService> mockUserServices = new Mock<IUserService>();

            mockUserServices.Setup(mr => mr.GetUserById(It.IsAny<Guid>())).Returns((Guid i) => {
                Result<User> result = new Result<User>();
                try
                {
                    result.Data = users.FirstOrDefault(x => x.Id == i);
                    result.Success = true;
                }
                catch (Exception e)
                {
                    result.Success = false;
                }
                return result;
            });
            singleUser = new User()
            {
                Email = "sinan@gmail.com",
                Name = "sinan",
                Surname = "borluk",
                Password = "123", // TODO Implement Hash Password
                Avatar = "/asset/avatar/avatar-25.png",
                Id = Guid.Parse("10214c91-68c5-47ca-a6ba-86430dc6711b"),
                IsActive = true,
                IsDeleted = false,
                DateModified = DateTime.Parse("2020-08-16 14:14:44.5504190"),
                DateCreated = DateTime.Parse("2020-08-16 14:14:44.5504190"),
                UserCreated = Guid.Parse("10214c91-68c5-47ca-a6ba-86430dc6711b"),
                UserModified = Guid.Parse("10214c91-68c5-47ca-a6ba-86430dc6711b")
            };
            mockUserServices.Setup(mr => mr.AddUser(singleUser)).Returns((User user) => {
                Result<User> result = new Result<User>();
                try
                {
                    users.Add(user);
                    result.Data = user;
                    result.Success = true;
                }
                catch (Exception e)
                {
                    result.Success = false;
                }
                return result;
            });
            mockUserServices.Setup(mr => mr.SearchUsers("selin")).Returns((string key) => {
                Result<List<User>> result = new Result<List<User>>();
                try
                {
                    result.Data = users.Where(a=>a.Email.Contains(key) || a.Name.Contains(key) || a.Surname.Contains(key)).ToList();
                    result.Success = true;
                }
                catch (Exception e)
                {
                    result.Success = false;
                }
                return result;
            });
            

            _mockUserService = mockUserServices.Object;
        }
        
        [TestMethod]
        public void AddNewUserTest()
        {
            var newUser = new User()
            {
                Email = "newUser@gmail.com",
                Name = "name",
                Surname = "fam",
                Password = "123", // TODO Implement Hash Password
                Avatar = "/asset/avatar/avatar-25.png",
                Id = Guid.Parse("20214c91-68c5-47ca-a6ba-86430dc6711b"),
                IsActive = true,
                IsDeleted = false,
                DateModified = DateTime.Parse("2020-08-16 14:14:44.5504190"),
                DateCreated = DateTime.Parse("2020-08-16 14:14:44.5504190"),
                UserCreated = Guid.Parse("20214c91-68c5-47ca-a6ba-86430dc6711b"),
                UserModified = Guid.Parse("20214c91-68c5-47ca-a6ba-86430dc6711b")
            };

            var result = _mockUserService.AddUser(newUser);

            Assert.IsNotNull(result); // Test if null
            Assert.IsInstanceOfType(result, typeof(Result)); // Test type
            Assert.AreEqual(result.Data, newUser); // Test type
            // Assert.AreEqual("ASP.Net Unleashed", testCategory.JsonData); // Verify it is the right product
        }
    }
}
