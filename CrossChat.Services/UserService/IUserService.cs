using System;
using System.Collections.Generic;
using System.Text;
using CrossChat.Domain.Common;
using CrossChat.Domain.DBModel;

namespace CrossChat.Services.UserService
{
    public interface IUserService
    {
        /// <summary>
        /// login 
        /// </summary>
        /// <param name="UserName">UserName</param>
        /// <param name="Password">Password</param>
        /// <returns></returns>
        Result<User> Login(String UserName, string Password);
        /// <summary>
        /// add new user
        /// </summary>
        /// <param name="user">object of user</param>
        /// <returns></returns>
        Result<User> AddUser(User user);
        /// <summary>
        /// Reset password if email is exist
        /// </summary>
        /// <param name="email">email</param>
        void ResetPassword(string email);
        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="oldPassword">old Password</param>
        /// <param name="newPassword">new Password</param>
        /// <returns></returns>
        Result ChangePassword( string oldPassword, string newPassword);
        /// <summary>
        /// find user by sended hashcode for reset password
        /// </summary>
        /// <param name="key">hashcode</param>
        /// <returns></returns>
        Result<User> GetUserByHashCode(string key);
        /// <summary>
        /// Edit a user
        /// </summary>
        /// <param name="data">object of user</param>
        /// <returns></returns>
        Result Edit(User data);
        /// <summary>
        /// Get active user
        /// </summary>
        /// <returns></returns>
        Result<User> GetUserRegisterdUser();
        /// <summary>
        /// Search Users
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Result<List<User>> SearchUsers(string key);
        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        Result<User> GetUserById(Guid userId);
    }
}
