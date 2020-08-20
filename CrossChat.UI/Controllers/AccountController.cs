using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using CrossChat.Domain.Common.CustomClass;
using System.Security.Claims;
using CrossChat.Services.JwtService;
using CrossChat.Services;
using CrossChat.Domain.DBModel;
using CrossChat.Services.UserService;
using System.Net.Mail;
using CrossChat.Domain.Common;
using CrossChat.UI.ViewModels.User;

namespace CrossChat.UI.Controllers
{
    public class AccountController : Controller
    {
        IRepository<User> _repository;
        IUserService _userService;
        public AccountController(IRepository<User> repository, IUserService userService)
        {

            _repository = repository;
            _userService = userService;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (TempData["success"] != null && (bool)TempData["success"] == true)
            {
                ViewBag.SuccessMessage = TempData["Message"].ToString();
            }
            return View();

        }

        /// <summary>
        /// Login and generate Token object
        /// </summary>
        /// <param name="model">Email and password</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginPost(User model)
        {
            model.Id = Guid.NewGuid();
            var result = _userService.Login(model.Email, model.Password);
            if (result.Success)
            {
                var claims = new List<Claim>
                    {
                        new Claim("UserId", result.Data.Id.ToString())
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(90),
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("index", "home");
            }
            else
            {
                ViewBag.error = result.Message;
                return RedirectToAction("login", "account");
            }

        }
        /// <summary>
        /// Request of reset password
        /// </summary>
        /// <param name="Email">Email address</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public JsonResult ResetPassword(string Email)
        {
            Result result = new Result();
            MailAddress email;
            try
            {
                email = new MailAddress(Email);

            }
            catch (FormatException)
            {
                result.Success = false;
                result.Message = "Email format is not correct";
                return new JsonResult(result);
            }
            _userService.ResetPassword(Email);
            result.Success = true;
            return new JsonResult(result);

        }
        /// <summary>
        /// Reset Password change
        /// </summary>
        /// <param name="key">Key is an hashcode that sended by Email when requested reset password</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        
        public IActionResult Reset(string key)
        {
            //Result result = new Result();
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            var user = _userService.GetUserByHashCode(key);
            if (user.Success)
            {
                user.Data.Password = "";
                user.Data.ConfirmPassowrd = "";
                return View(user.Data);
            }
            else
            {
                ViewBag.Error = "Key is not valid!";
                return View();

            }
        }
        /// <summary>
        /// Reseting the password
        /// </summary>
        /// <param name="ChangePasswordHashCode"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ResetFinish(string ChangePasswordHashCode, string Password)
        {

            var user = _userService.GetUserByHashCode(ChangePasswordHashCode);

            if (user.Success)
            {
                user.Data.Password = Password;
                user.Data.ChangePasswordHashCode = null;
                var result = _userService.Edit(user.Data);
                if (result.Success)
                {
                    TempData["message"] = "Password successfully changed!";
                    TempData["success"] = true;
                    return Redirect("/Account/Login");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;

                    return RedirectToAction("Reset", "account", new { key = ChangePasswordHashCode });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Key is not valid!";
                return RedirectToAction("Reset", "account", new { key = ChangePasswordHashCode });

            }
        }

        /// <summary>
        /// change password rquest 
        /// </summary>
        /// <param name="oldPassword">old password</param>
        /// <param name="newPassword">new password</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ChangePassword(string oldPassword, string newPassword)
        {
            Result result = new Result();
         
            result = _userService.ChangePassword(oldPassword, newPassword);

            return new JsonResult(result);

        }
        /// <summary>
        /// Register new user and automatic login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Register(User model)
        {
            model.Id = Guid.NewGuid();
            var result = _userService.AddUser(model);
            
            if (result.Success)
            {
                var register=_userService.Login(result.Data.Email, result.Data.Password);


                var claims = new List<Claim>
                    {
                        new Claim("UserId", register.Data.Id.ToString())
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(90),
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.error = result.Message;
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {

            var returnTo = "/Account/Login";
            try
            {
                var result = _userService.Login(model.UserName, model.Password);
                if (result.Success)
                {

                    var claims = new List<Claim>
                    {
                        new Claim("UserId", result.Data.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(90),
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    returnTo = "/Home/Index";
                }
                else
                {
                    return RedirectToAction("Login", "account"); // TODO Error message
                }

            }
            catch (Exception e)
            {
                //var Roles = _roleServices.GetRules();
                //ViewBag.Error = e.Message;
                return RedirectToAction("Login", "account");// TODO Error message
            }

            return RedirectToLocal(returnTo);

        }
        [Authorize]

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private static IEnumerable<Claim> AddMyClaims(UserClaimsInfo authenticatedUser)
        {
            var myClaims = new List<Claim>
            {
                new Claim("UserId", authenticatedUser.UserId)
                //,
                //new Claim("HasAdminRights", authenticatedUser.HasAdminRights ? "Y" : "N")
            };

            return myClaims;
        }

        /// <summary>
        /// get the current user data
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult GetCurrentUser()
        {
            Result<User> result = new Result<User>();
           
          
            result = _userService.GetUserRegisterdUser();
            if (result.Success)
            {
                result.Data.Password = "";
                result.Data.ChangePasswordHashCode = "";
                result.Data.ConfirmPassowrd = "";
            }
            return new JsonResult(result);

        }
        /// <summary>
        /// edit current user
        /// </summary>
        /// <param name="user">object of user</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult EditProfile(User user)
        {
            Result<User> result = new Result<User>();
            result = _userService.GetUserRegisterdUser();
            if (result.Success)
            {
                result.Data.Name = user.Name;
                result.Data.Surname = user.Surname;
                result.Data.Email = user.Email;
                result.Data.Avatar = user.Avatar;
                var edit = _userService.Edit(result.Data);
                if (edit.Success)
                {
                    result.Data.Password = "";
                    result.Data.ConfirmPassowrd = "";
                    result.Data.ChangePasswordHashCode = "";
                    result.Data.SendHashCodeDateTime = new DateTime();
                    result.Data.Id = new Guid();
                    return new JsonResult(result);
                }
                else {
                    return new JsonResult(edit);
                }
            }
            else {
                return new JsonResult(result);
            }


        }

    }
}