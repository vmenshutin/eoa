﻿using System;
using System.Windows.Forms;
using System.IO;
using System.Data.Odbc;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Drawing.Printing;
using Microsoft.Reporting.WinForms;
using ZXing;
using ZXing.Common;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        OdbcDataAdapter SOSettingsAdapter;
        DataSet SOSettingsDataSet = new DataSet();

        OdbcDataAdapter SONarrativeAdapter;
        DataSet SONarrativeDataSet = new DataSet();

        bool isSODetailsGridStyled = false;

        string activeGrid = null;

        private LocalReport shelfReport;
        private LocalReport stockReport;

        private int m_currentPageIndex;
        private IList<Stream> m_streams;

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

            // load SHELF data
            populateShelfCombobox();

            // load STOCK data
            populateStockData();
            setDataGridViewStyleProps(stockLblDataGridView);

            // load SETTINGS
            loadSettings();

            // add fake dropdown columns for dispatch status and method + time columns + calendar columns
            addStatusAndMethodDropdowns(SOMain);
            addStatusAndMethodDropdowns(SOSecondary);
            addDifotDropDown(SODifot);

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

            // bind to focus events to determine active grid
            SOMain.Enter += SOMain_Enter;
            SOSecondary.Enter += SOMain_Enter;

            // EXO integration. On hyperlinks click
            SOMain.CellMouseClick += SOMain_CellMouseClick;
            SOSecondary.CellMouseClick += SOMain_CellMouseClick;
            SOItemDetails.CellMouseClick += SOMain_CellMouseClick;
            SOItemDetails.CellValidating += SOItemDetails_CellValidating;
            SOItemDetails.DataError += SOItemDetails_DataError;
            SODifot.CellMouseClick += SOMain_CellMouseClick;

            // so item details conditional styling
            SOItemDetails.CellFormatting += SOItemDetails_CellFormatting;
            SOItemDetails.Leave += SOItemDetails_Leave;

            // customize columns for DIFOT
            styleDifotColumns();

            // focus main grid or secondary if main has 0 rows
            focusSO();

            // listen to app protocol handler
            listenToAppProtocolHandler();
        }

        private void SOItemDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Prevents system error messages since cell level validation occurs in cell validating event
        }

        private void SOItemDetails_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == SOItemDetails.Columns["X_ACTION"].Index)
            {
                int i;

                // if cell is not empty
                if (e.FormattedValue.ToString() != "")
                {
                    var row = SOItemDetails.Rows[e.RowIndex];
                    int min = Math.Min(Int32.Parse(row.Cells["UNSUP_QUANT"].Value.ToString()), Int32.Parse(row.Cells["TOTALSTOCK"].Value.ToString()));

                    // if not numeric
                    if (!int.TryParse(Convert.ToString(e.FormattedValue), out i))
                    {
                        e.Cancel = true;
                        MessageBox.Show("       Only numeric characters are accepted.       ");
                    }
                    // if numeric - check if exceeds max
                    else if (Int32.Parse(e.FormattedValue.ToString()) > min)
                    {
                        e.Cancel = true;
                        MessageBox.Show("       Action should not exceed Outstanding and/or Location Qty.       ");
                    }
                }
            }

            if (e.ColumnIndex == SOItemDetails.Columns["PICK_NOW"].Index)
            {
                int i;

                // if cell is not empty
                if (e.FormattedValue.ToString() != "")
                {
                    // if not numeric
                    if (!int.TryParse(Convert.ToString(e.FormattedValue), out i))
                    {
                        e.Cancel = true;
                        MessageBox.Show("       Only numeric characters are accepted.       ");
                    }
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show("       A numeric value must be specified.       ");
                }
            }
        }

        private void SOItemDetails_Leave(object sender, EventArgs e)
        {
            SOItemDetails.EndEdit();
        }

        private void SOMain_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dgv = (DataGridView)sender;
            var columnName = dgv.Columns[e.ColumnIndex].Name;

            if (columnName == "#" || columnName == "STOCKCODE")
            {
                var wait = showWaitForm();

                StringBuilder sb = new StringBuilder();
                //Starting Information for process like its path, use system shell i.e. control process by system etc.
                ProcessStartInfo psi = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe");
                // its states that system shell will not be used to control the process instead program will handle the process
                psi.UseShellExecute = false;
                psi.ErrorDialog = false;
                // Do not show command prompt window separately
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                //redirect all standard inout to program
                psi.RedirectStandardError = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                //create the process with above infor and start it
                Process plinkProcess = new Process();
                plinkProcess.StartInfo = psi;
                plinkProcess.Start();
                //link the streams to standard inout of process
                StreamWriter inputWriter = plinkProcess.StandardInput;
                StreamReader outputReader = plinkProcess.StandardOutput;
                StreamReader errorReader = plinkProcess.StandardError;
                //send command to cmd prompt and wait for command to execute with thread sleep
                if (e.RowIndex != -1)
                {
                    var line = columnName == "#"
                        ? @"START exo://saleorder(" + dgv.Rows[e.RowIndex].Cells["#"].Value.ToString() + ")"
                        : @"START exo://stockitem/?stockcode=" + dgv.Rows[e.RowIndex].Cells["STOCKCODE"].Value.ToString();
                    inputWriter.WriteLine(line);
                }

                wait.Close();
            }
        }

        private void process(string carrier)
        {
            updateCarrier(carrier);
            processPick();
        }

        private void printPickingBtn_Click(object sender, EventArgs e)
        {
            var row = getCurrentSORow();

            if (row != null)
            {
                var wait = showWaitForm();

                var preview = new PrintPickingDialog(row, settings_labelPrinter.Text, initSalesOrderReport); // new
                preview.Show();

                wait.Close();
            }
        }

        private void SOMain_Enter(object sender, EventArgs e)
        {
            activeGrid = ((DataGridView)sender).Name;
        }

        private void SOItemDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgv = (DataGridView)sender;

            // Stock Status coloring
            if (e.ColumnIndex == dgv.Columns["STOCKCHECK"].Index)
            {
                var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (cell.Value.ToString() == "IS")
                {
                    cell.Style.ForeColor = Color.Green;
                    cell.Style.SelectionForeColor = Color.Green;
                }
                else if (cell.Value.ToString() == "NIS" || cell.Value.ToString() == "OOS")
                {
                    cell.Style.ForeColor = Color.Red;
                    cell.Style.SelectionForeColor = Color.Red;
                }
                else if (cell.Value.ToString() == "Sh")
                {
                    cell.Style.ForeColor = Color.Gray;
                    cell.Style.SelectionForeColor = Color.Gray;
                }
                else if (cell.Value.ToString() == "TR")
                {
                    cell.Style.ForeColor = Color.Blue;
                    cell.Style.SelectionForeColor = Color.Blue;
                }
            }
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
                    cell.Style.SelectionForeColor = Color.Green;
                }
                else if (cell.Value.ToString() == "NIS")
                {
                    cell.Style.ForeColor = Color.Red;
                    cell.Style.SelectionForeColor = Color.Red;
                }
                else if (cell.Value.ToString() == "NowIS" || cell.Value.ToString() == "TR")
                {
                    cell.Style.ForeColor = Color.Blue;
                    cell.Style.SelectionForeColor = Color.Blue;
                }
            }
            // STATUS
            else if (e.ColumnIndex == dgv.Columns["STATUS_FAKE"].Index)
            {
                var row = dgv.Rows[e.RowIndex];
                var cell = row.Cells[e.ColumnIndex];

                string[] tp = { "TP-PICK", "TP-POWDER", "TP-PROJECT", "TP-KEY", "TP-CUT", "TP-SHIP", "TP-BULK" };

                if (cell.Value.ToString() == "P")
                {
                    cell.Style.ForeColor = Color.Green;
                    cell.Style.SelectionForeColor = Color.Green;
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.DefaultCellStyle.ForeColor = Color.Gray;
                }
                else if (Array.IndexOf(tp, cell.Value.ToString()) > -1)
                {
                    cell.Style.ForeColor = Color.Green;
                    cell.Style.SelectionForeColor = Color.Green;
                }
                else if (cell.Value.ToString() == "W" || cell.Value.ToString() == "Sc")
                {
                    cell.Style.ForeColor = Color.Blue;
                    cell.Style.SelectionForeColor = Color.Blue;
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

                        row.DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
                        row.Cells["#"].Style.Font = new Font("Arial", 11F, FontStyle.Underline, GraphicsUnit.Pixel);

                        if (((DateTime)dateCell.Value).Date < DateTime.Now.Date
                            || (((DateTime)dateCell.Value).Date == DateTime.Now.Date
                            && ((DateTime)timeCell.Value).TimeOfDay < DateTime.Now.TimeOfDay))
                        {
                            dateCell.Style.ForeColor = Color.Red;
                            timeCell.Style.ForeColor = Color.Red;
                            dueCell.Style.ForeColor = Color.Red;

                            dateCell.Style.SelectionForeColor = Color.Red;
                            timeCell.Style.SelectionForeColor = Color.Red;
                            dueCell.Style.SelectionForeColor = Color.Red;
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
              "WHEN STATUS = 'TP-PICK' THEN '2' " +
              "WHEN STATUS = 'TP-POWDER' THEN '2' " +
              "WHEN STATUS = 'TP-PROJECT' THEN '2' " +
              "WHEN STATUS = 'TP-KEY' THEN '2' " +
              "WHEN STATUS = 'TP-CUT' THEN '2' " +
              "WHEN STATUS = 'TP-SHIP' THEN '2' " +
              "WHEN STATUS = 'TP-BULK' THEN '2' " +
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
        private void loadSalesOrdersSecondary()
        {
            string searchText = searchBox.Text;

            string sortString = " ORDER BY CAST(DUEDATE AS DATE) ASC";

            // turn grid listeners off
            SOSecondary.CellValueChanged -= SO_CellValueChanged;
            SOSecondary.RowEnter -= SO_RowEnter;

            (new OdbcCommand("exec " + (searchText.Length > 0 ? "secondary_search_orders " : "query_salesorders_secondary ") + sessionId.ToString() +
                (searchText.Length > 0 ? (", '" + searchText + "'"): ""), connection)).ExecuteNonQuery();

            SOSecondaryAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SALESORD_SECONDARY where SESSIONID = " + sessionId.ToString() + sortString, connection);
            SOSecondaryDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOSecondaryAdapter);
            SOSecondaryAdapter.Fill(SOSecondaryDataSet);

            SOSecondary.DataSource = SOSecondaryDataSet.Tables[0];

            // turn grid listeners on again
            SOSecondary.CellValueChanged += SO_CellValueChanged;
            SOSecondary.RowEnter += SO_RowEnter;
        }

        // loads stock items for a selected sales order
        private void loadSalesOrderItemDetails(string seqno)
        {
            // turn grid listeners off
            SOItemDetails.CellValueChanged -= SODetails_CellValueChanged;

            (new OdbcCommand("exec eoa_fetch_so_item_details " + sessionId.ToString() + ", " + seqno, connection)).ExecuteNonQuery();

            SOItemDetailsAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SO_ITEM_DETAILS where SESSIONID = " + sessionId.ToString(), connection);
            SOItemDetailsDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOItemDetailsAdapter);
            SOItemDetailsAdapter.Fill(SOItemDetailsDataSet);

            SOItemDetails.DataSource = SOItemDetailsDataSet.Tables[0];

            // turn grid listeners on again
            SOItemDetails.CellValueChanged += SODetails_CellValueChanged;
        }

        // load SETTINGS
        private void loadSettings()
        {
            SOSettingsAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SETTINGS", connection);
            SOSettingsDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOSettingsAdapter);
            SOSettingsAdapter.Fill(SOSettingsDataSet);

            var settingsRow = SOSettingsDataSet.Tables[0].Rows[0];

            settings_printerName.Text = settingsRow["PRINTER_NAME"].ToString();
            settings_labelPrinter.Text = settingsRow["LABEL_PRINTER"].ToString();
        }

        // load DIFOT data
        private void loadDifotData()
        {
            var wait = showWaitForm();
            string searchText = difotSearchBox.Text;

            // turn grid listeners off
            SODifot.CellValueChanged -= SODifot_CellValueChanged;

            if (searchText != "")
            {
                (new OdbcCommand("exec eoa_query_difot_items_secondary '" + difotFrom.Value.ToString("yyyy-MM-dd") + "', '"
                + difotTo.Value.ToString("yyyy-MM-dd") + "', '" + searchText + "'," + sessionId.ToString(), connection)).ExecuteNonQuery();
            }
            else
            {
                (new OdbcCommand("exec eoa_query_difot_items '" + difotFrom.Value.ToString("yyyy-MM-dd") + "', '"
                + difotTo.Value.ToString("yyyy-MM-dd") + "'," + sessionId.ToString(), connection)).ExecuteNonQuery();
            }

            SODifotAdapter = new OdbcDataAdapter("SELECT * FROM EOA_DIFOT where SESSIONID = " + sessionId.ToString(), connection);
            SODifotDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SODifotAdapter);
            SODifotAdapter.Fill(SODifotDataSet);

            SODifot.DataSource = SODifotDataSet.Tables[0];

            wait.Close();

            // turn grid listeners on again
            SODifot.CellValueChanged += SODifot_CellValueChanged;
        }

        // populate STOCK data
        private void populateStockData()
        {
            stockLblDataGridView.ColumnCount = 4;

            // Item Code column
            stockLblDataGridView.Columns[0].Name = "ItemCode";
            stockLblDataGridView.Columns[0].HeaderText = "Item Code";
            stockLblDataGridView.Columns[0].ReadOnly = true;
            stockLblDataGridView.Columns[0].DefaultCellStyle.BackColor = Color.Silver;
            stockLblDataGridView.Columns[0].DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
            stockLblDataGridView.Columns[0].Width = 100;

            // Description column
            stockLblDataGridView.Columns[1].Name = "Description";
            stockLblDataGridView.Columns[1].MinimumWidth = 300;

            // Item Qty column
            stockLblDataGridView.Columns[2].Name = "ItemQty";
            stockLblDataGridView.Columns[2].HeaderText = "Item Qty";
            stockLblDataGridView.Columns[2].Width = 60;

            // Label Qty column
            stockLblDataGridView.Columns[3].Name = "LabelQty";
            stockLblDataGridView.Columns[3].HeaderText = "Label Qty";
            stockLblDataGridView.Columns[3].Width = 60;

            // STOCK combobox
            stockCombobox.DataSource = shelfCombobox.DataSource;
            stockCombobox.DisplayMember = "STOCKCODE";

            stockLblDataGridView.CellValidating += StockLblDataGridView_CellValidating;
        }

        private void initStockReportViewer(int rowIndex, bool initPreview)
        {
            var wait = showWaitForm();
            var selectedValue = stockLblDataGridView.Rows[rowIndex];

            stockReport = new LocalReport();
            stockReport.ReportPath = @".\STOCK.rdlc";

            DataTable dt = new DataTable();

            dt.Columns.Add("STOCKCODE", typeof(String));
            dt.Columns.Add("DESCRIPTION", typeof(String));
            dt.Columns.Add("BARCODE", typeof(byte[]));

            DataRow dr = dt.NewRow();

            dr["STOCKCODE"] = selectedValue.Cells[0].Value.ToString();
            dr["DESCRIPTION"] = selectedValue.Cells[1].Value.ToString();

            Bitmap bitmap = GenerateBarcode(selectedValue.Cells[0].ToString(), 100, 100, 0);
            dr["BARCODE"] = (byte[])(new ImageConverter().ConvertTo(bitmap, typeof(byte[])));

            dt.Rows.Add(dr);

            stockReport.DataSources.Add(new ReportDataSource("DataSet1", dt));
            stockReport.SetParameters(new ReportParameter("ItemQty", selectedValue.Cells[2].Value.ToString()));

            if (initPreview)
            {
                stockReportViewer.ProcessingMode = ProcessingMode.Local;
                stockReportViewer.LocalReport.ReportPath = @".\STOCK.rdlc";
                stockReportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt));
                stockReportViewer.LocalReport.SetParameters(new ReportParameter("ItemQty", selectedValue.Cells[2].Value.ToString()));
                stockReportViewer.RefreshReport();
            }

            wait.Close();
        }

        private void StockLblDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                if (!int.TryParse(Convert.ToString(e.FormattedValue), out int i))
                {
                    e.Cancel = true;
                    MessageBox.Show("      Numeric values only!      ");
                }
                else if ((int.Parse(Convert.ToString(e.FormattedValue)) < 1) && e.ColumnIndex == 3)
                {
                    e.Cancel = true;
                    MessageBox.Show("      Positive numbers only!      ");
                }
            }
        }

        // populate SHELF data
        private void populateShelfCombobox()
        {
            var wait = showWaitForm();

            var adapter = new OdbcDataAdapter("SELECT STOCKCODE, DESCRIPTION FROM STOCK_ITEMS order by STOCKCODE", connection);
            var ds = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(adapter);
            adapter.Fill(ds);

            SODifot.DataSource = SODifotDataSet.Tables[0];
            shelfCombobox.DataSource = ds.Tables[0];
            shelfCombobox.DisplayMember = "STOCKCODE";

            initShelfReportViewer();
            shelfCombobox.SelectedValueChanged += shelfCombobox_SelectedValueChanged;

            wait.Close();
        }

        private void shelfCombobox_SelectedValueChanged(object sender, EventArgs e)
        {
            shelfReport.DataSources.RemoveAt(0);
            shelfLabelReportViewer.LocalReport.DataSources.RemoveAt(0);

            initShelfReportViewer();
        }

        private void initShelfReportViewer()
        {
            var selectedValue = (shelfCombobox.SelectedValue as DataRowView);

            if (selectedValue != null)
            {
                shelfLabelReportViewer.ProcessingMode = ProcessingMode.Local;
                shelfLabelReportViewer.LocalReport.ReportPath = @".\SHELF.rdlc";

                shelfReport = new LocalReport();
                shelfReport.ReportPath = @".\SHELF.rdlc";

                DataTable dt = new DataTable();

                dt.Columns.Add("STOCKCODE", typeof(String));
                dt.Columns.Add("DESCRIPTION", typeof(String));
                dt.Columns.Add("BARCODE", typeof(byte[]));

                DataRow dr = dt.NewRow();

                dr["STOCKCODE"] = selectedValue.Row[0];
                dr["DESCRIPTION"] = selectedValue.Row[1];

                Bitmap bitmap = GenerateBarcode(selectedValue.Row[0].ToString(), 100, 100, 0);
                dr["BARCODE"] = (byte[])(new ImageConverter().ConvertTo(bitmap, typeof(byte[])));

                dt.Rows.Add(dr);

                shelfReport.DataSources.Add(new ReportDataSource("DataSet1", dt));
                shelfLabelReportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt));

                shelfLabelReportViewer.RefreshReport();
            }
            else
            {
                MessageBox.Show("       Not a valid value       ");
            }
        }

        private void initSalesOrderReport()
        {
            var so = getCurrentSO();

            if (so != null)
            {
                var wait = showWaitForm();
                var soRow = getCurrentSORow();

                var salesOrderReport = new LocalReport();
                salesOrderReport.ReportPath = @".\sales_order.rdlc";

                // so_hdr + accs
                var adapter = new OdbcDataAdapter("exec get_sales_order_report " + so, connection);
                var ds1 = new DataSet();
                OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(adapter);
                adapter.Fill(ds1);

                var firstRow = ds1.Tables[0].Rows[0];

                // so_hdr seqno barcode
                ds1.Tables[0].Columns.Add("BARCODE", typeof(byte[]));
                firstRow["BARCODE"] = (byte[])(new ImageConverter().ConvertTo(GenerateBarcode(so, 200, 200, 0), typeof(byte[])));

                // so_hdr all address barcode
                var allAddress = string.Join(" ", new[] { firstRow["ADDRESS1"], firstRow["ADDRESS2"], firstRow["ADDRESS3"], firstRow["ADDRESS4"], firstRow["ADDRESS5"], firstRow["ADDRESS6"] });
                allAddress = Regex.Replace(allAddress, @"\s+", " ");
                ds1.Tables[0].Columns.Add("ADDRESS_BARCODE", typeof(byte[]));
                firstRow["ADDRESS_BARCODE"] = (byte[])(new ImageConverter().ConvertTo(GenerateBarcode(allAddress, 300, 300, 0), typeof(byte[])));

                salesOrderReport.DataSources.Add(new ReportDataSource("DataSet1", ds1.Tables[0]));
                // end of so_hdr + accs

                // salesord_lines
                // filter by location and pick qty
                var dataTable2 = SOItemDetailsDataSet.Tables[0];

                var rows = dataTable2.AsEnumerable()
                    .Where(r => ((float.Parse(r["PICK_NOW"].ToString()) > 0) || (r["STOCKCODE"].ToString()).Length == 0)
                        && r["LOCATION"].ToString()[0].Equals('1'));

                dataTable2 = rows.Any() ? rows.CopyToDataTable() : dataTable2.Clone();

                // remove all unnecessary columns
                for (int i = dataTable2.Columns.Count - 1; i >= 0; i--)
                {
                    string[] columnsToKeep = { "STOCKCODE", "DESCRIPTION", "PICK_NOW" };

                    if (!columnsToKeep.Contains<string>(dataTable2.Columns[i].ColumnName))
                    {
                        dataTable2.Columns.RemoveAt(i);
                    }
                }
                    
                // add barcode column
                dataTable2.Columns.Add("BARCODE", typeof(byte[]));

                // set barcode for each row
                foreach (DataRow row in dataTable2.Rows)
                {
                    if (row["STOCKCODE"].ToString().Length > 0)
                    {
                        row["BARCODE"] = (byte[])(new ImageConverter().ConvertTo(GenerateBarcode(row["STOCKCODE"].ToString(), 100, 100, 0), typeof(byte[])));
                    }
                }

                salesOrderReport.DataSources.Add(new ReportDataSource("DataSet2", dataTable2));
                // end of salesord_lines

                exportReport(salesOrderReport, 8, 10.7, 0.59, 0.59);
                prepareDocAndPrint(new PaperSize("Sales Order", 800, 1070), settings_printerName.Text, 1);

                soRow.DataGridView.Focus();
                soRow.Cells["STATUS"].Value = "P";

                wait.Close();
            }
            else
            {
                MessageBox.Show("       Please select a Sales Order       ");   
            }
        }

        private Bitmap GenerateBarcode(string barcodeText, int height, int width, int margin)
        {
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };

            return barcodeWriter.Write(barcodeText);
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

        // add difot/shipped late dropdown
        private void addDifotDropDown(DataGridView dgv)
        {
            var difotColumn = new DataGridViewComboBoxColumn();
            difotColumn.HeaderText = "Difot";
            difotColumn.Name = "DIFOT_FAKE";

            var listSource = new string[] { "difot", "shipped late" };
            difotColumn.DataSource = listSource;
            difotColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            dgv.Columns.Add(difotColumn);
            dgv.Columns[difotColumn.Name].DataPropertyName = "X_DIFOT_STATUS";
            dgv.Columns[difotColumn.Name].DisplayIndex = 3;
        }

        // add dispatch status and method dropdowns
        private void addStatusAndMethodDropdowns(DataGridView dgv)
        {
            var dispatchStatusColumn = new DataGridViewComboBoxColumn();
            dispatchStatusColumn.HeaderText = "Status";
            dispatchStatusColumn.Name = "STATUS_FAKE";


            var listSource = new string[] { "TP-PICK", "TP-POWDER", "TP-PROJECT", "TP-KEY", "TP-CUT", "TP-SHIP", "TP-BULK", "W", "P", "TA", "Sc" };
            dispatchStatusColumn.DataSource = listSource;
            dispatchStatusColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            dgv.Columns.Add(dispatchStatusColumn);
            dgv.Columns[dispatchStatusColumn.Name].DataPropertyName = "STATUS";
            dgv.Columns[dispatchStatusColumn.Name].DisplayIndex = 3;

            //----------------------METHOD-----------------------

            var dispatchMethodColumn = new DataGridViewComboBoxColumn();
            dispatchMethodColumn.HeaderText = "Method";
            dispatchMethodColumn.Name = "METHOD_FAKE";

            var methodSource = new string[] { "E1", "E4", "P", "N", "(n)" };
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

            TimeColumn dueTimeColumn = new TimeColumn();

            dueTimeColumn.HeaderText = "Time";

            dueTimeColumn.Name = "DUETIME_FAKE";

            dgv.Columns.Add(dueTimeColumn);
            dgv.Columns[dueTimeColumn.Name].DataPropertyName = "DUETIME";
            dgv.Columns[dueTimeColumn.Name].DisplayIndex = 8;
        }

        private void addLocationDropdown()
        {
            var locationAdapter = new OdbcDataAdapter("SELECT CONCAT(LOCNO, ' ', LCODE) FROM STOCK_LOCATIONS", connection);
            var locationDS = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(locationAdapter);
            locationAdapter.Fill(locationDS);

            DataRow[] rows = locationDS.Tables[0].Select();
            string[] optionsArray = rows.Select(row => row[0].ToString()).ToArray();

            var locationColumn = new DataGridViewComboBoxColumn();
            locationColumn.HeaderText = "Location";
            locationColumn.Name = "LOCATION_FAKE";
            locationColumn.DisplayIndex = 9;

            var locationSource = optionsArray;
            locationColumn.DataSource = locationSource;
            locationColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            SOItemDetails.Columns.Add(locationColumn);
            SOItemDetails.Columns[locationColumn.Name].DataPropertyName = "LOCATION";

            // location dropdown on dashboard
            locationComboBox.DataSource = optionsArray.Clone();
        }

        // style Data Grid View columns for SO Details grid
        private void styleSODetailsColumns()
        {
            var columns = SOItemDetails.Columns;

            foreach (DataGridViewColumn column in columns)
            {
                if (column.Name == "PICK_NOW"
                    || column.Name == "LOCATION_FAKE"
                    || column.Name == "X_ACTION")
                {
                    column.DefaultCellStyle.BackColor = Color.Silver;
                }
                else
                {
                    column.ReadOnly = true;
                }
            }

            columns["SEQNO"].Visible = false;
            columns["LINES_ID"].Visible = false;
            columns["SESSIONID"].Visible = false;
            columns["LOCATION"].Visible = false;

            columns["STOCKCODE"].HeaderText = "Stock Code";
            columns["STOCKCODE"].HeaderText = "Stock Code";
            columns["STOCKCODE"].HeaderText = "Stock Code";
            columns["STOCKCODE"].HeaderText = "Stock Code";
            columns["DESCRIPTION"].HeaderText = "Description";
            columns["STOCKCHECK"].HeaderText = "Stock Status";
            columns["PICK_NOW"].HeaderText = "Pick Qty";
            columns["UNSUP_QUANT"].HeaderText = "Outstanding";
            columns["TOTALSTOCK"].HeaderText = "Location Qty";
            columns["X_ACTION"].HeaderText = "Action";

            columns["X_ACTION"].DisplayIndex = 10;

            columns["STOCKCODE"].DefaultCellStyle.ForeColor = Color.Blue;
            columns["STOCKCODE"].DefaultCellStyle.SelectionForeColor = Color.Blue;
            columns["STOCKCODE"].DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Underline, GraphicsUnit.Pixel);
            columns["STOCKCHECK"].DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
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
                    column.DefaultCellStyle.BackColor = Color.Silver;
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
            columns["DUETIME"].Visible = false;
            columns["REFERENCE"].Visible = false;
            columns["X_PROJECTNAME"].Visible = false;
            columns["CUSTORDERNO"].Visible = false;
            columns["X_CARRIER"].Visible = false;

            columns["ACCOUNTNAME"].HeaderText = "Account";
            columns["STOCK"].HeaderText = "Stock";

            columns["ACCOUNTNAME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns["ACCOUNTNAME"].MinimumWidth = 200;

            columns["#"].DefaultCellStyle.ForeColor = Color.DarkRed;
            columns["#"].DefaultCellStyle.SelectionForeColor = Color.DarkRed;
            columns["#"].DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Underline, GraphicsUnit.Pixel);

            columns["STOCK"].DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        // style Data Grid View columns for DIFOT
        private void styleDifotColumns()
        {
            SODifot.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            var columns = SODifot.Columns;

            foreach (DataGridViewColumn column in columns)
            {
                if (column.Name != "DIFOT_FAKE"
                    && column.Name != "X_DIFOT_TIMESTAMP"
                    && column.Name != "X_DIFOT_NOTE")
                {
                    column.ReadOnly = true;
                }
                else
                {
                    column.DefaultCellStyle.BackColor = Color.Silver;
                }
            }

            columns["SESSIONID"].Visible = false;
            columns["X_DIFOT_STATUS"].Visible = false;

            columns["INVNO"].HeaderText = "Invoice #";
            columns["ACCOUNTNAME"].HeaderText = "Account";
            columns["X_DESPATCH_METHOD"].HeaderText = "DM";
            columns["X_LEAD_TIME"].HeaderText = "Time";
            columns["X_SCHEDULE_TIMESTAMP"].HeaderText = "Last Sc";
            columns["X_DIFOT_TIMESTAMP"].HeaderText = "Difot Time";
            columns["X_DIFOT_NOTE"].HeaderText = "Difot Note";

            columns["ACCOUNTNAME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns["ACCOUNTNAME"].MinimumWidth = 300;

            columns["X_DIFOT_NOTE"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            columns["X_DIFOT_NOTE"].MinimumWidth = 500;

            columns["X_LEAD_TIME"].DefaultCellStyle.Format = "dd/MM HH:mm";
            columns["X_SCHEDULE_TIMESTAMP"].DefaultCellStyle.Format = "dd/MM HH:mm";
            columns["X_DIFOT_TIMESTAMP"].DefaultCellStyle.Format = "dd/MM HH:mm";

            columns["#"].DefaultCellStyle.ForeColor = Color.DarkRed;
            columns["#"].DefaultCellStyle.SelectionForeColor = Color.DarkRed;
            columns["#"].DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Underline, GraphicsUnit.Pixel);
        }

        // set major style properties for datagridview
        private void setDataGridViewStyleProps(DataGridView dgv)
        {
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            dgv.MultiSelect = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.DefaultCellStyle.SelectionBackColor = Color.Wheat;
            dgv.DefaultCellStyle.SelectionForeColor = dgv.DefaultCellStyle.ForeColor;
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
                loadSalesOrderItemDetails(cells["#"].Value.ToString());

                // customize columns for SO details grid
                if (!isSODetailsGridStyled)
                {
                    addLocationDropdown();
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
                label14.Text = cells["REFERENCE"].Value.ToString();

                (new OdbcCommand("exec EOA_get_narrative " + cells["#"].Value.ToString() + ", " + sessionId.ToString(), connection)).ExecuteNonQuery();

                SONarrativeAdapter = new OdbcDataAdapter("SELECT * FROM EOA_NARRATIVE where SESSION_ID = " + sessionId.ToString(), connection);
                SONarrativeDataSet = new DataSet();
                OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SONarrativeAdapter);
                SONarrativeAdapter.Fill(SONarrativeDataSet);

                narrativeTextBox.Text = SONarrativeDataSet.Tables[0].Rows[0]["NARRATIVE"].ToString();
            }
        }

        // triggers when pick now qty is changes in sales order details grid
        private void SODetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                Validate();

                //Multiple pasting support
                if ((e.ColumnIndex == SOItemDetails.Columns["LOCATION_FAKE"].Index)
                    && SOItemDetails.SelectedRows.Count > 1)
                {
                    var wait = showWaitForm();

                    foreach (DataGridViewRow row in SOItemDetails.SelectedRows)
                    {
                        if (row.Index != e.RowIndex)
                        {
                            row.Cells[e.ColumnIndex].Value = SOItemDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        }
                    }

                    wait.Close();
                }

                try
                {
                    SOItemDetailsAdapter.Update(SOItemDetailsDataSet);
                }
                catch (DBConcurrencyException)
                {
                    MessageBox.Show("Concurrency violation! Please, refresh the app.");
                }

                // run stored procedure and update real database tables
                (new OdbcCommand("eoa_so_item_details_update_" + SOItemDetails.Columns[e.ColumnIndex].Name + ' '
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

                //Multiple pasting support
                if (e.ColumnIndex == dgv.Columns["DIFOT_FAKE"].Index && dgv.SelectedRows.Count > 1)
                {
                    var wait = showWaitForm();

                    foreach (DataGridViewRow row in dgv.SelectedRows)
                    {
                        if (row.Index != e.RowIndex)
                        {
                            row.Cells[e.ColumnIndex].Value = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        }
                    }

                    wait.Close();
                }

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

                //Multiple pasting support
                if ((e.ColumnIndex == dgv.Columns["METHOD_FAKE"].Index
                    || e.ColumnIndex == dgv.Columns["STATUS_FAKE"].Index
                    || e.ColumnIndex == dgv.Columns["DUEDATE_FAKE"].Index)
                    && dgv.SelectedRows.Count > 1)
                {
                    var wait = showWaitForm();

                    foreach (DataGridViewRow row in dgv.SelectedRows)
                    {
                        if (row.Index != e.RowIndex)
                        {
                            row.Cells[e.ColumnIndex].Value = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        }
                    }

                    wait.Close();
                }

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
            var wait = showWaitForm();

            loadSalesOrdersMain();
            loadSalesOrdersSecondary();
            // loadDifotData();

            // re-select SO to refresh item details grid
            if (SOMain.Rows.Count > 0)
            {
                // SO_RowEnter(SOMain, new DataGridViewCellEventArgs(0, 0));
                SOMain.Focus();
            }
            else if (SOSecondary.Rows.Count > 0)
            {
                // SO_RowEnter(SOSecondary, new DataGridViewCellEventArgs(0, 0));
                SOSecondary.Focus();
            }

            wait.Close();
        }

        // returns # of the current/active SO
        private string getCurrentSO()
        {
            if (label6.Text != "")
            {
                return label6.Text;
            }

            return null;
        }

        // retruns DataGridViewRow object for current/active SO
        private DataGridViewRow getCurrentSORow()
        {
            if (activeGrid != null)
            {
                if (activeGrid == SOMain.Name)
                {
                    return SOMain.SelectedRows[0];
                }
                else
                {
                    return SOSecondary.SelectedRows[0];
                }
            }

            return null;
        }

        // Refresh F10 click
        private void refreshF10_Click(object sender, EventArgs e)
        {
            SOItemDetails.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            SOItemDetails.EndEdit();

            var wait = showWaitForm();

            // if any of the splits was selected - save current SO
            var order = getCurrentSO();

            // run the refresh
            loadSalesOrdersMain();
            loadSalesOrdersSecondary();

            if (order != null)
            {
                searchForRecordAndSelect(order);
            }

            SOItemDetails.EditMode = DataGridViewEditMode.EditOnEnter;

            wait.Close();
        }

        private void searchForRecordAndSelect(string id)
        {
            // try find selected record in split 1
            try
            {
                DataGridViewRow row = SOMain.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r => r.Cells["#"].Value.ToString().Equals(id))
                    .First();

                SOMain.ClearSelection();
                SOMain.Rows[row.Index].Selected = true;
                SO_RowEnter(SOMain, new DataGridViewCellEventArgs(0, row.Index));
            }
            catch (InvalidOperationException)
            {
                // try to find selected record in split 2
                try
                {
                    DataGridViewRow row = SOSecondary.Rows
                        .Cast<DataGridViewRow>()
                        .Where(r => r.Cells["#"].Value.ToString().Equals(id))
                        .First();

                    SOSecondary.ClearSelection();
                    SOSecondary.Rows[row.Index].Selected = true;
                    SO_RowEnter(SOSecondary, new DataGridViewCellEventArgs(0, row.Index));
                }
                catch (InvalidOperationException) { }
            }
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            loadSalesOrdersSecondary();
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

        // Hot keys initialization here
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F5))
            {
                if (tabControl1.SelectedTab.Name == "ScheduleTab")
                {
                    SOItemDetails.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                    SOItemDetails.EndEdit();

                    button1_Click(refreshF5, new EventArgs());

                    SOItemDetails.EditMode = DataGridViewEditMode.EditOnEnter;
                }
                else if (tabControl1.SelectedTab.Name == "DifotTab")
                {
                    SODifot.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                    SODifot.EndEdit();

                    refreshDifot_Click(refreshDifot, new EventArgs());

                    SODifot.EditMode = DataGridViewEditMode.EditOnEnter;
                }
                
            }
            else if (keyData == (Keys.Control | Keys.F5))
            {
                refreshF10_Click(refreshF10, new EventArgs());
            }
            else if (keyData == (Keys.Alt | Keys.P))
            {
                processPickBtn_Click(processPickBtn, new EventArgs());
            }
            else if (keyData == (Keys.Control | Keys.P))
            {
                printPickingBtn_Click(printPickingBtn, new EventArgs());
            }
            else if (keyData == (Keys.Alt | Keys.A))
            {
                pickAllBtn_Click(pickAllBtn, new EventArgs());
            }
            else if (keyData == (Keys.F3))
            {
                selectNextNot("P", () => printPickingBtn_Click(printPickingBtn, new EventArgs()));
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void settings_Save_Click(object sender, EventArgs e)
        {
            OdbcCommand command = new OdbcCommand("update EOA_SETTINGS set PRINTER_NAME = '" + settings_printerName.Text
                + "', " + "LABEL_PRINTER = '" + settings_labelPrinter.Text + "'", connection);
            command.ExecuteNonQuery();
        }

        private void processPick()
        {
            var order = getCurrentSO();

            var wait = showWaitForm();

            StringBuilder sb = new StringBuilder();

            (new OdbcCommand("exec eoa_process_pick " + order, connection)).ExecuteNonQuery();

            //Starting Information for process like its path, use system shell i.e. control process by system etc.
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe");
            // its states that system shell will not be used to control the process instead program will handle the process
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;
            // Do not show command prompt window separately
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            //redirect all standard inout to program
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            //create the process with above infor and start it
            Process plinkProcess = new Process();
            plinkProcess.StartInfo = psi;
            plinkProcess.Start();
            //link the streams to standard inout of process
            StreamWriter inputWriter = plinkProcess.StandardInput;
            StreamReader outputReader = plinkProcess.StandardOutput;
            StreamReader errorReader = plinkProcess.StandardError;
            //send command to cmd prompt and wait for command to execute with thread sleep
            inputWriter.WriteLine(@"START exo://saleorder(" + order + ")");

            Thread.Sleep(500);
            wait.Close();
        }

        private void processPickBtn_Click(object sender, EventArgs e)
        {
            var row = getCurrentSORow();

            if (row != null)
            {
                var wait = showWaitForm();

                var preview = new ProcessPickDialog(getCurrentSORow(), settings_labelPrinter.Text, process);
                preview.Show();

                wait.Close();
            }
        }

        private void pickAllBtn_Click(object sender, EventArgs e)
        {
            var order = getCurrentSO();
            var orderRow = getCurrentSORow();

            if (order != null)
            {
                var wait = showWaitForm();

                (new OdbcCommand("exec eoa_pick_all " + order, connection)).ExecuteNonQuery();
                loadSalesOrderItemDetails(order);

                orderRow.DataGridView.Focus();

                wait.Close();
            }
        }

        // SO # label click
        private void label6_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            //Starting Information for process like its path, use system shell i.e. control process by system etc.
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe");
            // its states that system shell will not be used to control the process instead program will handle the process
            psi.UseShellExecute = false;
            psi.ErrorDialog = false;
            // Do not show command prompt window separately
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            //redirect all standard inout to program
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            //create the process with above infor and start it
            Process plinkProcess = new Process();
            plinkProcess.StartInfo = psi;
            plinkProcess.Start();
            //link the streams to standard inout of process
            StreamWriter inputWriter = plinkProcess.StandardInput;
            StreamReader outputReader = plinkProcess.StandardOutput;
            StreamReader errorReader = plinkProcess.StandardError;
            //send command to cmd prompt and wait for command to execute with thread sleep
            var line = @"START exo://saleorder(" + label6.Text + ")";
            inputWriter.WriteLine(line);
        }

        private void listenToAppProtocolHandler()
        {
            var self = this;

            //Paralel thread that continiously reads information from named pipe called PipesOfPiece - receives information from AppProtocolhandler
            Task.Factory.StartNew(() =>
            {
                PipeSecurity ps = new PipeSecurity();
                ps.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().Name, PipeAccessRights.FullControl, AccessControlType.Allow));
                ps.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null),   //"Authenticated Users"
                    PipeAccessRights.ReadWrite, AccessControlType.Allow));
                ps.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),   //"Administrators"
                    PipeAccessRights.FullControl, AccessControlType.Allow));
                ps.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.TerminalServerSid, null),   //"Terminal Server Accounts"
                    PipeAccessRights.FullControl, AccessControlType.Allow));
                ps.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),   //"Everyone"
                    PipeAccessRights.FullControl, AccessControlType.Allow));

                using (NamedPipeServerStream server = new NamedPipeServerStream("PipesOfPiece" + "_" + WindowsIdentity.GetCurrent().Name, PipeDirection.InOut, 10,
                                    PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1024, 1024, ps))
                {
                    server.WaitForConnection();
                    StreamReader reader = new StreamReader(server);
                    Boolean connectedOrWaiting = true;
                    while (true)
                    {
                        //establish connection if not established
                        if (!connectedOrWaiting)
                        {
                            server.BeginWaitForConnection((a) => { server.EndWaitForConnection(a); }, null);
                            connectedOrWaiting = true;
                        }

                        //if there is a client connected to our named pipe, begin
                        if (server.IsConnected)
                        {

                            var line = reader.ReadLine();
                            if (line != null)
                            {
                                //read the SeqNo, enter the main thread, update the db (in case there are new orders in exo), open tab 1 and bring window to front
                                if (InvokeRequired)
                                {
                                    Invoke((MethodInvoker)delegate
                                    {
                                        if (WindowState == FormWindowState.Minimized)
                                        {
                                            WindowState = FormWindowState.Normal;
                                        }

                                        Activate();
                                        button1_Click(refreshF5, new EventArgs());
                                        searchForRecordAndSelect(line);
                                    });
                                }
                                else
                                {
                                    if (WindowState == FormWindowState.Minimized)
                                    {
                                        WindowState = FormWindowState.Normal;
                                    }

                                    Activate();
                                    button1_Click(refreshF5, new EventArgs());
                                    searchForRecordAndSelect(line);
                                }
                            }
                            server.Disconnect();
                            connectedOrWaiting = false;
                        }
                    }
                }
            });
        }

        private void narrativeTextBox_TextChanged(object sender, EventArgs e)
        {
            (new OdbcCommand("exec eoa_update_narrative '" + narrativeTextBox.Text + "', " + sessionId.ToString(), connection)).ExecuteNonQueryAsync();
        }

        public void updateCarrier(string carrier)
        {
            // run stored procedure and update real database tables
            (new OdbcCommand("exec so_update_CARRIER "
               + getCurrentSORow().Cells["#"].Value.ToString() + ", '"
               + carrier + "'", connection)).ExecuteNonQueryAsync();
        }

        private void transferBtn_Click(object sender, EventArgs e)
        {
            var wait = showWaitForm();

            SOItemDetails.Focus();

            var reference = referenceTextBox.Text;
            var toLocation = locationComboBox.SelectedValue.ToString();

            bool insertIntoHdr = true;

            foreach (DataGridViewRow itemRow in SOItemDetails.Rows)
            {
                if (itemRow.Cells["X_ACTION"].Value.ToString() != "")
                {
                    string stockCode = itemRow.Cells["STOCKCODE"].Value.ToString();
                    string quantity = itemRow.Cells["X_ACTION"].Value.ToString();
                    string location = itemRow.Cells["LOCATION"].Value.ToString();

                    itemRow.Cells["X_ACTION"].Value = DBNull.Value;

                    (new OdbcCommand("eoa_transfer '" + stockCode + "', '" + reference + "', " + quantity + ", '" + location + "', '" + toLocation + "', " + (insertIntoHdr ? "1" : "0"),
                        connection)).ExecuteNonQuery();

                    insertIntoHdr = false;
                }
            }

            referenceTextBox.Text = "";
            wait.Close();

            refreshF10_Click(refreshF10, new EventArgs());
        }

        private void duplicateBtn_Click(object sender, EventArgs e)
        {
            var wait = showWaitForm();

            SOItemDetails.Focus();

            var location = locationComboBox.SelectedValue.ToString();
            var reference = referenceTextBox.Text;

            bool addReference = true;

            foreach (DataGridViewRow itemRow in SOItemDetails.Rows)
            {
                if (itemRow.Cells["X_ACTION"].Value.ToString() != "")
                {
                    string seqno = itemRow.Cells["LINES_ID"].Value.ToString();
                    string quantity = itemRow.Cells["X_ACTION"].Value.ToString();

                    itemRow.Cells["X_ACTION"].Value = DBNull.Value;

                    (new OdbcCommand("eoa_duplicate " + seqno + ", " + quantity + ", '" + location + "', '" + reference + "', " + (addReference ? "1" : "0"), connection)).ExecuteNonQuery();

                    addReference = false;
                }
            }

            wait.Close();

            refreshF10_Click(refreshF10, new EventArgs());
        }

        private void tdBtn_Click(object sender, EventArgs e)
        {
            var wait = showWaitForm();

            SOItemDetails.Focus();

            var reference = referenceTextBox.Text;
            var toLocation = locationComboBox.SelectedValue.ToString();

            bool insertIntoHdr = true;

            foreach (DataGridViewRow itemRow in SOItemDetails.Rows)
            {
                if (itemRow.Cells["X_ACTION"].Value.ToString() != "")
                {
                    string seqno = itemRow.Cells["LINES_ID"].Value.ToString();
                    string stockCode = itemRow.Cells["STOCKCODE"].Value.ToString();
                    string quantity = itemRow.Cells["X_ACTION"].Value.ToString();
                    string location = itemRow.Cells["LOCATION"].Value.ToString();

                    itemRow.Cells["X_ACTION"].Value = DBNull.Value;

                    (new OdbcCommand("eoa_transfer '" + stockCode + "', '" + reference + "', " + quantity + ", '" + location + "', '" + toLocation + "', " + (insertIntoHdr ? "1" : "0"),
                        connection)).ExecuteNonQuery();

                    (new OdbcCommand("eoa_duplicate " + seqno + ", " + quantity + ", '" + toLocation + "', '" + reference + "', " + (insertIntoHdr ? "1" : "0"), connection)).ExecuteNonQuery();

                    insertIntoHdr = false;
                }
            }

            referenceTextBox.Text = "";
            wait.Close();

            refreshF10_Click(refreshF10, new EventArgs());
        }

        private void clearAllBtn_Click(object sender, EventArgs e)
        {
            var wait = showWaitForm();

            SOItemDetails.Focus();

            foreach (DataGridViewRow itemRow in SOItemDetails.Rows)
            {
                if (Int32.Parse(itemRow.Cells["PICK_NOW"].Value.ToString()) != 0)
                {
                    itemRow.Cells["PICK_NOW"].Value = 0;
                }
            }

            wait.Close();
        }

        private PleaseWaitForm showWaitForm()
        {
            PleaseWaitForm wait = new PleaseWaitForm();
            wait.Location = new Point(Location.X + (Width - wait.Width) / 2, Location.Y + (Height - wait.Height) / 2);
            wait.Show();

            System.Windows.Forms.Application.DoEvents();

            return wait;
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            exportReport(shelfReport, 1.97, 0.99);
            prepareDocAndPrint(new PaperSize("Stock Label", 197, 99), settings_labelPrinter.Text, 1);
        }

        private void prepareDocAndPrint(PaperSize paperSize, string printerName, short copies)
        {
            PrintDocument printDoc = new PrintDocument();

            printDoc.PrinterSettings.PrinterName = printerName;
            printDoc.PrinterSettings.Copies = copies;

            printDoc.DefaultPageSettings.PaperSize = paperSize;
            printDoc.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;

            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            m_currentPageIndex = 0;
            printDoc.Print();
        }

        // Export the given report as an EMF (Enhanced Metafile) file.
        private void exportReport(LocalReport report, double pageWidth, double pageHeight, double marginTop = 0, double marginBottom = 0)
        {
            string deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>" + pageWidth.ToString() + @"in</PageWidth>
                <PageHeight>" + pageHeight.ToString() + @"in</PageHeight>
                <MarginTop>" + marginTop.ToString() + @"in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>" + marginBottom.ToString() + @"in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }

        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left,
                ev.PageBounds.Top,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        // Routine to provide to the report renderer, in order to
        // save an image for each page of the report.
        private Stream CreateStream(string name,
          string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        private void selectNextNot(string status, Action callback = null)
        {
            var so = SOMain.SelectedRows;

            if (so.Count > 0)
            {
                for (int i = so[0].Index + 1; i < SOMain.Rows.Count; i++)
                {
                    if (SOMain.Rows[i].Cells["STATUS"].Value.ToString() != status)
                    {
                        SOMain.Focus();
                        SOMain.CurrentCell = SOMain.Rows[i].Cells["#"];
                        callback?.Invoke();
                        return;
                    }
                }
            }
        }

        private void addStockBtn_Click(object sender, EventArgs e)
        {
            var selectedValue = stockCombobox.SelectedValue as DataRowView;

            stockLblDataGridView.Rows.Add(new string[] { selectedValue.Row[0].ToString(),
                selectedValue.Row[1].ToString(), "0", "1" });
        }

        private void previewStockLabel_Click(object sender, EventArgs e)
        {
            var row = stockLblDataGridView.CurrentRow;

            if (row != null)
            {
                if (stockReport != null)
                {
                    stockReport.DataSources.RemoveAt(0);
                    stockReportViewer.LocalReport.DataSources.RemoveAt(0);
                }
                
                initStockReportViewer(row.Index, true);
            }
            else
            {
                MessageBox.Show("       No data selected       ");
            }
        }

        private void printAllBtn_Click(object sender, EventArgs e)
        {
            var wait = showWaitForm();
            addStockBtn.Enabled = false;
            previewStockLabel.Enabled = false;
            printAllBtn.Enabled = false;

            foreach (DataGridViewRow row in stockLblDataGridView.Rows)
            {
                initStockReportViewer(row.Index, false);
                exportReport(stockReport, 1.97, 0.99);
                prepareDocAndPrint(new PaperSize("Stock Label", 197, 99), settings_labelPrinter.Text, short.Parse(row.Cells[3].Value.ToString()));
            }

            addStockBtn.Enabled = true;
            previewStockLabel.Enabled = true;
            printAllBtn.Enabled = true;
            wait.Close();
        }

        private void clearAllStockRowsBtn_Click(object sender, EventArgs e)
        {
            stockLblDataGridView.Rows.Clear();
            stockLblDataGridView.Refresh();
        }
    }
}
