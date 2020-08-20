using System;
using System.Collections.Generic;
using System.Text;
using CrossChat.Domain.DBModel;

namespace CrossChat.Services
{

        public interface IRepository<T> where T : BaseEntity
        {
            T Add(T entity);
            List<T> GetAll();

            T GetByID(object id);
            bool Edit(T entity);
            bool Delete(T entity);
            bool Delete(object id);
            void Save();
            void Dispose();
        }
    
}
