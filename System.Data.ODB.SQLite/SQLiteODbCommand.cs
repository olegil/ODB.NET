using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteODbCommand : OdbCommand
    {
        public SQLiteODbCommand(IDbContext db) : base(db)
        {
        }

        public override int InsertReturnId<T>(T t)
        {
            if (this.Insert(t) > 0)
                return (int)(this.Db.Connection as SQLiteConnection).LastInsertRowId;

            return -1;
        }
    }
}
