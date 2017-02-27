using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker
{
    public class SqliteRepository : IRepository
    {
        private static readonly Lazy<SqliteRepository> lazy =
            new Lazy<SqliteRepository>(() => new SqliteRepository());
        public static SqliteRepository Instance { get { return lazy.Value; } }
        private const string sqliteFileName = "barcode.db";
        string connstring = String.Format("Data Source={0};Pooling=true;FailIfMissing=false", sqliteFileName);

        private SqliteRepository()
        {
        }

        public bool CreateSqliteDb()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connstring))
                {
                    conn.Open();
                    string sql = "CREATE TABLE TBarcode (ID integer primary key autoincrement, Barcode CHAR(20))";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Fatal(ex.Message);
                return false;
            }

        }

        public bool Add(string barcode)
        {
            throw new NotImplementedException();
        }

        public bool CheckDuplicate(string barcode)
        {
            throw new NotImplementedException();
        }
    }
}
