using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class Command
    {
        public static string SqliteconnStr = "Data Source={0};Version=3;Pooling=True;Max Pool Size=10;";
        public static string MssqlConnStr = "Data Source=localhost;Initial Catalog=School;User ID=dbuser;Password=ppm123456";

        public static string Dbname = "test1.db3";
    }
}
