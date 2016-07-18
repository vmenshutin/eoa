using System;
using System.Windows.Forms;
using System.IO;
using System.Data.Odbc;
using System.Data;
using System.Drawing;

namespace EntrostyleOperationsApplication
{
    public partial class Application : Form
    {

        int sessionId;
        OdbcConnection connection = new OdbcConnection();

        OdbcDataAdapter SOMainAdapter;
        DataSet SOMainDataSet = new DataSet();

        OdbcDataAdapter SOSecondaryAdapter;
        DataSet SOSecondaryDataSet = new DataSet();

        public Application()
        {
            InitializeComponent();

            // read connection string and open the connection
            connection.ConnectionString = readConfig();
            connection.Open();

            // generate temporary session id and make sure it is unique
            sessionId = generateSessionId();
            verifyUserIsUnique();

            // load main and secondary sales orders grids
            loadSalesOrdersMain();
            loadSalesOrdersSecondary();

            // add fake dropdown columns for dispatch status and method
            addStatusAndMethodDropdowns(SOMain);
            addStatusAndMethodDropdowns(SOSecondary);

            // set basic grid styling
            setDataGridViewStyleProps(SOMain);
            setDataGridViewStyleProps(SOSecondary);

            // customize columns
            styleDataGridViewColumns(SOMain);
            styleDataGridViewColumns(SOSecondary);

            // set up action listeners
            setDataGridViewListeners();

            // focus main grid or secondary if main has 0 rows
            focusSO();
        }

        // reads config txt
        private string readConfig()
        {
            using (StreamReader sr = new StreamReader("config.txt"))
            {
                string connectionString = sr.ReadToEnd();
                int serverNamePrefixIndex = connectionString.IndexOf("Data Source=");
                int serverNameIndex = serverNamePrefixIndex + 12;
                int databaseNamePrefixIndex = connectionString.IndexOf(";Initial Catalog=");

                string serverName = connectionString.Substring(serverNameIndex, databaseNamePrefixIndex - serverNameIndex);

                int databaseNameIndex = databaseNamePrefixIndex + 17;
                int securityPrefixIndex = connectionString.IndexOf(";Persist Security Info=");

                string databaseName = connectionString.Substring(databaseNameIndex, securityPrefixIndex - databaseNameIndex);

                int loginPrefixIndex = connectionString.IndexOf("User ID=");
                int loginIndex = loginPrefixIndex + 8;
                int passwordPrefixIndex = connectionString.IndexOf(";Password=");

                string loginName = connectionString.Substring(loginIndex, passwordPrefixIndex - loginIndex);

                int passwordIndex = passwordPrefixIndex + 10;
                int passwordEndIndex = connectionString.IndexOf("\"\r\n\r\n#");

                string passwordName = connectionString.Substring(passwordIndex, passwordEndIndex - passwordIndex);

                return "Driver={SQL Server};Server=" + serverName + ";UID=" + loginName + ";PWD=" + passwordName + ";Database=" + databaseName + ";";
            }
        }

        // focus main grid or secondary if main has 0 rows
        private void focusSO()
        {
            if (SOMain.Rows.Count > 0)
            {
                SOMain.Rows[0].Selected = true;
            }
            else
            {
                SOSecondary.Rows[0].Selected = true;
            }
        }

        // Generates randon int number for unique session id
        private int generateSessionId()
        {
            return (new Random()).Next(0, 9999);
        }

        // loads data to main sales orders table
        private void loadSalesOrdersMain()
        {
            (new OdbcCommand("exec query_salesorders_main " + sessionId.ToString(), connection)).ExecuteNonQuery();

            SOMainAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SALESORD_MAIN where SESSIONID = " + sessionId.ToString(), connection);
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOMainAdapter);
            SOMainAdapter.Fill(SOMainDataSet);

            SOMain.DataSource = SOMainDataSet.Tables[0];
        }

        // loads data to main sales orders table
        private void loadSalesOrdersSecondary()
        {
            (new OdbcCommand("exec query_salesorders_secondary " + sessionId.ToString(), connection)).ExecuteNonQuery();

            SOSecondaryAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SALESORD_SECONDARY where SESSIONID = " + sessionId.ToString(), connection);
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOSecondaryAdapter);
            SOSecondaryAdapter.Fill(SOSecondaryDataSet);

            SOSecondary.DataSource = SOSecondaryDataSet.Tables[0];
        }

        // clear obsolete sessions data and verify current session id is unique
        private void verifyUserIsUnique()
        {
            (new OdbcCommand("exec clear_obsolete_sessions", connection)).ExecuteNonQuery();

            try
            {
                (new OdbcCommand("exec eoa_authentificate " + sessionId.ToString(), connection)).ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Authentification failed. Please, restart the application");
            }
        }

        // add dispatch status and method dropdowns
        private void addStatusAndMethodDropdowns(DataGridView dgv)
        {
            var dispatchStatusColumn = new DataGridViewComboBoxColumn();
            dispatchStatusColumn.HeaderText = "Status";
            dispatchStatusColumn.Name = "STATUS_FAKE";


            var listSource = new string[] { "TP", "W", "P", "TA", "Sc", "Sh" };
            dispatchStatusColumn.DataSource = listSource;
            dispatchStatusColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            dgv.Columns.Add(dispatchStatusColumn);
            dgv.Columns[dispatchStatusColumn.Name].DataPropertyName = "STATUS";

            //----------------------METHOD-----------------------

            var dispatchMethodColumn = new DataGridViewComboBoxColumn();
            dispatchMethodColumn.HeaderText = "Method";
            dispatchMethodColumn.Name = "METHOD_FAKE";

            var methodSource = new string[] { "E4", "E4c", "G", "P", "I", "(n)" };
            dispatchMethodColumn.DataSource = methodSource;
            dispatchMethodColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            dgv.Columns.Add(dispatchMethodColumn);
            dgv.Columns[dispatchMethodColumn.Name].DataPropertyName = "METHOD";
        }

        // style Data Grid View columns
        private void styleDataGridViewColumns(DataGridView dgv)
        {
            var columns = dgv.Columns;

            foreach (DataGridViewColumn column in columns)
            {
                if (column.Name != "#"
                    && column.Name != "ACCOUNTNAME"
                    && column.Name != "STOCK")
                {
                    column.DefaultCellStyle.BackColor = Color.LightGray;
                }
                else
                {
                    column.ReadOnly = true;
                }
            }

            columns["STATUS"].Visible = false;
            columns["METHOD"].Visible = false;
            columns["SESSIONID"].Visible = false;
            columns["DIFOT_TIMESTAMP"].Visible = false;
            columns["LAST_SCHEDULED"].Visible = false;
            columns["ADDRESS1"].Visible = false;
            columns["ADDRESS2"].Visible = false;
        }

        // set major style properties for datagridview
        private void setDataGridViewStyleProps(DataGridView dgv)
        {
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            dgv.MultiSelect = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.DefaultCellStyle.SelectionBackColor = Color.White;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToDeleteRows = false;

            //Disable custom column sorting for all splits
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        // set listeners to datagridviews
        private void setDataGridViewListeners()
        {
            // save values to database with each call value change
            SOMain.CellValueChanged += SO_CellValueChanged;
            SOSecondary.CellValueChanged += SO_CellValueChanged;

            // react when selected row changed
            SOMain.RowEnter += SO_RowEnter;
            SOSecondary.RowEnter += SO_RowEnter;
        }

        // react when selected row changed
        private void SO_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var selectedRows = ((DataGridView)sender).SelectedRows;

            if (selectedRows.Count > 0)
            {
                var cells = selectedRows[0].Cells;

                label6.Text = cells["#"].Value.ToString();
                label7.Text = cells["ACCOUNTNAME"].Value.ToString();
                label8.Text = cells["ADDRESS1"].Value.ToString();
                label9.Text = cells["ADDRESS2"].Value.ToString();
                label10.Text = cells["LAST_SCHEDULED"].Value.ToString();
                label11.Text = cells["DIFOT_TIMESTAMP"].Value.ToString();
            }
        }

        // updates database when 
        private void SO_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Validate();

            // which grid to update
            var dgv = (DataGridView)sender;

            // which column value has been changed
            string columnName = dgv.Columns[e.ColumnIndex].Name;

            // is column fake?
            int index = columnName.IndexOf("_FAKE");

            // remove _FAKE part of column name if it exists
            string cleanColumnName = (index < 0)
                ? columnName
                : columnName.Remove(index, 5);

            // set corresponding data adapters and data sets
            var adapter = dgv.Name == "SOMain" ? SOMainAdapter : SOSecondaryAdapter;
            var dataset = dgv.Name == "SOMain" ? SOMainDataSet : SOSecondaryDataSet;

            // set corresponding stored procedure name prefix
            string procedureNamePrefix = dgv.Name == "SOMain" ? "exec so_main_update_" : "exec so_secondary_update_";

            try
            {
                adapter.Update(dataset);
            }
            catch (DBConcurrencyException)
            {
                MessageBox.Show("Concurrency violation! Please, refresh the app.");
            }

            // run stored procedure and update real database tables
            (new OdbcCommand(procedureNamePrefix
               + cleanColumnName + " "
               + sessionId.ToString() + ", "
               + dgv.Rows[e.RowIndex].Cells["#"].Value.ToString(), connection)).ExecuteNonQuery();
        }
    }
}
