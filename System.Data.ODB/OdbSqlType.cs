using System.Reflection;

namespace System.Data.ODB
{
    public class OdbSqlType
    {
        public static DbType Convert(Type t)
        { 
            if (t == DataType.Text)
            {
                return DbType.String;
            }
            else if (t == DataType.Char)
            {
                return DbType.StringFixedLength;
            }
            else if (t == DataType.Byte) 
            {
                return DbType.Byte;
            }
            else if (t == DataType.Bytes) 
            {
                return DbType.Binary;
            }
            else if (t == DataType.SByte) 
            {
                return DbType.SByte;
            }
            else if (t == DataType.Int32) 
            {
                return DbType.Int32;
            }
            else if (t == DataType.UInt32) 
            {
                return DbType.UInt32;
            }
            else if (t == DataType.Short) 
            {
                return DbType.Int16;
            }
            else if (t == DataType.UShort) 
            {
                return DbType.UInt16;
            }
            else if (t == DataType.Int64) 
            {
                return DbType.Int64;
            }
            else if (t == DataType.UInt64) 
            {
                return DbType.UInt64;
            }
            else if (t == DataType.Double) 
            {
                return DbType.Double;
            }
            else if (t == DataType.Single)  
            {
                return DbType.Single;
            }
            else if (t == DataType.Decimal) 
            {
                return DbType.Decimal;
            }
            else if (t == DataType.Bool) 
            {
                return DbType.Boolean;
            }
            else if (t == DataType.DateTime) 
            {
                return DbType.DateTime;
            }
            else if (t == DataType.Guid) 
            {
                return DbType.Guid;
            }
            else if (DataType.OdbEntity.IsAssignableFrom(t))
            {
                return DbType.Int64;
            }
            
            return DbType.String;
        } 
    }
}
