using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB.SQLite
{
    public class SQLiteReader<T> : IEnumerable<T> where T : IEntity
    {
        private SQLiteReaderEnumerator enumerator;

        public SQLiteReader(IDataReader reader)
        {
            this.enumerator = new SQLiteReaderEnumerator(reader);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal class SQLiteReaderEnumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private IDataReader _sr;

            private T _current;

            private PropertyInfo[] propertys; 

            public SQLiteReaderEnumerator(IDataReader sr)
            {
                _sr = sr;

                this.propertys = typeof(T).GetProperties();
            }

            public T Current
            {
                get
                {
                    if (_sr == null || _current == null)
                    {
                        throw new InvalidOperationException();
                    }

                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get { return this._current; }
            }

            // Implement MoveNext and Reset, which are required by IEnumerator.
            public bool MoveNext()
            {
                if (this._sr.Read())
                {
                    T instance = Activator.CreateInstance<T>();

                    string colName;

                    foreach (PropertyInfo pi in propertys)
                    {
                        ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                        if (attr != null)
                        {
                            colName = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;

                            object value = this._sr[colName] == DBNull.Value ? null : this._sr[colName];

                            pi.SetValue(instance, value, null);
                        }

                        if (pi.Name == "IsPersisted")
                        {
                            pi.SetValue(instance, true, null);
                        }
                    }

                    //if (IsEntityTracking)
                    //{
                    //    this.DbState.Add(instance.EntityId, new EntityState(instance));
                    //}

                    this._current = instance;

                    return true;
                }

                return false;              
            }

            public void Reset()
            {
                this._current = default(T);
            }

            // Implement IDisposable, which is also implemented by IEnumerator(T).
            private bool disposed = false;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        if (_sr != null)
                        {
                            _sr.Close();
                            _sr.Dispose();
                        }
                    }
                }

                this.disposed = true;
            }
        }
    }
}
