using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace UnifaceLibrary
{
    public class UnifaceDatabase : IDisposable
    {
        private SqlConnection _connection;

        public UnifaceDatabase(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public void PullObject(UnifaceObjectId objectId, DirectoryInfo codeRootDirectory)
        {
            UnifacePullObject.PullObject(_connection, objectId, codeRootDirectory);
        }

        // public void PushObject(UnifaceObjectId objectId, DirectoryInfo codeRootDirectory)
        // {
        //     UnifacePushObject.PushObject(_connectionString, objectId, codeRootDirectory);
        // }

        public IEnumerable<UnifaceObject> GetAllObjects()
        {
            foreach (var type in UnifaceObjectType.All)
            {
                var tableSource = type.TableSource;
                var command = new SqlCommand($"SELECT * FROM {tableSource.PrimaryTable} WHERE {tableSource.TypeFilter}", _connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        yield return GetUnifaceObject(reader, type);
                }
            }
        }

        public IEnumerable<UnifaceObjectChange> GetAllObjectsChangedSince(int sinceVersion)
        {
            foreach (var type in UnifaceObjectType.All)
            {
                var tableSource = type.TableSource;
                var command = new SqlCommand(
                    $"SELECT CT.SYS_CHANGE_OPERATION AS SYS_CHANGE_OPERATION, {tableSource.PrimaryTable}.* FROM {tableSource.PrimaryTable} " + 
                    $"INNER JOIN CHANGETABLE(CHANGES {tableSource.PrimaryTable}, {sinceVersion}) CT ON {tableSource.PrimaryTable}.{tableSource.IdField} = CT.{tableSource.IdField} " +
                    $"WHERE {tableSource.TypeFilter}",
                    _connection);

                //Console.WriteLine(command.CommandText);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        yield return new UnifaceObjectChange(
                            GetUnifaceObject(reader, type),
                            (ChangeOperation)reader["SYS_CHANGE_OPERATION"].ToString());
                }
            }
        }

        private static void EnsureSqlServerChangeTrackingEnabled(SqlConnection connection)
        {
            // check
            throw new InvalidOperationException($"Change tracking was not enabled for '{connection.ConnectionString}'");
        }

        private static UnifaceObject GetUnifaceObject(SqlDataReader reader, UnifaceObjectType type)
        {
            return new UnifaceObject(GetUnifaceObjectId(reader, type));
        }

        private static UnifaceObjectId GetUnifaceObjectId(SqlDataReader reader, UnifaceObjectType type)
        {
            return new UnifaceObjectId(type, reader[type.TableSource.LibraryField].ToString(), reader[type.TableSource.IdField].ToString());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
