using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using queryBuilder;
using System.Windows.Forms;
namespace queryBuilder.dbIncludes
{
    class dbqSelect : dbQuery
    {
        public enum orderType { ASC, DESC };
        private StringBuilder q = new StringBuilder();
        public enum dbqaTypes { insert, selector, condition };
        public enum dbqpDistinct {yes,no};
        private dbqpDistinct isDistinct = dbqSelect.dbqpDistinct.no;
        public List<String> sTableName = new List<String>();
        public List<dbqArgument> selectArgs = new List<dbqArgument>();
        public List<dbqArgument> whereArgs = new List<dbqArgument>();
        public List<dbqArgument> sQArgs = new List<dbqArgument>();
        private DataSet ds = new DataSet();
        private column orderby=null;
        private orderType ordtype = dbqSelect.orderType.ASC;

        public dbqSelect(String db, String tName): base(db,"---", tName)
        {
            this.addTableName(base.TableName);
        }
        public dbqSelect(String db,String dbi, String tName): base(db,dbi, tName)
        {
            this.addTableName(base.TableName);
        }

        public void addArg(dbqArgument arg)
        {
            if (arg.Type==dbqArgument.dbqaTypes.selector)
            {
               
                this.selectArgs.Add(arg);
                if (arg.column1.Table.CompareTo("---") != 0)
                {
                    this.addTableName(arg.column1.Table);
                    
                }
            }
            else if (arg.Type == dbqArgument.dbqaTypes.condition || arg.Type == dbqArgument.dbqaTypes.fKey || arg.Type == dbqArgument.dbqaTypes.subQ)
            {
                
                this.whereArgs.Add(arg);
                if (arg.column1.Table.CompareTo("---") != 0)
                    this.addTableName(arg.column1.Table);
                if (arg.Type == dbqArgument.dbqaTypes.fKey)
                {
                    if (arg.column1.Table.CompareTo("---") != 0)
                    {
                        this.addTableName(arg.column2.Table);
                    }
                }
                else if (arg.Type == dbqArgument.dbqaTypes.subQ)
                {
 
                }
            }
        }
        public void addTableName(String tName)
        {
            sTableName.Add(  tName);
        }

        public void IsDistinct() 
        {
            this.isDistinct = dbqpDistinct.yes; 
        }
        public void OrderBy(column v,orderType o) 
        {
             this.orderby = v;
             this.ordtype = o;
        }

        public String generateQuery()
        {
            this.q.Clear();
            this.q.Append("select ");
            if (this.isDistinct == dbqSelect.dbqpDistinct.yes)
            {
                this.q.Append(" distinct ");
            }
            String scfiv = "'";
            int temp = 0;

            
            foreach (dbqArgument arg1 in this.selectArgs)
            {
                if (arg1.column1.Table.CompareTo("---") != 0)
                {
                    if (temp == 0)
                        q.Append("[" + arg1.column1.Table + "].[");
                    else
                        q.Append(",[" + arg1.column1.Table + "].[");
                }
                else
                {
                    if (temp == 0)
                        q.Append("[");
                    else
                        q.Append(",[");
                }
                this.q.Append(arg1.column1.Title);
                if (arg1.column1.sAlias.CompareTo("---") != 0)
                {
                    q.Append("] as " + arg1.column1.sAlias + " ");
                }
                else 
                {
                    q.Append("] ");
                }
               
                temp = 1;
            }
            
            int temp1 = 0;
            //MessageBox.Show(this.q.ToString());
            this.q.Append(" FROM ");
            foreach(String table in this.sTableName.Distinct().ToList()  )
            {
                if (temp1 == 0)
                {
                    this.q.Append('[' + base.dbName + "].");
                    if (base.dbi != "---")
                    {
                        this.q.Append("[" + base.dbi + "].");
                    }
                        this.q.Append("[");
                    
                }
                else
                {
                    this.q.Append(",[" + base.dbName + "].");
                    if (base.dbi != "---")
                    {
                        this.q.Append("[" + base.dbi + "].");
                    }
                    this.q.Append("[");
                }
                this.q.Append(table);
                this.q.Append("]");
                temp1 = 1;
            }
            int temp2 = 0;

            //
            foreach (dbqArgument arg3 in this.whereArgs)
            {
                if (arg3.column1.Table.CompareTo("---") != 0)
                {
                    if (temp2 == 0)
                    {
                        q.Append(" WHERE [" + arg3.column1.Table + "].[");
                    }
                    else
                    {
                        q.Append(" AND [" + arg3.column1.Table + "].[");
                    }
                }
                else
                {
                    if (temp2 == 0)
                    {
                        q.Append(" WHERE [");
                    }
                    else
                    {
                        q.Append(" AND [");
                    }
                }
                q.Append(arg3.column1.Title);
                q.Append("]");
                if (arg3.COp == dbqArgument.dbqacOp.eq)
                    q.Append(" = ");
                if (arg3.COp == dbqArgument.dbqacOp.nEq)
                    q.Append(" != ");
                if (arg3.COp == dbqArgument.dbqacOp.gT)
                    q.Append(" > ");
                if (arg3.COp == dbqArgument.dbqacOp.gTorEq)
                    q.Append(" >= ");
                if (arg3.COp == dbqArgument.dbqacOp.sT)
                    q.Append(" < ");
                if (arg3.COp == dbqArgument.dbqacOp.sTorEq)
                    q.Append(" <= ");
                if (arg3.COp == dbqArgument.dbqacOp.In)
                    q.Append(" in ");
                if (arg3.COp == dbqArgument.dbqacOp.nIn)
                    q.Append(" NOT IN ");
                if (arg3.COp == dbqArgument.dbqacOp.like)
                    q.Append(" LIKE ");
                if (arg3.Type == dbqArgument.dbqaTypes.fKey)
                {
                    if (arg3.column2.Table.CompareTo("---") != 0)
                    {
                        q.Append("[" + arg3.column2.Table + "].[");
                    }
                    else
                    {
                        q.Append("[");
                    }
                    q.Append(arg3.column2.Title);
                    q.Append("]");
                    if (arg3.column2.sAlias.CompareTo("---") != 0)
                    {
                        q.Append(" as " + arg3.column1.sAlias+" "); 
                    }
                }
                else if(arg3.Type == dbqArgument.dbqaTypes.condition )
                {
                    if (arg3.IsInt == dbqArgument.dbvaTypes.INT) { scfiv = ""; } else { scfiv = "'"; }
                    q.Append(scfiv + arg3.Value + scfiv);  
                }
                else if (arg3.Type == dbqArgument.dbqaTypes.subQ)
                {
                    q.Append(" ( ");
                    q.Append(arg3.SubQuery.generateQuery());
                    if (arg3.column1.Table.CompareTo("---") != 0)
                    {
                        if (arg3.column1.sAlias.CompareTo("---") != 0)
                        {
                            q.Append(" and [" + arg3.column1.Table + "].[" + arg3.column1.Title + "] is not null ) as "+arg3.column1.sAlias+" ");
                        }
                        else
                        {
                            q.Append(" and [" + arg3.column1.Table + "].[" + arg3.column1.Title + "] is not null ) ");
                        }
                    }
                    else
                    {
                        q.Append(" and [" + arg3.column1.Title + "] is not null ) ");
                    }
                }
                temp2 = 1;
            }
            if (this.orderby != null) 
            {
                q.Append(" order by " + ((this.orderby.Table != "---") ? "[" + this.orderby.Table + "].[" + this.orderby.Title + "] "+this.ordtype.ToString() : "[" + this.orderby.Title + "] "+this.ordtype.ToString()));
            }

            controller.m.addToLog(new errorBox("select query generated", this.q.ToString(), errorBox.errorType.msg));
            return this.q.ToString();
        }
    }
}
