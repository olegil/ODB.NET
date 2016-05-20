using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.ODB
{
    public class OdbWriter
    {
        private int level;
        private IContext _db;

        public OdbWriter(IContext db)
        {
            this._db = db;
            this.level = 1;
        }

        public int Write<T>(T t) where T : IEntity
        {
            Type type = t.GetType();

            string table = OdbMapping.GetTableName(type);

            IQuery query = this._db.Query();

            int n = 0;

            List<string> cols = new List<string>();
            List<string> ps = new List<string>();
 
            //begin foreach
            foreach (OdbColumn col in OdbMapping.GetColumns(type))
            {
                ColumnAttribute attr = col.Attribute;

                if (!attr.IsAuto)
                {
                    object b = col.GetValue(t);

                    if (!attr.IsModel)
                    {
                        if (b == null)
                            b = DBNull.Value;
                    }
                    else
                    {
                        if (this.level < this._db.Depth)
                        {
                            if (b != null)
                            {                                 
                                this.level++;

                                //return id
                                b = this.Write(b as IEntity);

                                this.level--;
                            }
                        }
                        else
                        {
                            //if not update
                            if (b != null)
                            {
                                b = (b as IEntity).Id;
                            }                        
                        }
                    }

                    string name = "@p" + n;

                    IDbDataParameter p = this._db.CreateParameter();

                    p.ParameterName = name;
                    p.Value = b;
                    p.DbType = col.GetDbType();

                    query.Parameters.Add(p);

                    ps.Add(name);
                    cols.Add("[" + col.Name + "]");

                    n++;
                } 
            }
            //end 

            if (t.Id != 0)
            {
                for (int i = 0; i < cols.Count; i++)
                {
                    cols[i] = cols[i] + "=" + ps[i];
                }

                query.Update(table).Set(cols.ToArray()).Where("Id").Eq(t.Id);

                query.Execute();

                return t.Id;
            }
            else
            {
                query.Insert(table, cols.ToArray()).Values(ps.ToArray());

                return query.ExecuteReturnId();
            }
        }
    }
}
