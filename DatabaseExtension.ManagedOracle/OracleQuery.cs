using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DatabaseExtension;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
namespace DatabaseExtension.ManagedOracle
{
    public class OracleQuery : IDbQuery {
        public OracleConnection Connection {
            get;
        }

        IDbConnection IDbQuery.Connection {
            get;
        }

        private static string _connectionString = null;
        private bool disposedValue;

        public OracleQuery():this(_connectionString) {
        }
        public OracleQuery(string connectionString) {
            this.Connection = new OracleConnection(connectionString);
            this.Connection.Open();
        }
        public static void SetConnectionString(string connectionString) {
            _connectionString = connectionString;
        }
        public IDbTransaction BeginTransaction() {
            return this.Connection.BeginTransaction();
        }
        private OracleCommand GenerateCommand(string sql, IDictionary<string, object> param) {
            var cmd = this.Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.BindByName = true;
            if (param != null) {
                foreach(var p in param) {
                    cmd.Parameters.Add(new OracleParameter(p.Key, p.Value));
                }
            }
            return cmd;
        }
        public int ExecuteNonQuery(string sql, IDictionary<string, object> param) {
            return GenerateCommand(sql, param).ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql, IDictionary<string, object> param) {
            return GenerateCommand(sql, param).ExecuteScalar();
        }
        public DataTable GetDataTable(string sql, IDictionary<string, object> param,int? fetchSize) {
            using (var cmd = GenerateCommand(sql, param))
            using(var dr = cmd.ExecuteReader()) {
                if (fetchSize.HasValue) {
                    var columnCount = dr.FieldCount;
                    dr.FetchSize = fetchSize.Value * columnCount;
                }
                var dt = new System.Data.DataTable();
                dt.Load(dr);
                return dt;
            }
        }
        public DataTable GetDataTable(string sql, IDictionary<string, object> param) {
            return GetDataTable(sql, param, null);
        }

        public SqlResult GetSqlResult(string sql, IDictionary<string, object> param, int? fetchSize) {
            using (var cmd = GenerateCommand(sql, param))
            using (var dr = cmd.ExecuteReader()) {
                if (fetchSize.HasValue) {
                    var columnCount = dr.FieldCount;
                    dr.FetchSize = fetchSize.Value * columnCount;
                }
                return new SqlResult(dr);
            }
        }
        public SqlResult GetSqlResult(string sql, IDictionary<string, object> param) {
            return GetSqlResult(sql, param, null);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }
                try {
                    this.Connection?.Close();
                    this.Connection?.Dispose();
                } catch {
                }
                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~OracleQuery()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
