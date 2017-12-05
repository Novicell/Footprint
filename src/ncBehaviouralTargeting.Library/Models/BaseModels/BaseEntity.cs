using System.Collections.Generic;
using NPoco;

namespace ncBehaviouralTargeting.Library.Models.BaseModels
{
    [PrimaryKey("Id", AutoIncrement = true)]
    internal abstract class BaseEntity<T> : BaseDbEntity
    {
        public int Id { get; set; }

        public void Save(Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    connection.Save<T>(this);
                }
            }
            else
            {
                currentConnection.Save<T>(this);
            }
        }

        public void Delete(Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    connection.Delete<T>(this);
                }
            }
            else
            {
                currentConnection.Delete<T>(this);
            }
        }

        public static T GetById(int id, Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    return connection.SingleOrDefaultById<T>(id);
                }
            }
            return currentConnection.SingleOrDefaultById<T>(id);
        }

        public static IList<T> GetAllLight(Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    return connection.Query<T>().ToList();
                }
            }
            return currentConnection.Query<T>().ToList();
        }

        public static void Save(T model, Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    connection.Save<T>(model);
                }
            }
            else
            {
                currentConnection.Save<T>(model);
            }
        }

        public static void Insert(T model, Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    connection.Insert(model);
                }
            }
            else
            {
                currentConnection.Insert(model);
            }
        }

        public static int Update(T model, Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    return connection.Update(model);
                }
            }
            return currentConnection.Update(model);
        }

        public static int Delete(T model, Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    return connection.Delete(model);
                }
            }
            return currentConnection.Delete(model);
        }

        public static int DeleteById(int id, Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    return connection.Delete<T>(id);
                }
            }
            return currentConnection.Delete<T>(id);
        }
    }
}
