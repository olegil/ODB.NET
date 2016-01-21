using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class EntityReader<T> : IEnumerable<T>, IDisposable where T : IEntity
    {
        private IDataReader sr;

        private bool disposed = false;

        public int Level { get; private set; }
    
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
                int index = 0;

                object b = this.getEntry(type, index);               
                
                yield return (T)b;
            }

            this.Dispose();          
        }

        private object getEntry(Type type, int index)
        {
            object instance = Activator.CreateInstance(type);  

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                if (attr != null)
                {
                    if (attr.IsForeignkey)
                    {
                        if (this.Level > 1)
                        {
                            this.Level--;

                            index++;

                            object b = this.getEntry(pi.PropertyType, index);

                            this.Level++;

                            if ((b as IEntity).Id != 0)
                                pi.SetValue(instance, b, null);
                            else
                                pi.SetValue(instance, null, null);
                        }
                    }
                    else 
                    { 
                        string colName = "T" + index + "." + pi.Name; 

                        object value = this.sr[colName] == DBNull.Value ? null : this.sr[colName];

                        pi.SetValue(instance, value, null);                        
                    }
                  
                }

                //if (pi.Name == "IsPersisted")
                //{
                //    pi.SetValue(instance, true, null);
                //}
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
