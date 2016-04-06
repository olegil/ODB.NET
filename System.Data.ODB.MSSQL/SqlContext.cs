﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace System.Data.ODB.MSSQL
{
    public class SqlContext : DbContext
    {
        public SqlContext(string conn)
            : base(new SqlConnection(conn))
        {        
        }

        public override IQuery<T> Query<T>()
        {
            return new SqlQuery<T>(this);
        }
               
        public override long InsertReturnId<T>(T t)
        {
            if (this.Insert(t) > 0)
            {
                string table = MappingHelper.GetTableName(t.GetType());

                return this.ExecuteScalar<long>(string.Format("SELECT Id FROM {0} WHERE Id = SCOPE_IDENTITY();", table), null);
            } 

            return -1;
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

                if (colAttr != null)
                { 
                    if (!colAttr.IsForeignkey)
                    {
                        dbtype = this.TypeMapping(pi.PropertyType);
                    }
                    else
                    {
                        dbtype = this.TypeMapping(typeof(long));

                        this.Create(pi.PropertyType);
                    }

                    cols.Add(this.Define(pi.Name, dbtype, colAttr));
                }
            }
 
            cols.Add(string.Format("CONSTRAINT [PK_dbo.{0}] PRIMARY KEY (Id)", type.Name));

            return this.Create(type.Name, cols.ToArray());
        }

        public virtual int Create(string table, string[] cols)
        {
            string sql = "CREATE TABLE IF NOT EXISTS \"" + table + "\" (\r\n" + string.Join(",\r\n", cols) + "\r\n);";

            return this.ExecuteNonQuery(sql);
        }

        public virtual int Drop(string table)
        {
            return this.ExecuteNonQuery("DROP TABLE IF EXISTS " + table);
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

            return this.Drop(type.Name);
        }

        public override int Remove<T>()
        {
            Type type = typeof(T);

            return this.Drop(type);
        }

        public string Define(string name, string dbtype, ColumnAttribute colAttr)
        {
            string col = "[" + name + "] " + dbtype;

            if (colAttr.Length > 0)
                col += "(" + colAttr.Length + ")";
            else
                col += "(MAX)";

            if (colAttr.IsAuto)
            {
                col += " IDENTITY(1,1)";
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
                return "NVARCHAR";
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
                return "BIGINT";
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
                return "BIT";
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

            this.SetCommand(cmd, sql, commandParameters);

            //create the DataAdapter & DataSet 
            SqlDataAdapter da = new SqlDataAdapter(cmd as SqlCommand);

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
    }
}