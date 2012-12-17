using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace queryBuilder.dbIncludes
{
    class dbConnect
    {
        public enum queryMode { normalSelectQuery, singleRowSelectQuery };
		private string sUser;
		private string sPasswd;
		private string sErrString;
		private string sDB;
		private string sServer;
		private string sCnString;
        private SqlDataReader dbReader = null;
        private SqlCommand myCommand;
        public SqlConnection myConnection;
	    
        public dbConnect (String sServer,String sDB,String sUser,String sPasswd)
        {
            this.Server = sServer;
            this.DB = sDB;
            this.User = sUser;
            this.Passwd = sPasswd;
            this.CnString= "Data Source=" + this.Server + ";"
                +"Initial Catalog=" + this.DB + ";"
                +"Integrated Security=True;"
                +"Integrated Security=SSPI;"
                +"persist security info=False;"
                +"user id=" + this.User + ";"
                +"password =" + this.Passwd + ";";
        }
        public dbConnect(String sServer, String sDB)
        {
            this.Server = sServer;
            this.DB = sDB;
            this.CnString = "Data Source=" + this.Server + ";"
                 + "Initial Catalog=" + this.DB + ";"
                 + "Integrated Security=True;"
                 + "Integrated Security=SSPI;"
                 + "persist security info=False;"
                 + "Trusted_Connection=yes;";
        }

        private string User{
			set{sUser=value;}
			get{return sUser;}
		}
		private string Passwd{
			set{sPasswd=value;}
			get{return sPasswd;}
		}
		public string ErrString{
			set{sErrString=value;}
			get{return sErrString;}
		}
		private string DB{
			set{sDB=value;}
			get{return sDB;}
		}
		private string Server{
			set{sServer=value;}
			get{return sServer;}
		}
		public string CnString
        {
			set{sCnString = value;}
			get{return sCnString;}    
        }


        public void connect()
        {
            this.myConnection = new SqlConnection(this.CnString);
            try
            {
                myConnection.Open();
                this.ErrString += "\nConnected successfully";
                controller.m.addToLog(new errorBox("connect to db","successfull db online",errorBox.errorType.msg)) ;
                
            }
            catch (Exception e)
            {
                this.ErrString += "\n"+e.ToString();
                controller.m.addToLog(new errorBox("connect to db", "failed\n" + e.ToString(), errorBox.errorType.exception));
                MessageBox.Show("couldnot connect to database \n"+e.ToString());
                System.Windows.Forms.Application.Exit();
            }
        }

        public int execQuery(dbqInsert q)
        {
            try
            {
                this.myCommand= new SqlCommand("set identity_insert " + q.TableName+" on  ",this.myConnection);
                this.myCommand.ExecuteNonQuery();
            }
            catch(Exception e2)
            {
                this.ErrString += "\n" + e2.ToString();
                controller.m.addToLog(new errorBox("set identity executed", e2.ToString(), errorBox.errorType.exception));
                
            }
            try
            {
                this.myCommand = new SqlCommand(q.generateQuery(), this.myConnection);
                this.myCommand.ExecuteScalar();
                this.ErrString += "\ninsert query executed";
                controller.m.addToLog(new errorBox("insert query executed", "inserted" , errorBox.errorType.msg));
                return 1;
            }
            catch (Exception e1)
            {
                this.ErrString += "\n" + e1.ToString();
                controller.m.addToLog(new errorBox("insert query executed", e1.ToString(), errorBox.errorType.exception));
                return -1;
            }
        }
        public Boolean execQuery(dbqUpdate q)
        {
            try
            {
                this.myCommand = new SqlCommand(q.generateQuery(), this.myConnection);
                //this.connect();
                int affectedRows = this.myCommand.ExecuteNonQuery();
                this.ErrString += "\n" + affectedRows + "\n Update query executed";
                controller.m.addToLog(new errorBox("update query executed", "affected rows :" + affectedRows, errorBox.errorType.msg));
                return true;
            }
            catch (Exception e1)
            {
                this.ErrString += "\n" + e1.ToString();
                controller.m.addToLog(new errorBox("update query executed", e1.ToString(), errorBox.errorType.exception));
                return false;
            }
        }
        public Boolean execQuery(dbqDelete q)
        {
            try
            {
                this.myCommand = new SqlCommand(q.generateQuery(), this.myConnection);
                //this.connect();
                int affectedRows = this.myCommand.ExecuteNonQuery();
                this.ErrString += "\n" + affectedRows + "\n Delete query executed";
                controller.m.addToLog(new errorBox("Delete query executed", "affected rows :" + affectedRows, errorBox.errorType.msg));
                return true;
            }
            catch (Exception e1)
            {
                this.ErrString += "\n" + e1.ToString();
                controller.m.addToLog(new errorBox("Delete query executed", e1.ToString(), errorBox.errorType.exception));
                return false;
            }
        }
        public SqlDataReader execQuery(dbqSelect q , queryMode qm)
        {
            try
            {
                this.myCommand = new SqlCommand(q.generateQuery(), this.myConnection);
                //this.connect();
                if (qm == dbConnect.queryMode.singleRowSelectQuery)
                {
                    dbReader = this.myCommand.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                }
                else if (qm == dbConnect.queryMode.normalSelectQuery)
                {
                    dbReader = this.myCommand.ExecuteReader();
                }
                this.ErrString += "\n" + dbReader.HasRows.ToString();
                controller.m.addToLog(new errorBox("select query executed", dbReader.HasRows.ToString(), errorBox.errorType.msg));
                return dbReader;
            }
            catch (Exception e1)
            {
                this.ErrString += "\n" + e1.ToString();

               controller.m.addToLog(new errorBox("select query executed",e1.ToString(),errorBox.errorType.exception));
                
                return null;
            }
        }
        
    }
}
