using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class EntityReader<T> : IEnumerable<T>, IDisposable where T : IEntity
    {
        private IDataReader sr;

        private bool disposed = false;

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
            PropertyInfo[] propertys = typeof(T).GetProperties();

            while (this.sr.Read())
            {
                T instance = Activator.CreateInstance<T>();

                string colName;

                foreach (PropertyInfo pi in propertys)
                {
                    ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                    if (attr != null && !attr.IsForeignkey)
                    {
                        colName = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;

                        object value = this.sr[colName] == DBNull.Value ? null : this.sr[colName];

                        pi.SetValue(instance, value, null);
                    }

                    if (pi.Name == "IsPersisted")
                    {
                        pi.SetValue(instance, true, null);
                    }                    
                } 

                yield return instance;
            }

            this.Dispose();          
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        } 
    }
}
