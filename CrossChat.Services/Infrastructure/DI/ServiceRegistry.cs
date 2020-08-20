
using Microsoft.EntityFrameworkCore;
using StructureMap;
using CrossChat.DataAccess;

namespace CrossChat.ServiceCore.Infrastructure.DI
{
    public class ServiceRegistry : Registry
    {
        public ServiceRegistry(string connectionString)
        {
            //var optionsBuilder = new DbContextOptionsBuilder<DBContext>();

          
            //    optionsBuilder.UseSqlServer(connectionString);
           
            //For<IDBContext>().Use<DBContext>()
            //          .Ctor<DbContextOptions<DBContext>>("options")
            //                      .Is(optionsBuilder.Options)
            //                      .SelectConstructor(() => new DBContext(null));

            


        }
    }
}
