using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using queryBuilder;
using System.Windows.Forms;
namespace queryBuilder.dbIncludes
{
    class dbqInsert : dbQuery
    {
        private System.Text.StringBuilder q = new System.Text.StringBuilder(1024);
        private System.Text.StringBuilder strnj = new System.Text.StringBuilder(1024);
        public List<dbqArgument> insertArgs = new List<dbqArgument>();
        public dbqInsert(String dbname,String dbi,String tName): base(dbname,dbi,tName)
        {

        }
        public void addArg(dbqArgument arg)
        {
            if (arg.Type==dbqArgument.dbqaTypes.insert)
            {
                this.insertArgs.Add(arg);

               
            }
           
        }
        public String generateQuery() 
        {
            this.q.Clear();
            this.strnj.Clear();
            this.q.Append( "insert into ["+base.dbName+"].["+base.dbi+"].["+base.TableName+"] (");
            this.strnj.Append( "values (");
            String scfiv = "'";
            int temp = 0;
            foreach (dbqArgument arg in this.insertArgs) 
            {
                q.Append( ((temp == 0) ? "[" : ",[") + arg.column1.Title + "]");
                if (arg.IsInt == dbqArgument.dbvaTypes.INT) { scfiv = ""; } else { scfiv = "'"; }
                this.strnj.Append( ((temp == 0) ? scfiv : "," + scfiv) + arg.Value + scfiv );
                temp = 1;
            }
            this.q.Append( ")  ");
            this.q.Append(this.strnj.ToString());
            this.q.Append(") ");

          
                controller.m.addToLog(new errorBox("insert query generated", this.q.ToString(), errorBox.errorType.msg));
  
            return this.q.ToString();
        }
    }
}
