using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using CrossChat.UI.Infrastructure.Validation;

namespace CrossChat.UI.Controllers
{
    public abstract class BaseController : Controller
    {
        public IMapper Mapper { get; set; }
        public IValidatorFactory ValidatorFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextViewURL"> // i.e. "/Login/Account" </param>
        /// <returns></returns>
        public IActionResult RedirectToNext(string nextViewURL)
        {
            return RedirectToNext(nextViewURL, "Index", "Home");
        }

        public IActionResult RedirectToNext(string nextViewURL, string defaultAction, string defaultController)
        {
            if (nextViewURL == null || "".Equals(nextViewURL))
                return RedirectToAction(defaultAction, defaultController);
            else
                return Redirect(nextViewURL); ////return LocalRedirect(nextViewURL); // NextURL referer ile alındığı için değiştirildi.
        }


        public IActionResult RedirectToHome()
        {
            return RedirectToAction("Index", "Home");
        }


        public IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Membership");
        }

        public bool Validate<T>(T model)
        {
            var validator = ValidatorFactory.GetDefaultValidator<T>();
            var validatorresult = validator.Validate(model);

            validatorresult.Errors.ToList()
                .ForEach(err => ModelState.AddModelError(err.PropertyName, err.ErrorMessage));

            if (validatorresult.IsValid == false)
                ModelState.AddModelError(string.Empty, "Lütfen zorunlu alanları giriniz");

            return validatorresult.IsValid;
        }

    }
}
