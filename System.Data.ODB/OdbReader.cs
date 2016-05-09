using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbReader<T> : IEnumerable<T>, IDisposable where T : IEntity
    {
        private IDataReader sr;

        private bool disposed = false;

        public int Level { get; private set; }
        public OdbDiagram Diagram { get; set; }
      
        public OdbReader(IDataReader reader, OdbDiagram diagram)
        {
            this.sr = reader;

            this.Diagram = diagram;

            this.Level = diagram.Depth;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.sr != null)
                    {
                        this.sr.Close();
                    }
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            Type type = typeof(T); 

            while (this.sr.Read())
            {
                object b = this.getEntity(type);               
                
                yield return (T)b;
            }

            this.Dispose();          
        }

        private object getEntity(Type type)
        { 
            object instance = Activator.CreateInstance(type);

            string table = OdbMapping.GetTableName(type);

            string alias = this.Diagram.GetAlias(table);

            if (alias == "")
                throw new OdbException("");

            foreach (OdbColumn col in OdbMapping.GetColumn(type))              
            {
                ColumnAttribute colAttr = col.Attribute;
 
                if (!colAttr.IsForeignkey)
                {                   
                    string colName = alias + "." + col.Name;

                    object value = this.sr[colName] == DBNull.Value ? null : this.sr[colName];

                    col.Set(instance as IEntity, value);
                }
                else
                {  
                    if (this.Level > 1)
                    {
                        this.Level--; 

                        object b = this.getEntity(col.GetColumnType());

                        this.Level++;
 
                        if ((b as IEntity).Id != 0)
                            col.Set(instance as IEntity, b);
                        else
                            col.Set(instance as IEntity, null);
                    }
                }    
            }

            type.GetProperty("IsPersisted").SetValue(instance, true, null);

            return instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        } 
    }
}
