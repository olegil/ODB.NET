using System.Data;
using System.Data.SqlClient;
using System.Data.ODB;

namespace System.Data.ODB.MSSQL
{
    public class SqlOdbCommand : OdbCommand
    {
        public SqlOdbCommand(IDbContext db) : base(db)
        {
        }

        public override int InsertReturnId<T>(T t)
        {
            if (this.Insert(t) > 0)
            {
                string table = MappingHelper.GetTableName(t.GetType());

                return this.Db.ExecuteScalar<int>(string.Format("SELECT Id FROM {0} WHERE Id = SCOPE_IDENTITY();", table), null);
            }

            return -1;
        }
    }
}
