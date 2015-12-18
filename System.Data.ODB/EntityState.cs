using System; 
using System.Reflection;

namespace System.Data.ODB
{
    public class EntityState
    {
        public IEntity OriginalValue { get; private set; }

        private PropertyInfo[] propertys;
        
        public EntityState(IEntity entity)
        {
            this.OriginalValue = entity.Copy();

            Type type = this.OriginalValue.GetType();

            this.propertys = type.GetProperties();
        }

        public bool IsModified(string name, object val)
        {
            foreach (PropertyInfo pi in this.propertys)
            {
                if (pi.Name == name)
                {
                    return !pi.GetValue(this.OriginalValue, null).Equals(val);
                }
            }

            return false;
        }
    }
}
