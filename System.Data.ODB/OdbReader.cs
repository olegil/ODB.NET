using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Data.ODB
{
    public class OdbReader<T> : IEnumerable<T>, IDisposable
    {
        protected IDataReader sr;

        private bool disposed = false;

        public OdbReader(IDataReader reader)
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
                object b = this.GetEntity(type);

                yield return (T)b;
            }

            this.Dispose();
        }

        public virtual object GetEntity(Type type)
        {
            object instance = FormatterServices.GetUninitializedObject(type);

            PropertyInfo[] list = type.GetProperties();

            for(int i = 0; i < list.Length; i++)
            {                  
                list[i].SetValue(instance, this.sr[list[i].Name]);
            }

            return instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
