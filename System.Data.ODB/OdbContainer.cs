using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class OdbContainer : IDbContainer  
    {            
        protected IDbContext DbConext { get; set; }

        public OdbContainer(IDbContext db)
        {
            this.DbConext = db;
        } 
         
        #region ORM        
        public virtual void Create<T>() where T : IEntity
        { 
            this.DbConext.ExecuteCreate<T>();
        }
        
        public virtual void Remove<T>() where T : IEntity
        {
            this.DbConext.ExecuteDrop<T>();
        }
     
        public virtual IList<T> Get<T>() where T : IEntity
        {
            IQuery q = this.Collect<T>();

            return q.ToList<T>();
        }

        public virtual IQuery Collect<T>() where T : IEntity
        { 
            OdbDiagram dg = new OdbDiagram(this.DbConext.Depth);

            Type type = typeof(T);

            dg.Analyze(type);

            OdbTable table = dg.Table[0];

            IQuery q = this.DbConext.Query();

            q.Select(dg.Colums).From(table.Name, table.Alias);

            int i = 1;

            foreach (KeyValuePair<string, string> tc in dg.ForigeKey)
            {
                OdbTable tab = dg.Table[i++];

                q.LeftJoin(tab.Name).As(tab.Alias).On(tc.Key).Equal(tc.Value);
            }

            return q;
        } 

        public virtual int Store<T>(T t)where T : IEntity
        { 
            return this.DbConext.ExecutePersist(t);
        }
 
        public virtual int Delete<T>(T t) where T : IEntity
        { 
            IQuery query = this.DbConext.Query().Delete().From<T>().Where("Id").Eq(t.Id);

            return query.Execute(); 
        }
           
        public virtual void Clear<T>() where T : IEntity
        {
            this.DbConext.Query().Delete().From<T>().Execute();            
        }

        #endregion
         
    }
}
