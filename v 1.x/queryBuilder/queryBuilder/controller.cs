using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using queryBuilder.dbIncludes;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;
using System.Net;
namespace queryBuilder
{
    class controller
    {
     // system inistialization 

        public static dbConnect dbcnf = new dbConnect(".", "master");
  
        public static mainForm m  = new mainForm();
        public static stringValidate strGard;

        public static Boolean debugMode = true;

        public  controller()
        {
            dbcnf.connect();
            if (debugMode)
            {
              
                controller.m.Show();
            }
        }



        public static List<String> getDBList()
        {
            List<String> dblist = new List<String>();
            dbqSelect getDBquery = new dbqSelect("sys", "databases");
            getDBquery.addArg(new dbqArgument(new column("name")));
            getDBquery.addArg(new dbqArgument(new column("name"), "('master', 'tempdb', 'model', 'msdb')", dbqArgument.dbqacOp.nIn, dbqArgument.dbvaTypes.INT));
            using (SqlDataReader readerr = controller.dbcnf.execQuery(getDBquery, dbConnect.queryMode.normalSelectQuery))
            {
                while (readerr.Read())
                {
                    dblist.Add(readerr["name"].ToString());
                }
            }
            return dblist;
        }

        public static List<String> getTableList(String db)
        {
            List<String> tableList = new List<String>();
            dbcnf.myConnection.ChangeDatabase(db);
            dbqSelect getDBquery = new dbqSelect("sys", "tables");
            getDBquery.addArg(new dbqArgument(new column("name")));
            using (SqlDataReader readerr = controller.dbcnf.execQuery(getDBquery, dbConnect.queryMode.normalSelectQuery))
            {
                while (readerr.Read())
                {
                    tableList.Add(readerr["name"].ToString());
                }
            }
            return tableList;
        }
        public static List<column> getColumnList(String db, String table) 
        {
  //          SELECT columns.name,types.name,columns.is_nullable,columns.is_identity FROM sys.columns,sys.types 
//            WHERE object_id = OBJECT_ID('test') and  types.system_type_id=columns.system_type_id and types.system_type_id=types.user_type_id

            List<column> columnList = new List<column>();
            dbcnf.myConnection.ChangeDatabase(db);
            dbqSelect getColumns = new dbqSelect("sys", "columns");
            getColumns.addArg(new dbqArgument(new column("columns", "name","name")));
            getColumns.addArg(new dbqArgument(new column("types", "name","type")));
            getColumns.addArg(new dbqArgument(new column("columns", "is_nullable")));
            getColumns.addArg(new dbqArgument(new column("columns", "is_identity")));
            getColumns.addArg(new dbqArgument(new column("types", "system_type_id"), new column("columns", "system_type_id"), dbqArgument.dbqacOp.eq));
            getColumns.addArg(new dbqArgument(new column("types", "system_type_id"), new column("types", "user_type_id"), dbqArgument.dbqacOp.eq));
            getColumns.addArg(new dbqArgument(new column("columns", "object_id"), " OBJECT_ID('"+table+"')", dbqArgument.dbqacOp.eq,dbqArgument.dbvaTypes.INT));
            using (SqlDataReader readerr = controller.dbcnf.execQuery(getColumns, dbConnect.queryMode.normalSelectQuery))
            {
                while (readerr.Read())
                {
                    column c = new column(table, readerr["name"].ToString());
                    c.isNullable =(Boolean)readerr["is_nullable"];
                    c.isIdentity =(Boolean)readerr["is_identity"];
                    c.Type = readerr["type"].ToString();
                    columnList.Add(c);
                }
            }
            return columnList;
        }

    }
    
}
