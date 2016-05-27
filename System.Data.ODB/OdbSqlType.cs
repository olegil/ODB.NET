using System.Reflection;

namespace System.Data.ODB
{
    public class OdbSqlType
    {
        public static DbType Convert(Type t)
        { 
            if (t == OdbType.Text)
            {
                return DbType.String;
            }
            else if (t == OdbType.Char)
            {
                return DbType.StringFixedLength;
            }
            else if (t == OdbType.Byte) 
            {
                return DbType.Byte;
            }
            else if (t == OdbType.Bytes) 
            {
                return DbType.Binary;
            }
            else if (t == OdbType.SByte) 
            {
                return DbType.SByte;
            }
            else if (t == OdbType.Int32) 
            {
                return DbType.Int32;
            }
            else if (t == OdbType.UInt32) 
            {
                return DbType.UInt32;
            }
            else if (t == OdbType.Short) 
            {
                return DbType.Int16;
            }
            else if (t == OdbType.UShort) 
            {
                return DbType.UInt16;
            }
            else if (t == OdbType.Int64) 
            {
                return DbType.Int64;
            }
            else if (t == OdbType.UInt64) 
            {
                return DbType.UInt64;
            }
            else if (t == OdbType.Double) 
            {
                return DbType.Double;
            }
            else if (t == OdbType.Single)  
            {
                return DbType.Single;
            }
            else if (t == OdbType.Decimal) 
            {
                return DbType.Decimal;
            }
            else if (t == OdbType.Bool) 
            {
                return DbType.Boolean;
            }
            else if (t == OdbType.DateTime || t == OdbType.NullableDateTime) 
            {
                return DbType.DateTime;
            }
            else if (t == OdbType.Guid) 
            {
                return DbType.Guid;
            }
            else if (OdbType.OdbEntity.IsAssignableFrom(t))
            {
                return DbType.Int64;
            }
            
            return DbType.String;
        } 
    }
}
