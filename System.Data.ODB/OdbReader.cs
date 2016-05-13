using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbReader<T> : IEnumerable<T>, IDisposable where T : IEntity
    {
        private IDataReader sr;

        private bool disposed = false;

        public OdbDiagram Diagram { get; set; }

        private int level;
          
        public OdbReader(IDataReader reader, OdbDiagram diagram)
        {
            this.sr = reader;

            this.Diagram = diagram;

            this.level = 1;
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

            string name = OdbMapping.GetTableName(type);

            OdbTable table = this.Diagram.FindTable(name);

            if (table == null)
                throw new OdbException("Not found table");

            foreach (OdbColumn col in table.Columns)              
            { 
                if (!col.Attribute.IsModel)
                {                   
                    string colName = table.Alias + "." + col.Name;

                    object value = this.sr[colName] == DBNull.Value ? null : this.sr[colName];

                    if (col.Attribute.IsKey)
                        value = Convert.ToInt32(value);

                    col.SetValue(instance as IEntity, value);
                }
                else
                {  
                    if (this.level < OdbConfig.Depth)
                    {
                        this.level++; 

                        object b = this.getEntity(col.GetMapType());

                        this.level--;
 
                        if ((b as IEntity).Id != 0)
                            col.SetValue(instance as IEntity, b);
                    }
                }    
            }

            //type.GetProperty("IsPersisted").SetValue(instance, true, null);

            return instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        } 
    }
}
