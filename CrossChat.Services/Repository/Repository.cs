using System;
using System.Collections.Generic;
using System.Text;
using CrossChat.DataAccess;
using CrossChat.Domain.DBModel;
using System.Linq;

namespace CrossChat.Services
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        CrossChatContext context;
        public Repository(CrossChatContext _context)
        {
            context = _context;
        }
        public Repository()
        {

        }
        public T Add(T entity)
        {
            try
            {
                context.Set<T>().AddAsync(entity);
                Save();
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool Delete(T entity)
        {
            try
            {
                var result = GetByID(entity.Id);
                if (result != null)
                {
                    context.Set<T>().Remove(entity);
                    Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool Delete(object id)
        {
            try
            {
                var result = context.Set<T>().Find(id);
                if (result != null)
                {
                    context.Set<T>().Remove(result);
                    Save();
                    return true;
                }
                else
                    return false;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }

        public bool Edit(T entity)
        {
            try
            {
                var exist = context.Set<T>().Find(entity.Id);
                if (exist != null)
                {
                    context.Entry(exist).CurrentValues.SetValues(entity);
                    Save();
                    return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<T> GetAll()
        {
            try
            {
                return context.Set<T>().ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public T GetByID(object id)
        {
            try
            {
                return context.Set<T>().Find(id);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
