using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using queryBuilder;
using System.Windows.Forms;
namespace queryBuilder.dbIncludes
{
    class dbqDelete : dbQuery
    {
        private System.Text.StringBuilder q = new System.Text.StringBuilder(1024);
        //private List<dbqArgument> insertArgs = new List<dbqArgument>();
        private List<dbqArgument> whereArgs = new List<dbqArgument>();
        public dbqDelete(String db,String tName): base(db,"---",tName)
        {
            
        }
        public void addArg(dbqArgument arg)
        {
            if (arg.Type==dbqArgument.dbqaTypes.condition)
            {
                this.whereArgs.Add(arg);

                //MessageBox.Show("insert");
            }
             // else if (arg.Type.Equals(dbqArgument.dbqaTypes.condition))
             //   this.whereArgs.Add(arg);
        }
        public String generateQuery() 
        {
            q.Append( "DELETE FROM " + base.dbName + "["+base.TableName + "] ");

            String scfiv = "'";
            int temp = 0;

            //MessageBox.Show(this.q.ToString());
            foreach (dbqArgument arg2 in this.whereArgs)
            {
                q.Append((temp == 0) ? " WHERE [" : " AND [");
                q.Append(arg2.column1.Title);
                q.Append("]=");
                if (arg2.IsInt == dbqArgument.dbvaTypes.INT) { scfiv = ""; } else { scfiv = "'"; }
                q.Append(scfiv + arg2.Value + scfiv);
                temp = 1;

            }

            controller.m.addToLog(new errorBox("Delete query generated",this.q.ToString(),errorBox.errorType.msg));
            
            return this.q.ToString();

        }
    }
}
