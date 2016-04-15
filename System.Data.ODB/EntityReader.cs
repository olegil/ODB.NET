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
                object b = this.getEntry(type, 0);               
                
                yield return (T)b;
            }

            this.Dispose();          
        }

        private object getEntry(Type type, int index)
        { 
            object instance = Activator.CreateInstance(type);  

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = OdbMapping.GetColAttribute(pi);

                if (!colAttr.NotMapped)
                {
                    if (!colAttr.IsForeignkey)
                    {
                        string colName = string.IsNullOrEmpty(colAttr.Name) ? pi.Name : colAttr.Name;
                        string col = "T" + index + "." + colName;

                        object value = this.sr[col] == DBNull.Value ? null : this.sr[col];

                        pi.SetValue(instance, value, null);
                    }
                    else
                    {  
                        if (this.Level > 1)
                        {
                            this.Level--;

                            int next = index + 1;

                            object b = this.getEntry(pi.PropertyType, next);

                            this.Level++;
 
                            if ((b as IEntity).Id != 0)
                                pi.SetValue(instance, b, null);
                            else
                                pi.SetValue(instance, null, null);
                        }
                    }                                    
                }                  
               
                if (pi.Name == "IsPersisted")
                {
                    pi.SetValue(instance, true, null);
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
