﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.ODB.Linq;
using System.Data.ODB;
using System.Linq.Expressions;

namespace System.Data.ODB.SQLite
{
    public class SQLiteExpression<T> : QueryExpression<T>
        where T : IEntity
    { 
        public SQLiteExpression(IQuery<T> query) : base(query)
        {            
        }

        public override void Translate(Expression expression)
        {
            this.Visit(expression);                   
        }
    }
}
