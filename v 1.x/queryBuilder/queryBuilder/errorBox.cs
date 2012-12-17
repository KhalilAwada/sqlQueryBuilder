using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace queryBuilder
{
    public class errorBox
    {
       
        public enum errorType{msg,warning,error,exception}
        public errorBox(String title,String subject,errorType errt) 
        {
            this.ErrT = errt;
            this.Title = title;
            this.Subject = subject;
        }
        public String Title{set;get;}
        public String Subject { set; get; }
        public errorType ErrT { set; get; }
    }
}
