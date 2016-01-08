using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class EntityReader<T> : IEnumerable<T>, IDisposable where T : IEntity
    {
        private IDataReader sr;

        private bool disposed = false;

        private int n = 0;

        public EntityReader(IDataReader reader)
        {
            this.sr = reader;
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
                int level = 0;

                object b = this.getEntry(sr, type, level);               
                
                yield return (T)b;
            }

            this.Dispose();          
        }

        private object getEntry(IDataReader sr, Type type, int level)
        {
            object instance = Activator.CreateInstance(type);  
            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                if (attr != null)
                {
                    if (!attr.IsForeignkey)
                    {
                        string colName = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;

                        colName = "T" + level + "." + colName; 

                        object value = this.sr[colName] == DBNull.Value ? null : this.sr[colName];

                        pi.SetValue(instance, value, null);                        
                    }
                    else
                    {
                        //object b = this.getEntry(sr, pi.PropertyType);

                        //object b = Activator.CreateInstance(pi.PropertyType);  

                        //pi.SetValue(instance, b, null);
                    }
                }

                if (pi.Name == "IsPersisted")
                {
                    pi.SetValue(instance, true, null);
                }
            }

            return instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        } 
    }
}
