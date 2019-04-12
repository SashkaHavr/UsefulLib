using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace UsefulLib.DataAccess.DB
{
    public class EfRepository<C, T> : IRepository<T> where C : DbContext where T : class, IIdEntity
    {
        DbSet<T> set;

        public void Add(T obj) => set.Add(obj);

        public void Delete(int id)
        {
            T obj = Get(id);
            foreach (var prop in typeof(T).GetProperties())
                if (!prop.PropertyType.IsPrimitive)
                    foreach (var propProp in prop.PropertyType.GetProperties())
                        if (propProp.PropertyType == typeof(ICollection<T>))
                        {
                            var value = prop.GetValue(obj);
                            if(value!=null)
                                (propProp.GetValue(value) as ICollection<T>).Remove(obj);
                        }
            set.Remove(obj);
        }

        public T Get(int id)
        {
            foreach (var entity in set)
                if (entity.Id == id)
                    return entity;
            throw new ArgumentException();
        }

        public IEnumerable<T> GetAll() => set.ToList();

        public void Load(string dataSource = "")
        {
            set = (DbSet<T>)Singleton<C>.GetEntity().GetType().GetProperties().Where(p => p.PropertyType == typeof(DbSet<T>)).Single().GetValue(Singleton<C>.GetEntity());
        }

        public void Save() => Singleton<C>.GetEntity().SaveChanges();

        public void Update(T obj) => throw new NotImplementedException("Update method call is not required");
    }
}
