using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace DatabaseExtension {
    public interface IDbQuery:IDisposable {
        IDbConnection Connection {
            get;
        }
        IDbTransaction BeginTransaction();
        SqlResult GetSqlResult(string sql, IDictionary<string, object> param);
        int ExecuteNonQuery(string sql, IDictionary<string, object> param);
        object ExecuteScalar(string sql, IDictionary<string, object> param);
        System.Data.DataTable GetDataTable(string sql, IDictionary<string, object> param);
    }
}
