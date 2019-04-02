using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulLib.DataAccess.DB
{
    abstract public class MSSqlConnectedRepositoryBase<T> : IRepository<T> where T : IIdEntity, new()
    {
        abstract protected SqlCommand SelectAllCommand { get; }
        abstract protected SqlCommand SelectOneCommand { get; }
        abstract protected SqlCommand InsertCommand { get; }
        abstract protected SqlCommand UpdateCommand { get; }
        abstract protected SqlCommand DeleteCommand { get; }
        abstract protected string IdParameterName { get; } 
        abstract protected string TableName { get; }
        protected SqlConnection connection;
        protected SqlConnectionStringBuilder csBuilder;
        SqlCommand selectLastId = new SqlCommand($"select IDENT_CURRENT(@table)");

        public MSSqlConnectedRepositoryBase(string initialCatalog)
        {
            selectLastId.Parameters.AddWithValue("@table", TableName);
            csBuilder = new SqlConnectionStringBuilder()
            {
                InitialCatalog = initialCatalog,
                IntegratedSecurity = true,
                ConnectTimeout = 1
            };
        }

        void IRepository<T>.Load(string dataSource) => Connect(dataSource);

        public void Connect(string dataSource)
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
            csBuilder.DataSource = dataSource;
            connection = new SqlConnection(csBuilder.ConnectionString);
            connection.Open();
        }

        void IRepository<T>.Save() => Disconnect();

        public void Disconnect()
        {   
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        void ConnectionCheck()
        {
            if (connection.State != System.Data.ConnectionState.Open)
                throw new Exception("Connection is closed");
        }

        Type objectType = typeof(T);

        T GetObject(SqlDataReader reader)
        {
            T obj = new T();
            for (int i = 0; i < reader.FieldCount; i++)
                objectType.GetProperty(reader.GetName(i)).SetValue(obj, reader.GetValue(i));
            return obj;
        }

        public IEnumerable<T> GetAll()
        {
            ConnectionCheck();
            SelectAllCommand.Connection = connection;
            using (SqlDataReader reader = SelectAllCommand.ExecuteReader())
            {
                List<T> objects = new List<T>();
                while (reader.Read())
                    objects.Add(GetObject(reader));
                return objects;
            }
        }

        public T Get(int id)
        {
            ConnectionCheck();
            SelectOneCommand.Connection = connection;
            SelectOneCommand.Parameters.AddWithValue(IdParameterName, id);
            using (SqlDataReader reader = SelectOneCommand.ExecuteReader())
            {
                SelectOneCommand.Parameters.Clear();
                if (reader.HasRows)
                {
                    reader.Read();
                    return GetObject(reader);
                }
                else
                    throw new IndexOutOfRangeException();
            }
        }

        abstract protected void AddParametrs(T obj, SqlCommand sqlCommand);

        void AddUpdateTemplate(T obj, SqlCommand sqlCommand)
        {
            ConnectionCheck();
            SqlTransaction transaction = connection.BeginTransaction();
            sqlCommand.Connection = connection;
            sqlCommand.Transaction = transaction;
            AddParametrs(obj, sqlCommand);
            try
            {
                sqlCommand.ExecuteNonQuery();
                transaction.Commit();
            }
            catch(Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                sqlCommand.Parameters.Clear();
            }
        }

        public void Add(T obj)
        {
            AddUpdateTemplate(obj, InsertCommand);
            selectLastId.Connection = connection;
            obj.Id = (int)(decimal)selectLastId.ExecuteScalar();
        }

        public void Update(T obj) => AddUpdateTemplate(obj, UpdateCommand);

        public void Delete(int id)
        {
            ConnectionCheck();
            SqlTransaction transaction = connection.BeginTransaction();
            DeleteCommand.Connection = connection;
            DeleteCommand.Transaction = transaction;
            DeleteCommand.Parameters.AddWithValue(IdParameterName, id);
            try
            {
                DeleteCommand.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                DeleteCommand.Parameters.Clear();
            }
        }
    }
}
