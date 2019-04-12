using System.Collections.Generic;

namespace UsefulLib.DataAccess
{
    public interface IRepository<T> where T : IIdEntity
    {
        void Load(string dataSource = "");
        void Save();
        IEnumerable<T> GetAll();
        T Get(int id);
        void Add(T obj);
        void Update(T obj);
        void Delete(int id);
    }
}
