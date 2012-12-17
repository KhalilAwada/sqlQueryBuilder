using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

/*
 * this class trims unneeded characters from string
 * and returns only allowed char types
 * methods available:
 * ---[String Gaurd]----------------------
 * [1] (a-z + A-Z)
 * [2] (a-z + A-Z + 0-9)
 * [3] (a-z + A-Z + ' ' + '-')
 * [4] (a-z + A-Z + ' ')
 * [5] (a-z + A-Z + '-')
 * [6] (0-9)
 * [7] (0-9 + '/' )
 * [8] (a-z + A-Z + ' ' + '-' + '_')
 * ---[addSlashes]-------- quote special characters
 * ---[stripSlashes]------ unquote special characters
 * 
*/
namespace queryBuilder
{
    class stringValidate
    {
        public String strGaurd(String str,int type)
        {
            if (type == 1)
            {
                Regex rgx = new Regex("[^a-zA-Z]");
                str = rgx.Replace(str, "");
            }
            else if (type == 2)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                str = rgx.Replace(str, "");
            }
            else if (type == 3)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                str = rgx.Replace(str, "");
            }
            else if (type == 4)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 ]");
                str = rgx.Replace(str, "");
            }
            else if (type == 5)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9-]");
                str = rgx.Replace(str, "");
            }
            else if (type == 6)
            {
                Regex rgx = new Regex("[^0-9]");
                str = rgx.Replace(str, "");
            }
            else if (type == 7)
            {
                Regex rgx = new Regex("[^0-9/]");
                str = rgx.Replace(str, "");
            }
            else if (type == 7)
            {
                Regex rgx = new Regex("[^0-9-_ ]");
                str = rgx.Replace(str, "");
            }
            return str;
        }

        // Returns a string with backslashes before characters that need to be quoted
        public string addSlashes(string str)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            string Result = str;

            try
            {
                Result = System.Text.RegularExpressions.Regex.Replace(str, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Result = Ex.Message;
            }

            return Result;
        }

        // Un-quotes a quoted string
        public string stripSlashes(string str)
        {
            // List of characters handled:
            // \000 null
            // \010 backspace
            // \011 horizontal tab
            // \012 new line
            // \015 carriage return
            // \032 substitute
            // \042 double quote
            // \047 single quote
            // \134 backslash
            // \140 grave accent

            string Result = str;
            try
            {
                Result = System.Text.RegularExpressions.Regex.Replace(str, @"(\\)([\000\010\011\012\015\032\042\047\134\140])", "$2");
            }
            catch (Exception Ex)
            {
                // handle any exception here
                MessageBox.Show("Error: " + Ex.Message);
                Result = Ex.Message;
            }
            return Result;
        }
    }
}