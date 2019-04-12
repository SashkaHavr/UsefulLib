using System;
using System.Reflection;

namespace UsefulLib.DataAccess
{
    class Singleton<T>
    {
        static T entity;
        public static T GetEntity()
        {
            if (entity != null)
                return entity;
            else
            {
                var ctor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
                return entity = (T)ctor.Invoke(null);
            }
        }
    }
}
