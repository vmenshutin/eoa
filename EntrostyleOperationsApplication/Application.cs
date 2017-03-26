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

        OdbcDataAdapter SOItemDetailsAdapter;
        DataSet SOItemDetailsDataSet = new DataSet();

        OdbcDataAdapter SODifotAdapter;
        DataSet SODifotDataSet = new DataSet();

        bool isSODetailsGridStyled = false;
        bool isDifotPickerOpen = false;

        DateTime difotPickerValue;

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

            // load DIFOT data
            loadDifotData();

            // add fake dropdown columns for dispatch status and method + time columns + calendar columns
            addStatusAndMethodDropdowns(SOMain);
            addStatusAndMethodDropdowns(SOSecondary);

            // set basic grid styling
            setDataGridViewStyleProps(SOMain);
            setDataGridViewStyleProps(SOSecondary);
            setDataGridViewStyleProps(SODifot);

            // customize columns for split 1 and 2
            styleMainDataGridViewColumns(SOMain);
            styleMainDataGridViewColumns(SOSecondary);

            // split 1 and 2 condiional styling
            SOMain.CellFormatting += SOMain_CellFormatting;
            SOSecondary.CellFormatting += SOMain_CellFormatting;

            // customize columns for DIFOT
            styleDifotColumns();

            // focus main grid or secondary if main has 0 rows
            focusSO();
        }

        private void SOMain_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgv = (DataGridView)sender;

            // Stock coloring
            if (e.ColumnIndex == dgv.Columns["STOCK"].Index)
            {
                var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (cell.Value.ToString() == "IS")
                {
                    cell.Style.ForeColor = Color.Green;
                }
                else if (cell.Value.ToString() == "NIS")
                {
                    cell.Style.ForeColor = Color.Red;
                }
                else if (cell.Value.ToString() == "NowIS")
                {
                    cell.Style.ForeColor = Color.Blue;
                }
            }
            // STATUS
            else if (e.ColumnIndex == dgv.Columns["STATUS_FAKE"].Index)
            {
                var row = dgv.Rows[e.RowIndex];
                var cell = row.Cells[e.ColumnIndex];

                if (cell.Value.ToString() == "P")
                {
                    cell.Style.ForeColor = Color.Green;
                    row.DefaultCellStyle.BackColor = Color.Honeydew;
                }
                else if (cell.Value.ToString() == "TP")
                {
                    cell.Style.ForeColor = Color.Green;
                }
                else if (cell.Value.ToString() == "W" || cell.Value.ToString() == "Sc")
                {
                    cell.Style.ForeColor = Color.Blue;
                }
            }
            // Date, Time, Due color
            else if (e.ColumnIndex == dgv.Columns["PICKDATE_FAKE"].Index)
            {
                var row = dgv.Rows[e.RowIndex];
                var dateCell = row.Cells[e.ColumnIndex];

                if (dateCell.Value != null && dateCell.Value.ToString() != String.Empty)
                {
                    var timeCell = row.Cells["DUETIME_FAKE"];

                    if (timeCell.Value != null && timeCell.Value.ToString() != String.Empty)
                    {
                        var dueCell = row.Cells["DUEDATE_FAKE"];
                        var ttCell = row.Cells["TIME_FAKE"];

                        dateCell.Style.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
                        timeCell.Style.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
                        dueCell.Style.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);

                        if (ttCell.Value != null && ttCell.Value.ToString() != String.Empty)
                        {
                            if (((DateTime)dateCell.Value).Date < DateTime.Now.Date
                                || (((DateTime)dateCell.Value).Date == DateTime.Now.Date
                                && ((DateTime)timeCell.Value).TimeOfDay < DateTime.Now.TimeOfDay))
                            {
                                dateCell.Style.ForeColor = Color.Red;
                                timeCell.Style.ForeColor = Color.Red;
                                dueCell.Style.ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }
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
            // turn off grid listeners
            SOMain.CellValueChanged -= SO_CellValueChanged;
            SOMain.RowEnter -= SO_RowEnter;

            string sortString = " ORDER BY CASE WHEN STATUS = 'P' THEN '1' " +
              "WHEN STATUS = 'TP' THEN '2' " +
              "WHEN STATUS = 'W' THEN '3' " +
              "WHEN STATUS = 'TA' THEN '4' " +
              "WHEN STATUS = 'Sc' THEN '5' " +
              "ELSE STATUS END ASC, " +
              "CAST(PICKDATE AS DATE) ASC, " +
              "CAST(DUETIME AS TIME) ASC";

            (new OdbcCommand("exec query_salesorders_main " + sessionId.ToString(), connection)).ExecuteNonQuery();

            SOMainAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SALESORD_MAIN where SESSIONID = " + sessionId.ToString() + sortString, connection);
            SOMainDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOMainAdapter);
            SOMainAdapter.Fill(SOMainDataSet);

            SOMain.DataSource = SOMainDataSet.Tables[0];

            // turn grid listeners back on
            SOMain.CellValueChanged += SO_CellValueChanged;
            SOMain.RowEnter += SO_RowEnter;
        }

        // loads data to main sales orders table
        private void loadSalesOrdersSecondary(string searchText = "")
        {
            // turn grid listeners off
            SOSecondary.CellValueChanged -= SO_CellValueChanged;
            SOSecondary.RowEnter -= SO_RowEnter;

            (new OdbcCommand("exec " + (searchText.Length > 0 ? "secondary_search_orders " : "query_salesorders_secondary ") + sessionId.ToString() +
                (searchText.Length > 0 ? (", '" + searchText + "'"): ""), connection)).ExecuteNonQuery();

            SOSecondaryAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SALESORD_SECONDARY where SESSIONID = " + sessionId.ToString(), connection);
            SOSecondaryDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOSecondaryAdapter);
            SOSecondaryAdapter.Fill(SOSecondaryDataSet);

            SOSecondary.DataSource = SOSecondaryDataSet.Tables[0];

            // turn grid listeners on again
            SOSecondary.CellValueChanged += SO_CellValueChanged;
            SOSecondary.RowEnter += SO_RowEnter;
        }

        // loads stock items for a selected sales order
        private void loadSalesOrderItemDetails(int seqno)
        {
            // turn grid listeners off
            SOItemDetails.CellValueChanged -= SODetails_CellValueChanged;

            (new OdbcCommand("exec eoa_fetch_so_item_details " + sessionId.ToString() + ", " + seqno.ToString(), connection)).ExecuteNonQuery();

            SOItemDetailsAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SO_ITEM_DETAILS where SESSIONID = " + sessionId.ToString(), connection);
            SOItemDetailsDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOItemDetailsAdapter);
            SOItemDetailsAdapter.Fill(SOItemDetailsDataSet);

            SOItemDetails.DataSource = SOItemDetailsDataSet.Tables[0];

            // turn grid listeners on again
            SOItemDetails.CellValueChanged += SODetails_CellValueChanged;
        }

        // load DIFOT data
        private void loadDifotData()
        {
            PleaseWaitForm wait = new PleaseWaitForm();
            wait.Show();

            System.Windows.Forms.Application.DoEvents();

            // turn grid listeners off
            SODifot.CellValueChanged -= SODifot_CellValueChanged;

            (new OdbcCommand("exec eoa_query_difot_items '" + difotPattern.Text + "', '" + difotFrom.Value.ToString("yyyy-MM-dd") + "', '"
               + difotTo.Value.ToString("yyyy-MM-dd") + "'," + sessionId.ToString(), connection)).ExecuteNonQuery();

            SODifotAdapter = new OdbcDataAdapter("SELECT * FROM EOA_DIFOT where SESSIONID = " + sessionId.ToString(), connection);
            SODifotDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SODifotAdapter);
            SODifotAdapter.Fill(SODifotDataSet);

            SODifot.DataSource = SODifotDataSet.Tables[0];

            wait.Close();

            // turn grid listeners on again
            SODifot.CellValueChanged += SODifot_CellValueChanged;
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


            var listSource = new string[] { "TP", "W", "P", "TA", "Sc" };
            dispatchStatusColumn.DataSource = listSource;
            dispatchStatusColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            dgv.Columns.Add(dispatchStatusColumn);
            dgv.Columns[dispatchStatusColumn.Name].DataPropertyName = "STATUS";
            dgv.Columns[dispatchStatusColumn.Name].DisplayIndex = 3;

            //----------------------METHOD-----------------------

            var dispatchMethodColumn = new DataGridViewComboBoxColumn();
            dispatchMethodColumn.HeaderText = "Method";
            dispatchMethodColumn.Name = "METHOD_FAKE";

            var methodSource = new string[] { "E4", "G", "P", "N", "(n)" };
            dispatchMethodColumn.DataSource = methodSource;
            dispatchMethodColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            dgv.Columns.Add(dispatchMethodColumn);
            dgv.Columns[dispatchMethodColumn.Name].DataPropertyName = "METHOD";
            dgv.Columns[dispatchMethodColumn.Name].DisplayIndex = 4;

            //--------------------DateTimePickers-----------------

            CalendarColumn dateCalenderColumn = new CalendarColumn();
            CalendarColumn dueCalenderColumn = new CalendarColumn();

            dateCalenderColumn.HeaderText = "Date";
            dueCalenderColumn.HeaderText = "Due";

            dateCalenderColumn.Name = "PICKDATE_FAKE";
            dueCalenderColumn.Name = "DUEDATE_FAKE";

            dgv.Columns.Add(dateCalenderColumn);
            dgv.Columns.Add(dueCalenderColumn);

            dgv.Columns[dueCalenderColumn.Name].DataPropertyName = "DUEDATE";
            dgv.Columns[dateCalenderColumn.Name].DataPropertyName = "PICKDATE";

            dgv.Columns[dueCalenderColumn.Name].DisplayIndex = 9;
            dgv.Columns[dateCalenderColumn.Name].DisplayIndex = 7;

            //--------------------DateTimePickers-----------------

            TimeColumn timeColumn = new TimeColumn();
            TimeColumn dueTimeColumn = new TimeColumn();

            timeColumn.HeaderText = "TT";
            dueTimeColumn.HeaderText = "Time";

            timeColumn.Name = "TIME_FAKE";
            dueTimeColumn.Name = "DUETIME_FAKE";

            dgv.Columns.Add(timeColumn);
            dgv.Columns.Add(dueTimeColumn);

            dgv.Columns[timeColumn.Name].DataPropertyName = "TIME";
            dgv.Columns[dueTimeColumn.Name].DataPropertyName = "DUETIME";

            dgv.Columns[timeColumn.Name].DisplayIndex = 6;
            dgv.Columns[dueTimeColumn.Name].DisplayIndex = 8;
        }

        // style Data Grid View columns for SO Details grid
        private void styleSODetailsColumns()
        {
            var columns = SOItemDetails.Columns;

            foreach (DataGridViewColumn column in columns)
            {
                if (column.Name == "PICK_NOW")
                {
                    column.DefaultCellStyle.BackColor = Color.LightGray;
                }
                else
                {
                    column.ReadOnly = true;
                }
            }

            columns["SEQNO"].Visible = false;
            columns["LINES_ID"].Visible = false;
            columns["SESSIONID"].Visible = false;

            columns["STOCKCODE"].HeaderText = "Stock Code";
            columns["DESCRIPTION"].HeaderText = "Description";
            columns["STOCKCHECK"].HeaderText = "Stock Status";
            columns["PICK_NOW"].HeaderText = "Pick Qty";
            columns["UNSUP_QUANT"].HeaderText = "Outstanding";
            columns["TOTALSTOCK"].HeaderText = "Total Qty";
        }

        // style Data Grid View columns for split 1 and 2
        private void styleMainDataGridViewColumns(DataGridView dgv)
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
            columns["DUEDATE"].Visible = false;
            columns["PICKDATE"].Visible = false;
            columns["TIME"].Visible = false;
            columns["DUETIME"].Visible = false;

            columns["ACCOUNTNAME"].HeaderText = "Account";
            columns["STOCK"].HeaderText = "Stock";

            columns["ACCOUNTNAME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns["ACCOUNTNAME"].MinimumWidth = 200;
        }

        // style Data Grid View columns for DIFOT
        private void styleDifotColumns()
        {
            var columns = SODifot.Columns;

            foreach (DataGridViewColumn column in columns)
            {
                if (column.Name != "X_DIFOT_STATUS"
                    && column.Name != "X_DIFOT_TIMESTAMP"
                    && column.Name != "X_DIFOT_NOTE")
                {
                    column.ReadOnly = true;
                }
                else
                {
                    column.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            columns["SESSIONID"].Visible = false;

            columns["INVNO"].HeaderText = "Invoice #";
            columns["ACCOUNTNAME"].HeaderText = "Account";
            columns["X_DESPATCH_METHOD"].HeaderText = "DM";
            columns["X_LEAD_TIME"].HeaderText = "Time";
            columns["X_SCHEDULE_TIMESTAMP"].HeaderText = "Last Sc";
            columns["X_DIFOT_STATUS"].HeaderText = "Difot";
            columns["X_DIFOT_TIMESTAMP"].HeaderText = "Difot Time";
            columns["X_DIFOT_NOTE"].HeaderText = "Difot Note";

            columns["ACCOUNTNAME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns["ACCOUNTNAME"].MinimumWidth = 300;

            columns["X_DIFOT_NOTE"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns["X_DIFOT_NOTE"].MinimumWidth = 500;

            columns["X_LEAD_TIME"].DefaultCellStyle.Format = "dd/MM HH:mm";
            columns["X_SCHEDULE_TIMESTAMP"].DefaultCellStyle.Format = "dd/MM HH:mm";
            columns["X_DIFOT_TIMESTAMP"].DefaultCellStyle.Format = "dd/MM HH:mm";
        }

        // set major style properties for datagridview
        private void setDataGridViewStyleProps(DataGridView dgv)
        {
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            dgv.MultiSelect = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.DefaultCellStyle.SelectionBackColor = Color.AliceBlue;
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
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        // react when selected row changed
        private void SO_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var selectedRows = ((DataGridView)sender).SelectedRows;

            if (selectedRows.Count > 0)
            {
                var cells = selectedRows[0].Cells;

                // load sales order details
                loadSalesOrderItemDetails((int)cells["#"].Value);

                // customize columns for SO details grid
                if (!isSODetailsGridStyled)
                {
                    setDataGridViewStyleProps(SOItemDetails);
                    styleSODetailsColumns();
                    isSODetailsGridStyled = true;
                }

                label6.Text = cells["#"].Value.ToString();
                label7.Text = cells["ACCOUNTNAME"].Value.ToString();
                label8.Text = cells["ADDRESS1"].Value.ToString();
                label9.Text = cells["ADDRESS2"].Value.ToString();
                label10.Text = cells["LAST_SCHEDULED"].Value.ToString();
                label11.Text = cells["DIFOT_TIMESTAMP"].Value.ToString();
            }
        }

        // triggers when pick now qty is changes in sales order details grid
        private void SODetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                Validate();

                try
                {
                    SOItemDetailsAdapter.Update(SOItemDetailsDataSet);
                }
                catch (DBConcurrencyException)
                {
                    MessageBox.Show("Concurrency violation! Please, refresh the app.");
                }

                // run stored procedure and update real database tables
                (new OdbcCommand("eoa_so_item_details_update "
                   + sessionId.ToString() + ", "
                   + SOItemDetails.Rows[e.RowIndex].Cells["SEQNO"].Value.ToString() + ", "
                   + SOItemDetails.Rows[e.RowIndex].Cells["LINES_ID"].Value.ToString(), connection)).ExecuteNonQueryAsync();
                }
        }

        private void SODifot_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var dgv = (DataGridView)sender;

                Validate();

                try
                {
                    SODifotAdapter.Update(SODifotDataSet);
                }
                catch (DBConcurrencyException)
                {
                    MessageBox.Show("Concurrency violation! Please, refresh the app.");
                }

                // run stored procedure and update real database tables
                (new OdbcCommand("exec eoa_update_difot_"
                   + dgv.Columns[e.ColumnIndex].Name + " "
                   + sessionId.ToString() + ", "
                   + dgv.Rows[e.RowIndex].Cells["INVNO"].Value.ToString(), connection)).ExecuteNonQueryAsync();
            }
        }

        // updates database when cell value changed (split 1 and split 2)
        private void SO_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
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
                   + dgv.Rows[e.RowIndex].Cells["#"].Value.ToString(), connection)).ExecuteNonQueryAsync();
            }
        }

        // Refresh Button click
        private void button1_Click(object sender, EventArgs e)
        {
            loadSalesOrdersMain();
            loadSalesOrdersSecondary();
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            loadSalesOrdersSecondary(((TextBox)sender).Text);
        }

        private void clearSearchBtn_Click(object sender, EventArgs e)
        {
            searchBox.TextChanged -= searchBox_TextChanged;
            searchBox.Text = "";
            searchBox.TextChanged += searchBox_TextChanged;

            loadSalesOrdersSecondary();
        }

        private void refreshDifot_Click(object sender, EventArgs e)
        {
            loadDifotData();
        }
    }
}
