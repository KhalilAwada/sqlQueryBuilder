using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using queryBuilder;
using System.Windows.Forms;
namespace queryBuilder.dbIncludes
{
 


    class dbqArgument
    {
        
        public enum dbqaTypes { insert , selector , condition ,fKey,subQ} ;
        public enum dbqacOp {eq,nEq,gT,gTorEq,sT,sTorEq,like,In,nIn }
        public enum dbvaTypes { INT = 1 , STRING = 0 } ;
        private stringValidate strvldt = new stringValidate();
        private column col1 = null;
        private column col2 = null;
        private dbqSelect subQ;
        private String sValue;
        private dbqaTypes sType;
        private dbvaTypes isInt;
        private dbqacOp cOp = dbqArgument.dbqacOp.eq;
        public String Title
        {
            get
            {
                if (this.sType == dbqaTypes.selector)
                    return col1.Table + "." + col1.Title;
                else if (this.sType == dbqaTypes.condition)
                    return col1.Table + "." + col1.Title + " " + this.COp + " " + this.Value;
                else if (this.sType == dbqaTypes.fKey)
                    return col1.Table + "." + col1.Title + " " + this.COp + " " + col2.Table + "." + col2.Title;
                else if (this.sType == dbqaTypes.insert)
                {
                    if (col1.Type == "int")
                        return col1.Table + "." + col1.Title + " " + this.COp + " " + this.Value;
                    else
                        return col1.Table + "." + col1.Title + " " + this.COp + " '" + this.Value + "'";
                }
                else if (this.sType == dbqaTypes.subQ)
                {
                    return col1.Table + "." + col1.Title + " " + this.COp + " (" + this.subQ.Title + ")";
                }
                else
                    return "";
            }
        }

        public dbqArgument(column coll, String sValue, dbvaTypes isInt)
        {
            this.column1 = coll;
            this.Value = sValue;
            this.Type = dbqArgument.dbqaTypes.insert;
            this.IsInt = isInt;
        }

        public dbqArgument(column coll, String sValue, dbqacOp cOperator, dbvaTypes isInt)
        {
            this.column1 = coll;
            this.Value = sValue;
            this.Type = dbqArgument.dbqaTypes.condition;
            this.IsInt = isInt;
            this.cOp = cOperator;
        }

        public dbqArgument(column coll, dbqSelect q, dbqacOp cOperator)
        {
            this.column1 = coll;
            this.subQ = q;
            this.Type = dbqArgument.dbqaTypes.subQ;
            this.cOp = cOperator;
        }

        public dbqArgument(column col1, column col2,dbqacOp cOperator)
        {
            this.column1 = col1;
            this.column2 = col2;
            this.cOp = cOperator;
            this.Type = dbqaTypes.fKey;
        }

        public dbqArgument(column coll)
        {
            this.col1 = coll;
            this.sType = dbqaTypes.selector;
        }

        public dbqacOp COp
        {
            set
            {
                this.cOp = value;
            }
            get
            {
                return this.cOp;
            }
        }

        public dbqSelect SubQuery
        {
            set
            {
                this.subQ = value;
            }
            get
            {
                return this.subQ;
            }
        } 

        public column column1
        {
            set { col1 = value; }
            get { return this.col1; }
        }

        public column column2
        {
            set { col2 = value; }
            get { return this.col2; }
        }

        public string Value
        {
            set { sValue =  value; }
            get { return sValue; }
        }

        public dbvaTypes IsInt
        {
            set { isInt= value; }
            get { return isInt; }
        }

        public dbqaTypes Type
        {
            set { sType = value; }
            get { return sType; }
        }

    }

}
