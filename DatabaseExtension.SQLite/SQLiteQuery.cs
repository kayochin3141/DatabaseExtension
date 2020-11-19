using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using DatabaseExtension;
namespace DatabaseExtension.SQLite
{
    public class SQLiteQuery : IDbQuery {
        private bool disposedValue;

        public SQLiteQuery():this(_connectionString) {

        }
        public SQLiteQuery(string connectionString) {
            this.Connection = new SQLiteConnection(connectionString);
            this.Connection.Open();
        }
        private static string _connectionString = null;
        public static void SetConnectionString(string connectionString) {
            _connectionString = connectionString;
        }
        public IDbConnection Connection {
            get;
        }
        private SQLiteConnection SQLiteConnection {
            get {
                return (SQLiteConnection)this.Connection;
            }
        }


        public IDbTransaction BeginTransaction() {
            return this.Connection.BeginTransaction();
        }
        private SQLiteCommand GenerateCommand(string sql, IDictionary<string, object> param) {
            var cmd = this.SQLiteConnection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            if (param != null) {
                foreach(var p in param) {
                    cmd.Parameters.Add(new SQLiteParameter(p.Key, p.Value));
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

        public DataTable GetDataTable(string sql, IDictionary<string, object> param) {
            using (var cmd = GenerateCommand(sql, param))
            using (var dr = cmd.ExecuteReader()) {
                var dt = new System.Data.DataTable();
                dt.Load(dr);
                return dt;
            }
        }

        public SqlResult GetSqlResult(string sql, IDictionary<string, object> param) {
            using(var cmd = GenerateCommand(sql,param))
            using(var dr = cmd.ExecuteReader()) {
                return new SqlResult(dr);
            }
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
        // ~SQLiteQuery()
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
