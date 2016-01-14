using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class EntityReader<T> : IEnumerable<T>, IDisposable where T : IEntity
    {
        private IDataReader sr;

        private bool disposed = false;

        public int Level { get; set; }
        private int _n = 0;

        public EntityReader(IDataReader reader, int depth)
        {
            this.sr = reader;

            this.Level = depth;
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
                object b = this.getEntry(sr, type);               
                
                yield return (T)b;
            }

            this.Dispose();          
        }

        private object getEntry(IDataReader sr, Type type)
        {
            object instance = Activator.CreateInstance(type);  

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                if (attr != null)
                {
                    if (attr.IsForeignkey)
                    {
                        if (_n < this.Level - 1)
                        {
                            _n++;

                            object b = this.getEntry(sr, pi.PropertyType);

                            _n--;

                            pi.SetValue(instance, b, null);
                        }
                    }
                    else 
                    {
                        string colName = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;

                        colName = "T" + _n + "." + colName; 

                        object value = this.sr[colName] == DBNull.Value ? null : this.sr[colName];

                        pi.SetValue(instance, value, null);                        
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
