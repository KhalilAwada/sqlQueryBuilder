using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace queryBuilder.dbIncludes
{
    class column
    {
        private String sTable;
        private String sTitle;
        public String sAlias { set; get; }
        public Boolean isNullable{ set; get; }
        public Boolean isIdentity{ set; get; }
        public String Type{ set; get; }
        public column(String table, String title, String alias)
        {
            this.sAlias = alias;
            this.Table = table;
            this.Title = title;
        }
        public column(String table, String title)
        {
            this.sAlias = "---";
            this.Table = table;
            this.Title = title;
        }
        public column(String title)
        {
            this.sAlias = "---";
            this.Table = "---";
            this.Title = title;
        }
        
        public String Table { set { this.sTable = value; } get { return this.sTable; } }
        public String Title { set { this.sTitle = value; } get { return this.sTitle; } }
    }
}
