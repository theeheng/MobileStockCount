using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using SQLite.Net;
using SQLite.Net.Interop;

namespace DomainInterface
{
    public interface IDatabase
    {

        TimeSpan BusyTimeout { get; set; }
        string DatabasePath { get; }
        IDictionary<Type, string> ExtraTypeMappings { get; }
        IDbHandle Handle { get; }
        bool IsInTransaction { get; }
        ISQLitePlatform Platform { get; }
        IBlobSerializer Serializer { get; }
        bool StoreDateTimeAsTicks { get; }
        IEnumerable<TableMapping> TableMappings { get; }
        bool TimeExecution { get; set; }
        ITraceListener TraceListener { get; set; }

        void BeginTransaction();
        void Close();
        void Commit();
        SQLiteCommand CreateCommand(string cmdText, params object[] args);
        void CreateIndex<T>(Expression<Func<T, object>> property, bool unique = false);
        int CreateIndex(string tableName, string columnName, bool unique = false);
        int CreateIndex(string tableName, string[] columnNames, bool unique = false);
        int CreateIndex(string indexName, string tableName, string columnName, bool unique = false);
        int CreateIndex(string indexName, string tableName, string[] columnNames, bool unique = false);
        int CreateTable<T>(CreateFlags createFlags = CreateFlags.None);
        int CreateTable(Type ty, CreateFlags createFlags = CreateFlags.None);
        IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new();
        IEnumerable<object> DeferredQuery(TableMapping map, string query, params object[] args);
        int Delete<T>(object primaryKey);
        int Delete(object objectToDelete);
        int DeleteAll<T>();
        void Dispose();
        int DropTable<T>();
        void EnableLoadExtension(int onoff);
        int Execute(string query, params object[] args);
        T ExecuteScalar<T>(string query, params object[] args);
        T Find<T>(Expression<Func<T, bool>> predicate) where T : new();
        T Find<T>(object pk) where T : new();
        object Find(object pk, TableMapping map);
        T Get<T>(Expression<Func<T, bool>> predicate) where T : new();
        T Get<T>(object pk) where T : new();
        TableMapping GetMapping<T>();
        TableMapping GetMapping(Type type, CreateFlags createFlags = CreateFlags.None);
        List<SQLiteConnection.ColumnInfo> GetTableInfo(string tableName);
        int Insert(object obj);
        int Insert(object obj, string extra);
        int Insert(object obj, Type objType);
        int Insert(object obj, string extra, Type objType);
        int InsertAll(IEnumerable objects);
        int InsertAll(IEnumerable objects, string extra);
        int InsertAll(IEnumerable objects, Type objType);
        int InsertOrReplace(object obj);
        int InsertOrReplace(object obj, Type objType);
        int InsertOrReplaceAll(IEnumerable objects);
        int InsertOrReplaceAll(IEnumerable objects, Type objType);
        List<T> Query<T>(string query, params object[] args) where T : new();
        List<object> Query(TableMapping map, string query, params object[] args);
        void Release(string savepoint);
        void Rollback();
        void RollbackTo(string savepoint);
        void RunInTransaction(Action action);
        string SaveTransactionPoint();
        TableQuery<T> Table<T>() where T : new();
        int Update(object obj);
        int Update(object obj, Type objType);
        int UpdateAll(IEnumerable objects);
    }
}