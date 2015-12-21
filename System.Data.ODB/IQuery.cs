using System; 
using System.Collections.Generic;

namespace System.Data.ODB
{
    public interface IQuery
    {
        IQuery Create();
        IQuery Drop();
        IQuery Select(string[] cols);
        IQuery From();
        IQuery Insert(string[] cols);
        IQuery Update();
        IQuery Delete();
        IQuery Values(string[] cols);
        IQuery Set(string[] cols);
        IQuery Where(string str);
        IQuery And(string str);
        IQuery Or(string str);
        IQuery OrderBy(string str);
        IQuery Eq(object val);
        IQuery NotEq(object val);
        IQuery Gt(object val);
        IQuery Lt(object val);
        IQuery Gte(object val);
        IQuery Lte(object val);
        IQuery Like(string str);
        IQuery Count();
        IQuery Skip(int start);
        IQuery Take(int count);
        IQuery Join<T>() where T : IEntity;
        IQuery LeftJoin<T>() where T : IEntity;
        IQuery As(string str);
        IQuery On(string str);
        IQuery Equal(string str);
        IQuery SortAsc();
        IQuery SortDesc();

        IQuery Append(string str);

        IDbDataParameter BindParam(string name, object b, ColumnAttribute attr);

        List<IDbDataParameter> Parameters { get; set; }

        T First<T>() where T : IEntity;

        List<T> ToList<T>() where T : IEntity;

        string ToString();
    }
}
