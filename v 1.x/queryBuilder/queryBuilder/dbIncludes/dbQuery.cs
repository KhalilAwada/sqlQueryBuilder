using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using queryBuilder;
namespace queryBuilder.dbIncludes
{
    class dbQuery
    {
        private String sTableName;
        public dbQuery(String dbName, String dbi, String tName)
        {
            this.dbName = dbName;
            this.TableName = tName;
            this.dbi = dbi;
        }

        public string TableName
        {
            set { sTableName = value; }
            get { return sTableName; }
        }
        public String dbName { set; get; }
        public String dbi { set; get; }
        public String Title { set; get; }



       
    }
}
