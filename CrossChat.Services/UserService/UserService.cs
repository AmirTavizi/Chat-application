using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CrossChat.DataAccess;
using CrossChat.Domain.Common;
using CrossChat.Domain.Common.constants;
using CrossChat.Domain.DBModel;
using Microsoft.AspNetCore.Http;

namespace CrossChat.Services.UserService
{
    public class UserService : IUserService
    {
		CrossChatContext _context;
		IRepository<User> _userRipository;
		IHttpContextAccessor _httpContextAccessor;
		public UserService(CrossChatContext context, IHttpContextAccessor httpContextAccessor, IRepository<User> userRipository)
		{
			_context = context;
			_userRipository = userRipository;
			_httpContextAccessor = httpContextAccessor;
		}
        public Result<User> Login(string UserName, string Password)
        {
			Result<User> result = new Result<User>();
			try
			{
				var user = _context.Users.Where(a => a.Email == UserName && a.Password == Password).FirstOrDefault();
				if (user != null)
				{
					result.Data = user;
					result.Success = true;
				}
				else {
					throw new Exception("Username or Password is wrong!!");
				}
				
			}
			catch (Exception e)
			{
				Exception temp = e;
				while (temp!=null) {
					result.Messages.Add(temp.Message);
					temp = temp.InnerException;
				}
				result.Message = e.Message;
				result.Success = false;
			}
			return result;
        }
		public Result<User> AddUser(User user)
		{
			Result<User> result = new Result<User>();
			try
			{
				//var newUser = _userRipository.Add(user);
				var newUser = _context.Users.Add(user).Entity;
				_context.SaveChanges();
				result.Data = newUser;
				result.Success = true;
			}
			catch (Exception e)
			{
				Exception temp = e;
				while (temp != null)
				{
					result.Messages.Add(temp.Message);
					temp = temp.InnerException;
				}
				result.Message = e.Message;
				result.Success = false;
			}
			return result;
		}
        public void ResetPassword(string email)
        {
			try {
				email = email.ToLower();
				var user = _context.Users.FirstOrDefault(a => a.Email == email);

				if (user != null) {
					var guid = Guid.NewGuid();
					using (SHA256 sha256Hash = SHA256.Create())
					{
						// ComputeHash - returns byte array  
						byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(guid.ToString()));

						// Convert byte array to a string   
						StringBuilder builder = new StringBuilder();
						for (int i = 0; i < bytes.Length; i++)
						{
							builder.Append(bytes[i].ToString("x2"));
						}
						user.ChangePasswordHashCode = builder.ToString();
						user.SendHashCodeDateTime = DateTime.Now;
					}
					var Request = _httpContextAccessor.HttpContext.Request;
					var path = GetAbsoluteUri()+ "Account/Reset?key="+ user.ChangePasswordHashCode;
					EmailHelper.SendMailAsync(user.Email, user.Surname, path);
					_context.Users.Update(user);
					_context.SaveChanges();
				}
			}
			catch (Exception e) {
				//todo log
			}
        }
		private string GetAbsoluteUri()
		{
			var request = _httpContextAccessor.HttpContext.Request;
			UriBuilder uriBuilder = new UriBuilder();
			uriBuilder.Scheme = request.Scheme;
			uriBuilder.Host = request.Host.Host;
			if(request.Host.Port!=null && request.Host.Port!=80)
			uriBuilder.Port = (int)request.Host.Port;
			
			
			return uriBuilder.Uri.ToString();
		}
		public Result ChangePassword(string oldPassword,string newPassword)
		{
			Result result = new Result();
			try
			{
				if (_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId") != null)
				{
					var userId = new Guid(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);
					var user = _userRipository.GetByID(userId);

					if (user != null && user.Password==oldPassword)
					{
						//var guid = Guid.NewGuid();
						user.Password = newPassword;
						// todo send Email for changed password info 
						_context.Users.Update(user);
						_context.SaveChanges();
						result.Success = true;
					}
					else
					{
						result.Success = false;
						result.Message = "Authorization Failed";
					}
				}
				else {
					result.Message = "Authorization Failed";
					result.Success = false;
				}
			}
			catch (Exception e)
			{
				//todo log
				result.Message = "Internal Server Error. Please retry later.";
				result.Success = false;
			}
			return result;
		}
        public Result<User> GetUserByHashCode(string key)
        {
			Result<User> result = new Result<User>();
			var user = _context.Users.FirstOrDefault(a => a.ChangePasswordHashCode==key );
			if (user != null && user.SendHashCodeDateTime.AddMinutes(30)>=DateTime.Now) 
			{
				result.Data = user;
				result.Success = true;
			} 
			else {
				result.Message = "Key is Not valid or is expired!";
				result.Success = false;
			}
			return result;

		}
        public Result Edit(User user)
        {
			Result result = new Result();
            try
            {
				_userRipository.Edit(user);
				result.Success = true;
				
			}
            catch (Exception e)
            {

				result.Success = false;
				result.Message = "Internal Sercer Error! Try again Later;";
			}
			return result;
			

		}     
		

        public Result<User> GetUserRegisterdUser()
        {
			Result<User> result = new Result<User>();
			if (_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId") != null)
			{
				var userId = new Guid(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);
				var user = _userRipository.GetByID(userId);
				if (user != null)
				{
					result.Success = true;
					result.Data = user;
				}
				else
				{
					result.Success = false;
					result.Message = "User is not exist!";
				}

			}
			else {
				result.Success = false;
				result.Message = "Not Authorized!";
			}
			return result;
		}

        public Result<List<User>> SearchUsers(string key)
        {
			Result<List<User>> result = new Result<List<User>>();
            try
            {
				var selfId = new Guid(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);
				result.Data= _context.Users.Where(
					a => a.Id != selfId &&
					(a.Email.Contains(key) || a.Name.Contains(key) || a.Surname.Contains(key))
				).ToList();
				result.Success = true;
            }
            catch (Exception e)
            {
				result.Success = true;
				result.Message = "Internal server error!";
			}
			return result;
        }


		public Result<User> GetUserById(Guid userId)
		{
			Result<User> result = new Result<User>();
			try
			{

				result.Data = _userRipository.GetByID(userId);
				if (result.Data != null)
				{
					result.Success = true;
				}
				else {
					result.Success = false;
					result.Message = "User is not Exist";
				}
				
			}
			catch (Exception e)
			{
				result.Success = true;
				result.Message = "Internal server error!";
			}
			return result;
		}

	
	}
}
