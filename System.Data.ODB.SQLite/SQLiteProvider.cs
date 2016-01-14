﻿using System.Collections.Generic;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.ODB.SQLite
{
    public class SQLiteProvider : QueryProvider
    {
        public SQLiteProvider(IDbContext db) : base(db)
        {
        }

        public override object Execute(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            IQueryParser parser = new SQLiteParser();
            
            parser.Translate(expression, Db.Depth);

            IDataReader sr = this.Db.ExecuteReader(parser.ToString(), parser.GetParamters());

            return Activator.CreateInstance(typeof(EntityReader<>).MakeGenericType(elementType), new object[] { sr, Db.Depth });
        }

        public override string GetSQL(Expression expression)
        {
            IQueryParser parser = new SQLiteParser();

            parser.Translate(expression, Db.Depth);

            return parser.ToString();
        } 
    }
}
