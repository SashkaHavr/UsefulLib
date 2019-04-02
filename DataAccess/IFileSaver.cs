namespace UsefulLib.DataAccess
{
    public interface IFileSaver<T>
    {
        void Save(T obj, string fileName = "Data.bin");
    }
}
