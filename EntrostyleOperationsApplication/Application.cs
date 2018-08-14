using System;
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

        private LocalReport shelfReport = new LocalReport();
        private LocalReport stockReport = new LocalReport();
        private LocalReport supplierReport = new LocalReport();
        private LocalReport layout30SHELFReport = new LocalReport();
        private LocalReport layout30STOCKReport = new LocalReport();
        private LocalReport layout30SUPPLIERReport = new LocalReport();

        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        public Application()
        {
            InitializeComponent();

            // read connection string and open the connection
            connection.ConnectionString = ReadConfig();
            connection.Open();

            // generate temporary session id and make sure it is unique
            sessionId = GenerateSessionId();
            VerifyUserIsUnique();

            // load main and secondary sales orders grids
            LoadSalesOrdersMain();
            LoadSalesOrdersSecondary();

            // load DIFOT data
            LoadDifotData();

            // load LABEL data
            PopulateLABELData();
            SetDataGridViewStyleProps(stockLblDataGridView);

            // load SETTINGS
            LoadSettings();

            // add fake dropdown columns for dispatch status and method + time columns + calendar columns
            AddStatusAndMethodDropdowns(SOMain);
            AddStatusAndMethodDropdowns(SOSecondary);
            AddDifotDropDown(SODifot);

            // set basic grid styling
            SetDataGridViewStyleProps(SOMain);
            SetDataGridViewStyleProps(SOSecondary);
            SetDataGridViewStyleProps(SODifot);

            // customize columns for split 1 and 2
            StyleMainDataGridViewColumns(SOMain);
            StyleMainDataGridViewColumns(SOSecondary);

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
            stockLblDataGridView.CellMouseClick += SOMain_CellMouseClick;

            // so item details conditional styling
            SOItemDetails.CellFormatting += SOItemDetails_CellFormatting;
            SOItemDetails.Leave += SOItemDetails_Leave;

            // customize columns for DIFOT
            StyleDifotColumns();

            // focus main grid or secondary if main has 0 rows
            FocusSO();

            // listen to app protocol handler
            ListenToAppProtocolHandler();
        }

        private void SOItemDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Prevents system error messages since cell level validation occurs in cell validating event
        }

        private void SOItemDetails_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == SOItemDetails.Columns["X_ACTION"].Index)
            {
                // if cell is not empty
                if (e.FormattedValue.ToString() != "")
                {
                    // if not numeric
                    if (!int.TryParse(Convert.ToString(e.FormattedValue), out int i))
                    {
                        e.Cancel = true;
                        MessageBox.Show("       Only numeric characters are accepted.       ");
                    }
                }
            }

            if (e.ColumnIndex == SOItemDetails.Columns["PICK_NOW"].Index)
            {
                // if cell is not empty
                if (e.FormattedValue.ToString() != "")
                {
                    // if not numeric
                    if (!int.TryParse(Convert.ToString(e.FormattedValue), out int i))
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

            if (columnName == "#" || columnName == "STOCKCODE" || columnName == "ItemCode")
            {
                var wait = ShowWaitForm();

                StringBuilder sb = new StringBuilder();
                //Starting Information for process like its path, use system shell i.e. control process by system etc.
                var psi = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe")
                {
                    // its states that system shell will not be used to control the process instead program will handle the process
                    UseShellExecute = false,
                    ErrorDialog = false,
                    // Do not show command prompt window separately
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    //redirect all standard inout to program
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                };
                //create the process with above infor and start it
                Process plinkProcess = new Process
                {
                    StartInfo = psi
                };
                plinkProcess.Start();
                //link the streams to standard inout of process
                StreamWriter inputWriter = plinkProcess.StandardInput;
                StreamReader outputReader = plinkProcess.StandardOutput;
                StreamReader errorReader = plinkProcess.StandardError;
                //send command to cmd prompt and wait for command to execute with thread sleep
                if (e.RowIndex != -1)
                {
                    var line = columnName == "#"
                        ? @"START exo://saleorder(" + dgv.Rows[e.RowIndex].Cells[columnName].Value.ToString() + ")"
                        : @"START exo://stockitem/?stockcode=" + dgv.Rows[e.RowIndex].Cells[columnName].Value.ToString();
                    inputWriter.WriteLine(line);
                }

                wait.Close();
            }
        }

        private void Process(string carrier)
        {
            UpdateCarrier(carrier);
            ProcessPick();
        }

        private void PrintPickingBtn_Click(object sender, EventArgs e)
        {
            var row = GetCurrentSORow();

            if (row != null)
            {
                var wait = ShowWaitForm();

                var preview = new PrintPickingDialog(row, settings_PickLabelPrinter.Text, InitSalesOrderReport); // new
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
        private string ReadConfig()
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
        private void FocusSO()
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
        private int GenerateSessionId()
        {
            return (new Random()).Next(0, 9999);
        }

        // loads data to main sales orders table
        private void LoadSalesOrdersMain()
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
        private void LoadSalesOrdersSecondary()
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
        private void LoadSalesOrderItemDetails(string seqno)
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
        private void LoadSettings()
        {
            SOSettingsAdapter = new OdbcDataAdapter("SELECT * FROM EOA_SETTINGS", connection);
            SOSettingsDataSet = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(SOSettingsAdapter);
            SOSettingsAdapter.Fill(SOSettingsDataSet);

            var settingsRow = SOSettingsDataSet.Tables[0].Rows[0];

            settings_printerName.Text = settingsRow["PRINTER_NAME"].ToString();
            settings_labelPrinter.Text = settingsRow["LABEL_PRINTER"].ToString();
            settings_30LabelPrinter.Text = settingsRow["30_LABEL_PRINTER"].ToString();
            settings_PickLabelPrinter.Text = settingsRow["PICK_LABEL_PRINTER"].ToString();
        }

        // load DIFOT data
        private void LoadDifotData()
        {
            var wait = ShowWaitForm();
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
        private void PopulateLABELData()
        {
            var wait = ShowWaitForm();

            var adapter = new OdbcDataAdapter("SELECT STOCKCODE, DESCRIPTION, BARCODE1 FROM STOCK_ITEMS order by STOCKCODE", connection);
            var ds = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(adapter);
            adapter.Fill(ds);

            // STOCK combobox
            stockCodeLABELCombobox.DataSource = ds.Tables[0];
            stockCodeLABELCombobox.DisplayMember = "STOCKCODE";

            stockLblDataGridView.ColumnCount = 5;

            // Item Code column
            stockLblDataGridView.Columns[0].Name = "ItemCode";
            stockLblDataGridView.Columns[0].HeaderText = "Item Code";
            stockLblDataGridView.Columns[0].ReadOnly = true;
            stockLblDataGridView.Columns[0].DefaultCellStyle.BackColor = Color.Silver;
            stockLblDataGridView.Columns[0].Width = 100;
            stockLblDataGridView.Columns[0].DefaultCellStyle.ForeColor = Color.Blue;
            stockLblDataGridView.Columns[0].DefaultCellStyle.SelectionForeColor = Color.Blue;
            stockLblDataGridView.Columns[0].DefaultCellStyle.Font = new Font("Arial", 11F, FontStyle.Underline, GraphicsUnit.Pixel);

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

            // Barcode1 column
            stockLblDataGridView.Columns[4].Name = "Barcode1";
            stockLblDataGridView.Columns[4].Visible = false;

            // report paths
            stockReport.ReportPath = @".\1_stock.rdlc";
            shelfReport.ReportPath = @".\1_shelf.rdlc";
            supplierReport.ReportPath = @".\1_supplier.rdlc";
            layout30SHELFReport.ReportPath = @".\30_shelf.rdlc";
            layout30STOCKReport.ReportPath = @".\30_stock.rdlc";
            layout30SUPPLIERReport.ReportPath = @".\30_supplier.rdlc";

            // event listeners
            stockLblDataGridView.CellValidating += StockLblDataGridView_CellValidating;
            itemQtyNumberInput.Enter += ItemQtyNumberInput_Enter;
            labelQtyNumberInput.Enter += ItemQtyNumberInput_Enter;
            labelQtyNumberInput.Leave += LabelQtyNumberInput_Leave;

            wait.Close();
        }

        private void LabelQtyNumberInput_Leave(object sender, EventArgs e)
        {
            AddStockBtn_Click(addStockBtn, new EventArgs());
            addStockBtn.Enabled = true;
        }

        private void ItemQtyNumberInput_Enter(object sender, EventArgs e)
        {
            var control = sender as NumericUpDown;
            control.Select(0, control.Text.Length);

            if (control.Name == labelQtyNumberInput.Name)
            {
                addStockBtn.Enabled = false;
            }
        }

        private void SetLabelDataRow(int rowIndex, DataTable dt)
        {
            var selectedValue = stockLblDataGridView.Rows[rowIndex];

            DataRow dr = dt.NewRow();

            dr["STOCKCODE"] = selectedValue.Cells[0].Value.ToString();
            dr["DESCRIPTION"] = selectedValue.Cells[1].Value.ToString();
            dr["ITEMQTY"] = int.Parse(selectedValue.Cells[2].Value.ToString());
            dr["BARCODE1"] = selectedValue.Cells[4].Value.ToString();

            Bitmap bitmap = GenerateBarcode(selectedValue.Cells[0].Value.ToString(), 100, 100, 0);
            dr["BARCODE"] = (byte[])(new ImageConverter().ConvertTo(bitmap, typeof(byte[])));

            dt.Rows.Add(dr);
        }

        private void InitLABELReport(LocalReport report, int? rowIndex = null)
        {
            var wait = ShowWaitForm();
            

            DataTable dt = new DataTable();

            dt.Columns.Add("STOCKCODE", typeof(String));
            dt.Columns.Add("DESCRIPTION", typeof(String));
            dt.Columns.Add("BARCODE", typeof(byte[]));
            dt.Columns.Add("ITEMQTY", typeof(int));
            dt.Columns.Add("BARCODE1", typeof(String));

            if (rowIndex == null)
            {
                for (int i = 0; i < stockLblDataGridView.Rows.Count; i ++)
                {
                    var labelQty = int.Parse(stockLblDataGridView.Rows[i].Cells[3].Value.ToString());

                    for (int j = 0; j < labelQty; j++)
                    {
                        SetLabelDataRow(i, dt);
                    }
                }
            }
            else
            {
                SetLabelDataRow((int)rowIndex, dt);
            }

            if (report.DataSources.Count > 0)
            {
                report.DataSources.RemoveAt(0);
            }
            
            report.DataSources.Add(new ReportDataSource("DataSet1", dt));
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

        private void InitSalesOrderReport()
        {
            var so = GetCurrentSO();

            if (so != null)
            {
                var wait = ShowWaitForm();
                var soRow = GetCurrentSORow();

                var salesOrderReport = new LocalReport
                {
                    ReportPath = @".\picksheet.rdlc"
                };

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
                var filteredSOItems = SOItemDetailsDataSet.Tables[0];

                var dataSource2 = new DataTable();
                dataSource2.Columns.Add("BARCODE", typeof(byte[]));

                var rows = filteredSOItems.AsEnumerable()
                    .Where(r => (float.Parse(r["PICK_NOW"].ToString()) != 0) || (r["STOCKCODE"].ToString()).Length == 0);

                filteredSOItems = rows.Any() ? rows.CopyToDataTable() : filteredSOItems.Clone();

                // remove all unnecessary columns from filteredSOItems
                for (int i = filteredSOItems.Columns.Count - 1; i >= 0; i--)
                {
                    string[] columnsToKeep = { "STOCKCODE", "DESCRIPTION", "PICK_NOW", "X_HEADING_LINE", "X_HIDEFROMPICK", "LINETYPE" };

                    if (!columnsToKeep.Contains<string>(filteredSOItems.Columns[i].ColumnName))
                    {
                        filteredSOItems.Columns.RemoveAt(i);
                    }
                }

                // add barcode column
                filteredSOItems.Columns.Add("BARCODE", typeof(byte[]));

                string tempBarCode = "";

                // set barcode for each row + generate QR per each 10 records
                for (int i = 0; i < filteredSOItems.Rows.Count; i++)
                {
                    var row = filteredSOItems.Rows[i];

                    tempBarCode += (row["STOCKCODE"].ToString().Length > 0 ? row["STOCKCODE"].ToString() : @"N/A") + "," +
                        (row["DESCRIPTION"].ToString().Length > 0 ? row["DESCRIPTION"].ToString() : @"N/A") + "," +
                        (row["PICK_NOW"].ToString().Length > 0 ? row["PICK_NOW"].ToString() : @"N/A") + ";";

                    if (row["STOCKCODE"].ToString().Length > 0)
                        row["BARCODE"] = (byte[])(new ImageConverter().ConvertTo(GenerateBarcode(row["STOCKCODE"].ToString(), 100, 100, 0), typeof(byte[])));

                    if ((i + 1) % 10 == 0 || i == (filteredSOItems.Rows.Count - 1))
                    {
                        DataRow newRow = dataSource2.NewRow();
                        newRow["BARCODE"] = (byte[])(new ImageConverter().ConvertTo(GenerateBarcode(tempBarCode, 800, 800, 0), typeof(byte[])));
                        dataSource2.Rows.Add(newRow);

                        tempBarCode = "";
                    }
                }

                salesOrderReport.DataSources.Add(new ReportDataSource("DataSet2", dataSource2));
                salesOrderReport.DataSources.Add(new ReportDataSource("DataSet3", filteredSOItems));
                // end of salesord_lines

                ExportReport(salesOrderReport, 8, 10.7, 0.59, 0.59);
                PrepareDocAndPrint(new PaperSize("Sales Order", 800, 1070), settings_printerName.Text, 1);

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
        private void VerifyUserIsUnique()
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
        private void AddDifotDropDown(DataGridView dgv)
        {
            var listSource = new string[] { "difot", "shipped late" };

            var difotColumn = new DataGridViewComboBoxColumn
            {
                HeaderText = "Difot",
                Name = "DIFOT_FAKE",
                DataSource = listSource,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };

            dgv.Columns.Add(difotColumn);
            dgv.Columns[difotColumn.Name].DataPropertyName = "X_DIFOT_STATUS";
            dgv.Columns[difotColumn.Name].DisplayIndex = 3;
        }

        // add dispatch status and method dropdowns
        private void AddStatusAndMethodDropdowns(DataGridView dgv)
        {
            var listSource = new string[] { "TP-PICK", "TP-POWDER", "TP-PROJECT", "TP-KEY", "TP-CUT", "TP-SHIP", "TP-BULK", "W", "P", "TA", "Sc" };
            var dispatchStatusColumn = new DataGridViewComboBoxColumn
            {
                HeaderText = "Status",
                Name = "STATUS_FAKE",
                DataSource = listSource,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };

            dgv.Columns.Add(dispatchStatusColumn);
            dgv.Columns[dispatchStatusColumn.Name].DataPropertyName = "STATUS";
            dgv.Columns[dispatchStatusColumn.Name].DisplayIndex = 3;

            //----------------------METHOD-----------------------

            var methodSource = new string[] { "E1", "E4", "P", "N", "(n)" };
            var dispatchMethodColumn = new DataGridViewComboBoxColumn
            {
                HeaderText = "Method",
                Name = "METHOD_FAKE",
                DataSource = methodSource,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };

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

            TimeColumn dueTimeColumn = new TimeColumn
            {
                HeaderText = "Time",
                Name = "DUETIME_FAKE"
            };

            dgv.Columns.Add(dueTimeColumn);
            dgv.Columns[dueTimeColumn.Name].DataPropertyName = "DUETIME";
            dgv.Columns[dueTimeColumn.Name].DisplayIndex = 8;
        }

        private void AddLocationDropdown()
        {
            var locationAdapter = new OdbcDataAdapter("SELECT CONCAT(LOCNO, ' ', LCODE) FROM STOCK_LOCATIONS", connection);
            var locationDS = new DataSet();
            OdbcCommandBuilder cmdbuilder = new OdbcCommandBuilder(locationAdapter);
            locationAdapter.Fill(locationDS);

            DataRow[] rows = locationDS.Tables[0].Select();
            string[] optionsArray = rows.Select(row => row[0].ToString()).ToArray();
            var locationSource = optionsArray;

            var locationColumn = new DataGridViewComboBoxColumn
            {
                HeaderText = "Location",
                Name = "LOCATION_FAKE",
                DisplayIndex = 9,
                DataSource = locationSource,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };

            SOItemDetails.Columns.Add(locationColumn);
            SOItemDetails.Columns[locationColumn.Name].DataPropertyName = "LOCATION";

            // location dropdown on dashboard
            locationComboBox.DataSource = optionsArray.Clone();
        }

        // style Data Grid View columns for SO Details grid
        private void StyleSODetailsColumns()
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
            columns["x_heading_line"].Visible = false;
            columns["x_hidefrompick"].Visible = false;
            columns["LINETYPE"].Visible = false;

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
        private void StyleMainDataGridViewColumns(DataGridView dgv)
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
        private void StyleDifotColumns()
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
        private void SetDataGridViewStyleProps(DataGridView dgv)
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
                LoadSalesOrderItemDetails(cells["#"].Value.ToString());

                // customize columns for SO details grid
                if (!isSODetailsGridStyled)
                {
                    AddLocationDropdown();
                    SetDataGridViewStyleProps(SOItemDetails);
                    StyleSODetailsColumns();
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
                    var wait = ShowWaitForm();

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
                    var wait = ShowWaitForm();

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
                    var wait = ShowWaitForm();

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
        private void Button1_Click(object sender, EventArgs e)
        {
            var wait = ShowWaitForm();

            LoadSalesOrdersMain();
            LoadSalesOrdersSecondary();
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
        private string GetCurrentSO()
        {
            if (label6.Text != "")
            {
                return label6.Text;
            }

            return null;
        }

        // retruns DataGridViewRow object for current/active SO
        private DataGridViewRow GetCurrentSORow()
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

        private void RefreshF10()
        {
            SOItemDetails.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            SOItemDetails.EndEdit();

            var wait = ShowWaitForm();

            // if any of the splits was selected - save current SO
            var order = GetCurrentSO();

            // run the refresh
            LoadSalesOrdersMain();
            LoadSalesOrdersSecondary();

            if (order != null)
            {
                SearchForRecordAndSelect(order);
            }

            SOItemDetails.EditMode = DataGridViewEditMode.EditOnEnter;

            wait.Close();
        }

        private void SearchForRecordAndSelect(string id)
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

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            LoadSalesOrdersSecondary();
        }

        private void ClearSearchBtn_Click(object sender, EventArgs e)
        {
            searchBox.TextChanged -= SearchBox_TextChanged;
            searchBox.Text = "";
            searchBox.TextChanged += SearchBox_TextChanged;

            LoadSalesOrdersSecondary();
        }

        private void RefreshDifot_Click(object sender, EventArgs e)
        {
            LoadDifotData();
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

                    Button1_Click(refreshF5, new EventArgs());

                    SOItemDetails.EditMode = DataGridViewEditMode.EditOnEnter;
                }
                else if (tabControl1.SelectedTab.Name == "DifotTab")
                {
                    SODifot.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                    SODifot.EndEdit();

                    RefreshDifot_Click(refreshDifot, new EventArgs());

                    SODifot.EditMode = DataGridViewEditMode.EditOnEnter;
                }
                
            }
            else if (keyData == (Keys.Control | Keys.F5))
            {
                RefreshF10();
            }
            else if (keyData == (Keys.Alt | Keys.P))
            {
                ProcessPickBtn_Click(processPickBtn, new EventArgs());
            }
            else if (keyData == (Keys.Control | Keys.P))
            {
                PrintPickingBtn_Click(printPickingBtn, new EventArgs());
            }
            else if (keyData == (Keys.F3))
            {
                SelectNextNot("P", () => PrintPickingBtn_Click(printPickingBtn, new EventArgs()));
            }
            else if (keyData == (Keys.Alt | Keys.Delete))
            {
                if (stockLblDataGridView.ContainsFocus)
                {
                    DeleteCurrentStockLblRow();
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Settings_Save_Click(object sender, EventArgs e)
        {
            OdbcCommand command = new OdbcCommand("update EOA_SETTINGS set PRINTER_NAME = '" + settings_printerName.Text
                + "', " + "LABEL_PRINTER = '" + settings_labelPrinter.Text + "', "
                + "PICK_LABEL_PRINTER = '" + settings_PickLabelPrinter.Text + "', "
                + "[30_LABEL_PRINTER] = '" + settings_30LabelPrinter.Text + "'", connection);
            command.ExecuteNonQuery();
        }

        private void ProcessPick()
        {
            var order = GetCurrentSO();

            var wait = ShowWaitForm();

            StringBuilder sb = new StringBuilder();

            (new OdbcCommand("exec eoa_process_pick " + order, connection)).ExecuteNonQuery();

            //Starting Information for process like its path, use system shell i.e. control process by system etc.
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe")
            {
                // its states that system shell will not be used to control the process instead program will handle the process
                UseShellExecute = false,
                ErrorDialog = false,
                // Do not show command prompt window separately
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                //redirect all standard inout to program
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            //create the process with above infor and start it
            Process plinkProcess = new Process
            {
                StartInfo = psi
            };
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

        private void ProcessPickBtn_Click(object sender, EventArgs e)
        {
            var row = GetCurrentSORow();

            if (row != null)
            {
                var wait = ShowWaitForm();

                var preview = new ProcessPickDialog(GetCurrentSORow(), settings_PickLabelPrinter.Text, Process);
                preview.Show();

                wait.Close();
            }
        }

        private void PickAllBtn_Click(object sender, EventArgs e)
        {
            var order = GetCurrentSO();
            var orderRow = GetCurrentSORow();

            if (order != null)
            {
                var wait = ShowWaitForm();

                (new OdbcCommand("exec eoa_pick_all " + order, connection)).ExecuteNonQuery();
                LoadSalesOrderItemDetails(order);

                orderRow.DataGridView.Focus();

                wait.Close();
            }
        }

        // SO # label click
        private void Label6_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            //Starting Information for process like its path, use system shell i.e. control process by system etc.
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\WINDOWS\system32\cmd.exe")
            {
                // its states that system shell will not be used to control the process instead program will handle the process
                UseShellExecute = false,
                ErrorDialog = false,
                // Do not show command prompt window separately
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                //redirect all standard inout to program
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            //create the process with above infor and start it
            Process plinkProcess = new Process
            {
                StartInfo = psi
            };
            plinkProcess.Start();
            //link the streams to standard inout of process
            StreamWriter inputWriter = plinkProcess.StandardInput;
            StreamReader outputReader = plinkProcess.StandardOutput;
            StreamReader errorReader = plinkProcess.StandardError;
            //send command to cmd prompt and wait for command to execute with thread sleep
            var line = @"START exo://saleorder(" + label6.Text + ")";
            inputWriter.WriteLine(line);
        }

        private void ListenToAppProtocolHandler()
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
                                        Button1_Click(refreshF5, new EventArgs());
                                        SearchForRecordAndSelect(line);
                                    });
                                }
                                else
                                {
                                    if (WindowState == FormWindowState.Minimized)
                                    {
                                        WindowState = FormWindowState.Normal;
                                    }

                                    Activate();
                                    Button1_Click(refreshF5, new EventArgs());
                                    SearchForRecordAndSelect(line);
                                }
                            }
                            server.Disconnect();
                            connectedOrWaiting = false;
                        }
                    }
                }
            });
        }

        private void NarrativeTextBox_TextChanged(object sender, EventArgs e)
        {
            (new OdbcCommand("exec eoa_update_narrative '" + narrativeTextBox.Text + "', " + sessionId.ToString(), connection)).ExecuteNonQueryAsync();
        }

        public void UpdateCarrier(string carrier)
        {
            // run stored procedure and update real database tables
            (new OdbcCommand("exec so_update_CARRIER "
               + GetCurrentSORow().Cells["#"].Value.ToString() + ", '"
               + carrier + "'", connection)).ExecuteNonQueryAsync();
        }

        private void TransferBtn_Click(object sender, EventArgs e)
        {
            PerformStockLogic(locationComboBox.SelectedValue.ToString(), true, 2, "EOA TRANSFER");
        }

        private void DuplicateBtn_Click(object sender, EventArgs e)
        {
            var wait = ShowWaitForm();

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

            RefreshF10();
        }

        private void TdBtn_Click(object sender, EventArgs e)
        {
            var wait = ShowWaitForm();

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

            RefreshF10();
        }

        private void ClearAllBtn_Click(object sender, EventArgs e)
        {
            var wait = ShowWaitForm();

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

        private PleaseWaitForm ShowWaitForm()
        {
            PleaseWaitForm wait = new PleaseWaitForm();
            wait.Location = new Point(Location.X + (Width - wait.Width) / 2, Location.Y + (Height - wait.Height) / 2);
            wait.Show();

            System.Windows.Forms.Application.DoEvents();

            return wait;
        }

        private void PrepareDocAndPrint(PaperSize paperSize, string printerName, short copies)
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
        private void ExportReport(LocalReport report, double pageWidth, double pageHeight, double marginTop = 0, double marginBottom = 0)
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
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out Warning[] warnings);
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

        private void SelectNextNot(string status, Action callback = null)
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

        private void AddStockBtn_Click(object sender, EventArgs e)
        {
            if (stockCodeLABELCombobox.SelectedValue is DataRowView selectedValue)
            {
                stockLblDataGridView.Rows.Add(
                    new string[] { selectedValue.Row[0].ToString(),
                    selectedValue.Row[1].ToString(), itemQtyNumberInput.Value.ToString(),
                    labelQtyNumberInput.Value.ToString(),
                    selectedValue.Row[2].ToString()
                });
            }
            else
            {
                MessageBox.Show("       Please select a valid Stock Code       ");
                stockCodeLABELCombobox.Focus();
            }
        }

        private void PreviewStockLabel_Click(object sender, EventArgs e)
        {
            var row = stockLblDataGridView.CurrentRow;

            if (row != null && stockReport != null)
            {
                stockReport.DataSources.RemoveAt(0);
            }
            else
            {
                MessageBox.Show("       No data selected       ");
            }
        }

        private void ClearAllStockRowsBtn_Click(object sender, EventArgs e)
        {
            stockLblDataGridView.Rows.Clear();
            stockLblDataGridView.Refresh();
        }

        private void PrintSingleLABELLayout(LocalReport report)
        {
            var wait = ShowWaitForm();
            LabelPrintButtonsEnabled(false);

            foreach (DataGridViewRow row in stockLblDataGridView.Rows)
            {
                InitLABELReport(report, row.Index);
                ExportReport(report, 1.97, 0.99);
                PrepareDocAndPrint(new PaperSize("Stock Label", 197, 99),
                    customLabelPrinterCheckbox.Checked ? customLabelPrinterTextBox.Text : settings_labelPrinter.Text,
                    short.Parse(row.Cells[3].Value.ToString()));
            }

            LabelPrintButtonsEnabled(true);
            wait.Close();
        }

        private void Print30LABELLayout(LocalReport report)
        {
            var wait = ShowWaitForm();
            LabelPrintButtonsEnabled(false);

            InitLABELReport(report);
            ExportReport(report, 8.27, 11.69); // 0.59, 0.59
            PrepareDocAndPrint(new PaperSize("Layout 30", 827, 1169),
                customLabelPrinterCheckbox.Checked ? customLabelPrinterTextBox.Text : settings_30LabelPrinter.Text, 1);

            LabelPrintButtonsEnabled(true);
            wait.Close();
        }

        private void LabelPrintButtonsEnabled(bool enabled)
        {
            printStockButton.Enabled = enabled;
            print30StockButton.Enabled = enabled;
            printShelfButton.Enabled = enabled;
            print30ShelfButton.Enabled = enabled;
            printSupplierBtn.Enabled = enabled;
            print30SupplierBtn.Enabled = enabled;
        }

        private void PrintStockButton_Click(object sender, EventArgs e)
        {
            PrintSingleLABELLayout(stockReport);
        }

        private void Print30StockButton_Click(object sender, EventArgs e)
        {
            Print30LABELLayout(layout30STOCKReport);
        }

        private void PrintShelfButton_Click(object sender, EventArgs e)
        {
            PrintSingleLABELLayout(shelfReport);
        }

        private void Print30ShelfButton_Click(object sender, EventArgs e)
        {
            Print30LABELLayout(layout30SHELFReport);
        }

        private void CustomLabelPrinterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var control = sender as CheckBox;

            if (control.Checked)
                customLabelPrinterTextBox.Enabled = true;
            else
                customLabelPrinterTextBox.Enabled = false;
            
        }

        private void PrintSupplierBtn_Click(object sender, EventArgs e)
        {
            PrintSingleLABELLayout(supplierReport);
        }

        private void Print30SupplierBtn_Click(object sender, EventArgs e)
        {
            Print30LABELLayout(layout30SUPPLIERReport);
        }

        private void DeleteCurrentStockLblRow()
        {
            if (stockLblDataGridView.CurrentCell != null)
            {
                stockLblDataGridView.Rows.RemoveAt(stockLblDataGridView.CurrentCell.RowIndex);
            }
        }

        private void DeleteSelectedBtn_Click(object sender, EventArgs e)
        {
            DeleteCurrentStockLblRow();
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            RefreshF10();
        }

        private void ForceFullyProcessedBtn_Click(object sender, EventArgs e)
        {
            var order = GetCurrentSO();
            var orderRow = GetCurrentSORow();

            if (order != null)
            {
                var wait = ShowWaitForm();

                (new OdbcCommand("exec eoa_force_fully_processed " + order, connection)).ExecuteNonQuery();

                orderRow.DataGridView.Focus();

                wait.Close();
            }
        }

        private void PerformStockLogic(string toLocation, bool isQuantityNegative, int transtype, string ref1)
        {
            var wait = ShowWaitForm();

            SOItemDetails.Focus();

            var reference = referenceTextBox.Text;
            bool insertIntoHdr = true;
            bool wasError = false;

            foreach (DataGridViewRow itemRow in SOItemDetails.Rows)
            {
                int min = Math.Min( 
                    Int32.Parse((itemRow.Cells["UNSUP_QUANT"].Value != DBNull.Value ? itemRow.Cells["UNSUP_QUANT"].Value : -9999999).ToString()),
                    Int32.Parse((itemRow.Cells["TOTALSTOCK"].Value != DBNull.Value ? itemRow.Cells["TOTALSTOCK"].Value : -9999999).ToString())
                );
                var xAction = itemRow.Cells["X_ACTION"].Value;

                if (xAction.ToString() != "")
                {
                    if (Int32.Parse(xAction.ToString()) > min)
                    {
                        itemRow.Cells["X_ACTION"].Value = DBNull.Value;
                        wasError = true;
                    }
                    else
                    {
                        string stockCode = itemRow.Cells["STOCKCODE"].Value.ToString();
                        string quantity = (isQuantityNegative ? "-" : "") + itemRow.Cells["X_ACTION"].Value.ToString();
                        string location = itemRow.Cells["LOCATION"].Value.ToString();

                        itemRow.Cells["X_ACTION"].Value = DBNull.Value;

                        (new OdbcCommand("eoa_transfer '" +
                            stockCode + "', '" +
                            reference + "', '" +
                            ref1 + "', " +
                            quantity + ", '" +
                            location + "', '" +
                            toLocation + "', "
                            + (insertIntoHdr ? "1" : "0") + ", " +
                            transtype.ToString(),
                            connection)).ExecuteNonQuery();

                        insertIntoHdr = false;
                    }
                }
            }

            referenceTextBox.Text = "";
            wait.Close();

            RefreshF10();

            if (wasError)
            {
                MessageBox.Show("One or more Action values exceeded Outstanding and/or Location Qty and have been emptied.");
            }
        }

        private void AdjustInBtn_Click(object sender, EventArgs e)
        {
            PerformStockLogic("0", false, 4, "EOA ADJUST IN");
        }

        private void AdjustOutBtn_Click(object sender, EventArgs e)
        {
            PerformStockLogic("0", true, 3, "EOA ADJUST OUT");
        }
    }
}
