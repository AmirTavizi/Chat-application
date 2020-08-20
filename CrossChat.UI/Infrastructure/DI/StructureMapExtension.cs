
using CrossChat.UI.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using CrossChat.ServiceCore.Infrastructure.DI;

namespace CrossChat.UI.Infrastructure
{
    public static class StructureMapExtension
    {
        public static IContainer AddStructureMap(this IServiceCollection services, IConfiguration configuration)
        {
            var container = new Container();
            
                container.Configure(config =>
                {
                    config.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.Assembly("CrossChat.Services");    
                    x.Assembly("CrossChat.DataAccess");
                    
                    x.WithDefaultConventions();
                });
                    
                config.AddRegistry<AutoMapperRegistry>();
                config.AddRegistry(new UIRegistry(configuration));
                //config.Policies.SetAllProperties(y => y.OfType<ISessionManager>());
                

                var connectionString = configuration.GetConnectionString("CrossChatDBContext");
                config.AddRegistry(new ServiceRegistry(connectionString));

                config.Populate(services);
            });


            var configText = container.WhatDoIHave();
            return container;
        }


    }
}
