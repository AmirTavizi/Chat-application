using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using CrossChat.UI.Infrastructure.Validation;
using StructureMap;

namespace CrossChat.UI.DI
{
    public class UIRegistry : Registry
    {
        public UIRegistry(IConfiguration configuration)
        {
            //For<IControllerManager>().Use<ControllerManager>();

            //For<ISessionManager<UserSessionInfo>>().Use<UserSessionManager>();

            


            //Policies.SetAllProperties(y => y.OfType<IControllerManager>());

            For<IMapper>().Use<Mapper>();
            Policies.SetAllProperties(y => y.OfType<IMapper>());

            For<IValidatorFactory>().Use<ValidatorFactory>();
            Policies.SetAllProperties(y => y.OfType<IValidatorFactory>());

            For<IConfiguration>().Use(x => configuration);

            // external login için
            //For<IUserStore<ApplicationUser>>().Use<EmptyUserStore<ApplicationUser>>();
            //For<IRoleStore<IdentityRole>>().Use<EmptyRoleStore<IdentityRole>>();


        }
    }
}
