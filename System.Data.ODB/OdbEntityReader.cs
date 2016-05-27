using System;
 
namespace System.Data.ODB
{
    public class OdbEntityReader<T> : OdbReader<T> where T : IEntity
    {  
        private int level;
        private OdbDiagram dg;

        public OdbEntityReader(IDataReader reader, OdbDiagram diagram) : base(reader)
        {  
            this.dg = diagram;

            this.level = 1;
        }
   
        public override object GetEntity(Type type)
        { 
            object instance = Activator.CreateInstance(type);
          
            OdbTable table = this.dg.FindTable(type);

            if (table == null)
                throw new OdbException("No table.");

            foreach (OdbColumn col in table.Columns)              
            { 
                if (!col.Attribute.IsModel)
                {                   
                    string colName = table.Alias + "." + col.Name;

                    object value = this.sr[colName] == DBNull.Value ? null : this.sr[colName];

                    if (col.Attribute.IsPrimaryKey)
                        value = Convert.ToInt32(value);

                    col.SetValue(instance as IEntity, value);
                }
                else
                {  
                    if (this.level < this.dg.Depth)
                    {
                        this.level++; 

                        object b = this.GetEntity(col.GetMapType());

                        this.level--;
 
                        if ((b as IEntity).Id != 0)
                            col.SetValue(instance as IEntity, b);
                    }
                }    
            } 

            return instance;
        }         
    }
}
