using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;

namespace System.Data.ODB.SQLite
{
    public class SQLiteContext : DbContext
    {
        public SQLiteContext(string conn)
            : base(new SQLiteConnection(conn))
        {        
        }

        public override IQuery<T> Query<T>()
        {
            return new SQLiteQuery<T>(this);
        } 

        public override int Create<T>()
        {
            Type type = typeof(T);

            return this.Create(type);
        } 

        public virtual int Create(Type type)
        {
            string dbtype = "";

            List<string> cols = new List<string>();

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);
 
                if (!colAttr.NotMapped)
                { 
                    if (!colAttr.IsForeignkey)
                    {
                        dbtype = this.TypeMapping(pi.PropertyType);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(colAttr.Name))
                            colAttr.Name = pi.Name + "Id";

                        dbtype = this.TypeMapping(typeof(long));

                        this.Create(pi.PropertyType);
                    }

                    string colName = string.IsNullOrEmpty(colAttr.Name) ? pi.Name : colAttr.Name;

                    cols.Add(this.Define(colName, dbtype, colAttr));
                }                
            }

            string table = MappingHelper.GetTableName(type);

            return this.Create(table, cols.ToArray());
        }
         
        public virtual int Drop(Type type)
        {
            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null && colAttr.IsForeignkey)
                {
                    this.Drop(pi.PropertyType);
                }
            }

            string table = MappingHelper.GetTableName(type);

            return this.Drop(table);
        }

        public override int Remove<T>()
        {
            Type type = typeof(T);

            return this.Drop(type);
        }

        public string Define(string name, string dbtype, ColumnAttribute colAttr)
        { 
            string col = "[" + name + "] " + dbtype;

            if (colAttr.IsPrimaryKey)
            {
                col += " PRIMARY KEY";
            }

            if (colAttr.IsAuto)
            {
                col += " AUTOINCREMENT";
            }

            if (colAttr.IsNullable)
            {
                col += " NULL";
            }
            else
            {
                col += " NOT NULL";
            }

            return col;
        }

        public string TypeMapping(Type type)
        {
            if (type == DataType.String)
            {
                return "TEXT";
            }
            else if (type == DataType.Char)
            {
                return "CHAR(1)";
            }
            else if (type == DataType.SByte)
            {
                return "TINYINT";
            }
            else if (type == DataType.Short || type == DataType.Byte)
            {
                return "SMALLINT";
            }
            else if (type == DataType.Int32 || type == DataType.UShort)
            {
                return "INT";
            }
            else if (type == DataType.Int64 || type == DataType.UInt32)
            {
                return "INTEGER";
            }
            else if (type == DataType.UInt64)
            {
                return "BIGINT";
            }
            else if (type == DataType.Double)
            {
                return "DOUBLE";
            }
            else if (type == DataType.Float)
            {
                return "REAL";
            }
            else if (type == DataType.Decimal)
            {
                return "NUMERIC(20,10)";
            }
            else if (type == DataType.Bool)
            {
                return "BOOLEAN";
            }
            else if (type == DataType.DateTime)
            {
                return "DATETIME";
            }
            else if (type == DataType.Bytes)
            {
                return "BLOB";
            }
            else if (type == DataType.Guid)
            {
                return "GUID";
            }

            return "TEXT";
        }

        public override DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, this.Connection, this.OdbTransaction, sql, commandParameters);

            //create the DataAdapter & DataSet 
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd as SQLiteCommand);

            DataSet ds = new DataSet();

            try
            {
                //fill the DataSet using default values for DataTable names
                da.Fill(ds);

                cmd.Parameters.Clear();
            }
            catch
            {
                this.Connection.Close();

                throw;
            }
            finally
            {
                da.Dispose();
            }

            //return the dataset
            return ds;
        }

        public override IOdbCommand CreateCommand()
        {
            return new SQLiteODbCommand(this); 
        }
    }
}
