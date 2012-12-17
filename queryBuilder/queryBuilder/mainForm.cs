using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using queryBuilder.dbIncludes;
using System.Data.SqlClient;
using System.Dynamic;
using System.Diagnostics;
using System.Threading;
namespace queryBuilder
{
    public partial class mainForm : Form
    {
        private String CurrentDB{set;get;}
        private String CurrentTable { set; get; }
        
        private column QueryConditionCol1 = null;
        private column QueryConditionCol2 = null;

        private List<dbQuery> SavedQueriesList = new List<dbQuery>();
        private List<dbQuery> SavedSelectQueriesList = new List<dbQuery>();

        //select query
        private dbqSelect CurrentSelectQuery;
        
        //insert query
        private dbqInsert CurrentInsertQuery;
 
        //update query
        private dbqUpdate CurrentUpdateQuery;

        private Random rn = new Random();
        public mainForm()
        {
            InitializeComponent();
        }
        
        private void Main_Load(object sender, EventArgs e)
        {
            panel1.Height=0;
            panel1.Location = new Point(50, -2);
            selectQueryConditionTypeComboBox.DataSource = System.Enum.GetValues(typeof(dbqArgument.dbqacOp));
            updateQueryConditionTypeComboBox.DataSource = System.Enum.GetValues(typeof(dbqArgument.dbqacOp));
            selectQuerySavedQueryComboBox.DataSource = this.SavedQueriesList;
            selectQuerySavedQueryComboBox.DisplayMember="Title";
            updateQuerySavedQueriesComboBox.DataSource = this.SavedQueriesList;
            updateQuerySavedQueriesComboBox.DisplayMember = "title";
            this.reloadDBlist();
            generateQueryGroupBox.Enabled = false;
        }
  
        private void resetAllSelectQueryTabPageEelements()
        {
            selectQuerySelectColumnsListBox.DataSource = null;
            selectQueryTablesListBox.DataSource = null;
            selectQueryConditionListBox.DataSource = null;
            selectQueryConditionCol1TextBox.Text = "";
            selectQueryConditionValueTextBox.Text = "";
            selectQueryConditionCol2TextBox.Text = "";
            this.CurrentSelectQuery = null;
            this.QueryConditionCol1 = null;
            this.QueryConditionCol2 = null;
        }
       
        private void reloadDBlist() 
        {
            dbListView.Items.Clear();
            foreach(String a in controller.getDBList())
                dbListView.Items.Add(a,0);

            tableListView.Items.Clear();
            columnListView.Items.Clear();
        }

        private void reloadTableList(string db)
        {
            tableListView.Items.Clear();
            foreach (String a in controller.getTableList(db))
                tableListView.Items.Add(a,3);
           
            columnListView.Items.Clear();
        }

        private void reloadColumnList(string db, string table)
        {
            columnListView.Items.Clear();
            int i;
            foreach (column a in controller.getColumnList(db, table))
            {

                if (a.isIdentity)
                    i = 0;
                else if (!a.isNullable)
                    i = 2;
                else
                    i = 3;
                ListViewItem c = new ListViewItem(a.Title, i);
                c.Tag = a;
                c.ToolTipText = a.Type;
                columnListView.Items.Add(c);
            }

        }

        private void reSetAllInsertQueryTabPageElements()
        {
            insertQueryInsertionColumnsListBox.DataSource = null;
            insertQueryinsertionColumnValueRichTextBox.Text = "";
            InsertQueryTableTextBox.Text=this.CurrentTable;
        }

        private void reSetAllUpdateQueryTabPageElements()
        {
            updateQueryTableTextBox.Text = this.CurrentTable;
            updateQueryConditionCol1TextBox.Text="";
            updateQueryConditionCol2TextBox.Text="";
            updateQueryConditionsListBox.DataSource=null;
            updateQueryConditionValueTextBox.Text="";
            updateQuerySetValueToColumnRichTextBox.Text="";
            updateQueryUpdateColumnsListBox.DataSource=null;
        }

        public void addToLog(errorBox ebox)
        {
           
                if (controller.debugMode)
                {
                    try
                    {
                       StringBuilder logstr = new StringBuilder();
                        logstr.Append("\n\n---[ " + ebox.Title + " ]");
                        for(int i=0;i<=((32)-(ebox.Title.ToCharArray().Count()));i++)
                        {
                            logstr.Append("-");
                        }
                        logstr.AppendLine("\n");
                        logstr.AppendLine(ebox.Subject);
                        debugRichTextBox.Text += logstr.ToString();
                        debugRichTextBox.SelectionStart = debugRichTextBox.Text.Length;
                        debugRichTextBox.ScrollToCaret();
                    }
                    catch (Exception ee) 
                    {
                        MessageBox.Show(ee.ToString());
                    }

                }
            }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void dbListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CurrentSelectQuery = null;
            this.resetAllSelectQueryTabPageEelements();
            try{ this.CurrentDB = dbListView.SelectedItems[0].Text.ToString(); }
            catch {}
            this.reloadTableList(this.CurrentDB);
        }

        private void tableListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CurrentInsertQuery = null;
            this.CurrentUpdateQuery = null;
            this.reSetAllInsertQueryTabPageElements();
            this.reSetAllUpdateQueryTabPageElements();
            try
            {
                this.CurrentTable = tableListView.SelectedItems[0].Text.ToString();
                InsertQueryTableTextBox.Text = this.CurrentTable;
                updateQueryTableTextBox.Text = this.CurrentTable;
                if (this.CurrentSelectQuery == null && generateQueryTabControl.SelectedTab == selectQueryTabPage)
                {
                    this.CurrentSelectQuery = new dbqSelect(this.CurrentDB, "dbo", this.CurrentTable);
                    generateQueryGroupBox.Enabled = true;
                }
            }
            catch {}
            this.reloadColumnList(this.CurrentDB, this.CurrentTable);
        }

        private void columnListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            StringBuilder a = new StringBuilder();
            foreach ( int i in columnListView.SelectedIndices)
                a.Append(i.ToString()+',');
            DoDragDrop((a.ToString()), DragDropEffects.Move);
        }

        private void selectQuerySelectColumnsListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void columnListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void selectQuerySelectColumnsListBox_DragDrop(object sender, DragEventArgs e)
        {
            String w = e.Data.GetData(DataFormats.StringFormat) as String;
            String[] indecies = (e.Data.GetData(DataFormats.StringFormat) as String).Split(',');

            foreach (String i in indecies)
            {
                if (i != "")
                {
                   // MessageBox.Show("--" + i);
                    int ii = Convert.ToInt32(i.Trim());
                    column ci = (column)((ListViewItem)(columnListView.Items[ii])).Tag;
                    this.CurrentSelectQuery.addArg(new dbqArgument(ci));
                }
            }
            selectQuerySelectColumnsListBox.DataSource = null;
            selectQuerySelectColumnsListBox.DataSource = this.CurrentSelectQuery.selectArgs;
            selectQuerySelectColumnsListBox.DisplayMember = "Title";
            selectQueryTablesListBox.DataSource = null;
            selectQueryTablesListBox.DataSource = this.CurrentSelectQuery.sTableName.Distinct().ToList();

        }

        private void newQueryButton_Click(object sender, EventArgs e)
        {
            generateQueryGroupBox.Enabled = false;
            this.resetAllSelectQueryTabPageEelements();
            this.reSetAllInsertQueryTabPageElements();
            this.reSetAllUpdateQueryTabPageElements();
            generateQueryGroupBox.Enabled = true;
            if (this.CurrentDB != null && this.CurrentTable != null)
            {
                if (generateQueryTabControl.SelectedTab == selectQueryTabPage)
                {
                    this.CurrentSelectQuery = new dbqSelect(this.CurrentDB, "dbo", this.CurrentTable);
                }
                else if (generateQueryTabControl.SelectedTab == insertQueryTabPage)
                {
                    this.CurrentInsertQuery = new dbqInsert(this.CurrentDB, "dbo", this.CurrentTable);
                }
                else if (generateQueryTabControl.SelectedTab == updateQueryTabPage)
                {
                    this.CurrentUpdateQuery = new dbqUpdate(this.CurrentDB, "dbo", this.CurrentTable);
                }
            }
            else
            {
                MessageBox.Show("please select a table 1st");
            }

        }

        private void generateQueryButton_Click(object sender, EventArgs e)
        {
            if (generateQueryTabControl.SelectedTab == selectQueryTabPage)
            {
                richTextBox1.Text = this.CurrentSelectQuery.generateQuery();
            }
            else if (generateQueryTabControl.SelectedTab == insertQueryTabPage)
            {
                richTextBox1.Text = this.CurrentInsertQuery.generateQuery();
            }
            else if (generateQueryTabControl.SelectedTab == updateQueryTabPage)
            {
                richTextBox1.Text = this.CurrentUpdateQuery.generateQuery();
            }
        }

        private void selectQueryConditionCol1TextBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void selectQueryConditionCol1TextBox_DragDrop(object sender, DragEventArgs e)
        {
            String w = e.Data.GetData(DataFormats.StringFormat) as String;
            String[] indecies = (e.Data.GetData(DataFormats.StringFormat) as String).Split(',');
            foreach (String i in indecies)
            {
                if (i != "")
                {
                    int ii = Convert.ToInt32(i.Trim());
                    column ci = (column)((ListViewItem)(columnListView.Items[ii])).Tag;
                    this.QueryConditionCol1 = ci;
                    ((System.Windows.Forms.TextBox)sender).Text = ci.Table + "." + ci.Title;
                }
            }
        }

        private void selectQueryConditionAddButton_Click(object sender, EventArgs e)
        {
            if (selectQueryConditionTabControl.SelectedTab==colToValueTabPage){
                if (this.QueryConditionCol1.Type == "int" )
                    this.CurrentSelectQuery.addArg(new dbqArgument(this.QueryConditionCol1,selectQueryConditionValueTextBox.Text,(dbqArgument.dbqacOp)selectQueryConditionTypeComboBox.SelectedValue,dbqArgument.dbvaTypes.INT));
                else 
                    this.CurrentSelectQuery.addArg(new dbqArgument(this.QueryConditionCol1,selectQueryConditionValueTextBox.Text,(dbqArgument.dbqacOp)selectQueryConditionTypeComboBox.SelectedValue,dbqArgument.dbvaTypes.STRING));
               
            }
            else if (selectQueryConditionTabControl.SelectedTab == colToColTabPage)
            {
                this.CurrentSelectQuery.addArg(new dbqArgument(this.QueryConditionCol1, this.QueryConditionCol2, (dbqArgument.dbqacOp)selectQueryConditionTypeComboBox.SelectedValue));
            }
            else if (selectQueryConditionTabControl.SelectedTab == colToQueryTabPage)
            {
                    this.CurrentSelectQuery.addArg(new dbqArgument(this.QueryConditionCol1, (dbqSelect)selectQuerySavedQueryComboBox.SelectedItem, (dbqArgument.dbqacOp)selectQueryConditionTypeComboBox.SelectedValue));
            }
            selectQueryTablesListBox.DataSource = null;
            selectQueryTablesListBox.DataSource = this.CurrentSelectQuery.sTableName.Distinct().ToList();
            selectQueryConditionListBox.DataSource = null;
            selectQueryConditionListBox.DataSource = this.CurrentSelectQuery.whereArgs.Distinct().ToList();//
            selectQueryConditionListBox.DisplayMember = "Title";
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            String w = e.Data.GetData(DataFormats.StringFormat) as String;
            String[] indecies = (e.Data.GetData(DataFormats.StringFormat) as String).Split(',');
            foreach (String i in indecies)
            {
                if (i != "")
                {
                    int ii = Convert.ToInt32(i.Trim());
                    column ci = (column)((ListViewItem)(columnListView.Items[ii])).Tag;
                    this.QueryConditionCol2 = ci;
                    ((System.Windows.Forms.TextBox)sender).Text = ci.Table + "." + ci.Title;
                }
            }
        }

        private void insertQueryInsertionColumnsListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void insertQueryInsertionColumnsListBox_DragDrop(object sender, DragEventArgs e)
        {
            if (this.CurrentInsertQuery == null)
                this.CurrentInsertQuery = new dbqInsert(this.CurrentDB,"dbo",this.CurrentTable);
            String w = e.Data.GetData(DataFormats.StringFormat) as String;
            String[] indecies = (e.Data.GetData(DataFormats.StringFormat) as String).Split(',');

            foreach (String i in indecies)
            {
                if (i != "")
                {
                    int ii = Convert.ToInt32(i.Trim());
                    column ci = (column)((ListViewItem)(columnListView.Items[ii])).Tag;
                    if (ci.Type == "int" )
                    {
                        try
                        {
                            this.CurrentInsertQuery.addArg(new dbqArgument(ci, "", dbqArgument.dbvaTypes.INT));
                        }
                        catch { }
                    }
                    else 
                    {
                        this.CurrentInsertQuery.addArg(new dbqArgument(ci,"",dbqArgument.dbvaTypes.STRING));
                    }
                }
            }
            insertQueryInsertionColumnsListBox.DataSource = null;
            insertQueryInsertionColumnsListBox.DataSource = this.CurrentInsertQuery.insertArgs.Distinct().ToList();
            insertQueryInsertionColumnsListBox.DisplayMember = "Title";

        }

        private void generateQueryTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.reSetAllInsertQueryTabPageElements();
            this.resetAllSelectQueryTabPageEelements();
            this.reSetAllUpdateQueryTabPageElements();
            this.CurrentSelectQuery = new dbqSelect(this.CurrentDB, "dbo", this.CurrentTable);
            this.CurrentInsertQuery = new dbqInsert(this.CurrentDB, "dbo", this.CurrentTable);
            InsertQueryTableTextBox.Text = this.CurrentTable;
            this.CurrentUpdateQuery = new dbqUpdate(this.CurrentDB, "dbo", this.CurrentTable);
            updateQueryTableTextBox.Text = this.CurrentTable;
        }

        private void insertQueryInsertionColumnsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                insertQueryinsertionColumnValueRichTextBox.Text = ((dbqArgument)insertQueryInsertionColumnsListBox.SelectedItem).Value;
            }
            catch 
            {
            }
        }

        private void insertQueryinsertionColumnValueRichTextBox_Leave(object sender, EventArgs e)
        {
            ((dbqArgument)insertQueryInsertionColumnsListBox.SelectedItem).Value = insertQueryinsertionColumnValueRichTextBox.Text;
            insertQueryInsertionColumnsListBox.DataSource = null;
            insertQueryInsertionColumnsListBox.DataSource = this.CurrentInsertQuery.insertArgs.Distinct().ToList();
            insertQueryInsertionColumnsListBox.DisplayMember = "Title";
        }

        private void updateQueryUpdateColumnsListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect=DragDropEffects.All;
        }

        private void updateQueryUpdateColumnsListBox_DragDrop(object sender, DragEventArgs e)
        {
            String w = e.Data.GetData(DataFormats.StringFormat) as String;
            String[] indecies = (e.Data.GetData(DataFormats.StringFormat) as String).Split(',');

            foreach (String i in indecies)
            {
                if (i != "")
                {
                    int ii = Convert.ToInt32(i.Trim());
                    column ci = (column)((ListViewItem)(columnListView.Items[ii])).Tag;
                    if (ci.Type == "int")
                    {
                        this.CurrentUpdateQuery.addArg(new dbqArgument(ci, "", dbqArgument.dbvaTypes.INT));
                    }
                    else
                    {
                        this.CurrentUpdateQuery.addArg(new dbqArgument(ci, "", dbqArgument.dbvaTypes.STRING));
                    }
                }
            }
            updateQueryUpdateColumnsListBox.DataSource = null;
            updateQueryUpdateColumnsListBox.DataSource = this.CurrentUpdateQuery.insertArgs.Distinct().ToList();
            updateQueryUpdateColumnsListBox.DisplayMember = "Title";
        }

        private void updateQuerySetValueToColumnRichTextBox_Leave(object sender, EventArgs e)
        {
            ((dbqArgument)updateQueryUpdateColumnsListBox.SelectedItem).Value = updateQuerySetValueToColumnRichTextBox.Text;
            updateQueryUpdateColumnsListBox.DataSource = null;
            updateQueryUpdateColumnsListBox.DataSource = this.CurrentUpdateQuery.insertArgs.Distinct().ToList();
            updateQueryUpdateColumnsListBox.DisplayMember = "Title";
        }

        private void updateQueryConditionCol1TextBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void updateQueryConditionCol1TextBox_DragDrop(object sender, DragEventArgs e)
        {
            String w = e.Data.GetData(DataFormats.StringFormat) as String;
            String[] indecies = (e.Data.GetData(DataFormats.StringFormat) as String).Split(',');
            foreach (String i in indecies)
            {
                if (i != "")
                {
                    int ii = Convert.ToInt32(i.Trim());
                    column ci = (column)((ListViewItem)(columnListView.Items[ii])).Tag;
                    this.QueryConditionCol1 = ci;
                    ((System.Windows.Forms.TextBox)sender).Text = ci.Table + "." + ci.Title;
                }
            }
        }

        private void updateQueryConditionAddButton_Click(object sender, EventArgs e)
        {
            if (updateQueryAddConditionTabControl.SelectedTab == updateQueryConditionColToValueTabPage)
            {
                if (this.QueryConditionCol1.Type == "int")
                {
                    this.CurrentUpdateQuery.addArg(new dbqArgument(this.QueryConditionCol1, updateQueryConditionValueTextBox.Text, (dbqArgument.dbqacOp)updateQueryConditionTypeComboBox.SelectedValue, dbqArgument.dbvaTypes.INT));
                }
                else
                {
                    this.CurrentUpdateQuery.addArg(new dbqArgument(this.QueryConditionCol1, updateQueryConditionValueTextBox.Text, (dbqArgument.dbqacOp)updateQueryConditionTypeComboBox.SelectedValue, dbqArgument.dbvaTypes.STRING));
                }
            }
            else if (updateQueryAddConditionTabControl.SelectedTab == updateQueryConditionColToColTabPage)
            {
                this.CurrentUpdateQuery.addArg(new dbqArgument(this.QueryConditionCol1, this.QueryConditionCol2, (dbqArgument.dbqacOp)updateQueryConditionTypeComboBox.SelectedValue));
            }
            else if (updateQueryAddConditionTabControl.SelectedTab == updateQueryConditionColToQueryTabPage)
            {
                this.CurrentUpdateQuery.addArg(new dbqArgument(this.QueryConditionCol1, (dbqSelect)updateQuerySavedQueriesComboBox.SelectedItem, (dbqArgument.dbqacOp)selectQueryConditionTypeComboBox.SelectedValue));
            }

            updateQueryTableTextBox.Text = this.CurrentTable;
            updateQueryConditionsListBox.DataSource = null;
            updateQueryConditionsListBox.DataSource = this.CurrentUpdateQuery.whereArgs;
            updateQueryConditionsListBox.DisplayMember = "Title";
        }

        private void updateQueryConditionCol2TextBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void updateQueryConditionCol2TextBox_DragDrop(object sender, DragEventArgs e)
        {
            String w = e.Data.GetData(DataFormats.StringFormat) as String;
            String[] indecies = (e.Data.GetData(DataFormats.StringFormat) as String).Split(',');
            foreach (String i in indecies)
            {
                if (i != "")
                {
                    int ii = Convert.ToInt32(i.Trim());
                    column ci = (column)((ListViewItem)(columnListView.Items[ii])).Tag;
                    this.QueryConditionCol2 = ci;
                    ((System.Windows.Forms.TextBox)sender).Text = ci.Table + "." + ci.Title;
                }
            }
        }

        private void updateQueryUpdateColumnsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                updateQuerySetValueToColumnRichTextBox.Text = ((dbqArgument)updateQueryUpdateColumnsListBox.SelectedItem).Value;
            }
            catch
            {

            }
        }

        private void saveQueryButton_Click(object sender, EventArgs e)
        {
            if (generateQueryTabControl.SelectedTab == selectQueryTabPage)
            {
                if (QueryTitleTextBox.Text != "")
                {
                    this.CurrentSelectQuery.Title = QueryTitleTextBox.Text;
                }
                else
                {
                    
                    this.CurrentSelectQuery.Title = "Select Query " + this.rn.Next(0, 9999);
                }
                this.SavedQueriesList.Add(this.CurrentSelectQuery);
                this.SavedSelectQueriesList.Add(this.CurrentSelectQuery);
            }
            else if (generateQueryTabControl.SelectedTab == insertQueryTabPage)
            {
                if (QueryTitleTextBox.Text != "")
                {
                    this.CurrentInsertQuery.Title = QueryTitleTextBox.Text;
                }
                else
                {
                    
                    this.CurrentInsertQuery.Title = "Insert Query " + this.rn.Next(0, 9999);
                }
                this.SavedQueriesList.Add(this.CurrentInsertQuery);
            }
            else if (generateQueryTabControl.SelectedTab == updateQueryTabPage)
            {
                if (QueryTitleTextBox.Text != "")
                {
                    this.CurrentUpdateQuery.Title = QueryTitleTextBox.Text;
                }
                else
                {
                    
                    this.CurrentUpdateQuery.Title = "Update Query " + this.rn.Next(0, 9999);
                }
                this.SavedQueriesList.Add(this.CurrentUpdateQuery);
            }
            savedQueriesListBox.DataSource = null;
            savedQueriesListBox.DataSource = this.SavedQueriesList;
            savedQueriesListBox.DisplayMember = "Title";
            QueryTitleTextBox.Text = "";
            selectQuerySavedQueryComboBox.DataSource = null;
            selectQuerySavedQueryComboBox.DataSource = this.SavedSelectQueriesList;
            selectQuerySavedQueryComboBox.DisplayMember = "Title";
            updateQuerySavedQueriesComboBox.DataSource = null;
            updateQuerySavedQueriesComboBox.DataSource = this.SavedSelectQueriesList;
            updateQuerySavedQueriesComboBox.DisplayMember = "Title";
        }

        private void savedQueriesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try{}
            catch{}
        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (generateQueryTabControl.SelectedTab == selectQueryTabPage && this.CurrentSelectQuery != null)
            {
                richTextBox1.Text = this.CurrentSelectQuery.generateQuery();
                SqlDataReader r = controller.dbcnf.execQuery(this.CurrentSelectQuery, dbConnect.queryMode.normalSelectQuery);
                DataSet ds = new DataSet();
                DataTable dt = new DataTable("Table1");
                dt.Clear();
                ds.Tables.Add(dt);
                ds.Load(r, LoadOption.PreserveChanges, ds.Tables[0]);
                dataGridView1.DataSource = ds.Tables[0];
                expandPanel();
            }
            else if (generateQueryTabControl.SelectedTab == insertQueryTabPage && this.CurrentInsertQuery != null)
            {
                richTextBox1.Text = this.CurrentInsertQuery.generateQuery();
                int i = controller.dbcnf.execQuery(this.CurrentInsertQuery);
                if (i >= 0)
                {
                    MessageBox.Show("record inserted");
                }
            }
            else if (generateQueryTabControl.SelectedTab == updateQueryTabPage && this.CurrentUpdateQuery != null)
            {
                richTextBox1.Text = this.CurrentUpdateQuery.generateQuery();

                if (controller.dbcnf.execQuery(this.CurrentUpdateQuery))
                {
                    MessageBox.Show("record updated");
                }
            }
        }

        private void closePictureBox_Click(object sender, EventArgs e)
        {
            collapsPanel();
        }

        private void expandPanel()
        {
            while (panel1.Height != 500)
            {
                panel1.Height = panel1.Height + 20;
                Thread.Sleep(10);
            }
        }
        private void collapsPanel()
        {
            while (panel1.Height != 0)
            {
                panel1.Height = panel1.Height - 20;
                Thread.Sleep(10);
            }
        }



   



       

        
    }
}
