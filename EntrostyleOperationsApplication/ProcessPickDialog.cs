using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Drawing;
using ZXing;
using ZXing.Common;
using System.Text.RegularExpressions;

namespace EntrostyleOperationsApplication
{
    public partial class ProcessPickDialog : Form
    {
        DataGridViewRow row;
        string printerName;
        Action<string> callback;

        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        LocalReport report;

        public ProcessPickDialog(DataGridViewRow row, string printerName, Action<string> callback)
        {
            this.row = row;
            this.printerName = printerName;
            this.callback = callback;
            InitializeComponent();
        }

        private void PrintDialog_Load(object sender, EventArgs e)
        {
            string carrier = row.Cells["X_CARRIER"].Value.ToString();
            var regex = new Regex(@"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$");

            // do not pre-populate if carrier is a URL
            if (!regex.IsMatch(carrier))
            {
                carrierTextBox.Text = carrier;
            }
            carrierTextBox.TextChanged += carrierTextBox_TextChanged;

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            reportViewer1.LocalReport.ReportPath = @".\carrier_label.rdlc";
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Carrier", carrierTextBox.Text));
                
            report = new LocalReport();
            report.ReportPath = @".\carrier_label.rdlc";
            report.SetParameters(new ReportParameter("Carrier", carrierTextBox.Text));

            DataTable dt = new DataTable();

            dt.Columns.Add("SEQNO", typeof(String));
            dt.Columns.Add("ACCOUNTNAME", typeof(String));
            dt.Columns.Add("CUSTORDERNO", typeof(String));
            dt.Columns.Add("X_PROJECTNAME", typeof(String));
            dt.Columns.Add("ADDRESS", typeof(String));
            dt.Columns.Add("BARCODE", typeof(byte[]));

            DataRow dr = dt.NewRow();
            dr["SEQNO"] = row.Cells["#"].Value.ToString();
            dr["ACCOUNTNAME"] = row.Cells["ACCOUNTNAME"].Value.ToString();
            dr["CUSTORDERNO"] = row.Cells["CUSTORDERNO"].Value.ToString();
            dr["X_PROJECTNAME"] = row.Cells["X_PROJECTNAME"].Value.ToString();
            dr["ADDRESS"] = row.Cells["ADDRESS1"].Value.ToString() + ' ' + row.Cells["ADDRESS2"].Value.ToString();

            Bitmap bitmap = GenerateBarcode(row.Cells["#"].Value.ToString(), 300, 300, 0);
            dr["BARCODE"] = (byte[])(new ImageConverter().ConvertTo(bitmap, typeof(byte[])));

            dt.Rows.Add(dr);

            report.DataSources.Add(new ReportDataSource("DataSet1", dt));
            reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt));

            reportViewer1.RefreshReport();
            initRadioButtons();
        }

        private void initRadioButtons()
        {
            CustomRadioButton.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
            TntRadioButton.CheckedChanged += new EventHandler(radioButtons_CheckedChanged);
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (CustomRadioButton.Checked)
            {
                printLabelBtn.Enabled = true;
                printOnlyBtn.Enabled = true;
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Carrier", carrierTextBox.Text));
                carrierTextBox.TextChanged += carrierTextBox_TextChanged;
            }
            else if (TntRadioButton.Checked)
            {
                printLabelBtn.Enabled = false;
                printOnlyBtn.Enabled = false;
                reportViewer1.LocalReport.SetParameters(new ReportParameter("Carrier", ""));
                carrierTextBox.TextChanged -= carrierTextBox_TextChanged;
            }

            reportViewer1.RefreshReport();
        }

        // Routine to provide to the report renderer, in order to
        //    save an image for each page of the report.
        private Stream CreateStream(string name,
          string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        // Export the given report as an EMF (Enhanced Metafile) file.
        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>4.01in</PageWidth>
                <PageHeight>1.88in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
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

        private void Print()
        {
            report.SetParameters(new ReportParameter("Carrier", carrierTextBox.Text));

            Export(report);

            PrintDocument printDoc = new PrintDocument();

            PaperSize paperSize = new PaperSize("EOA Label", 401, 188);

            printDoc.PrinterSettings.PrinterName = printerName;
            printDoc.PrinterSettings.Copies = (short)numericUpDown1.Value;

            printDoc.DefaultPageSettings.PaperSize = paperSize;
            printDoc.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;

            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            m_currentPageIndex = 0;
            printDoc.Print();
        }

        private void printLabelBtn_Click(object sender, EventArgs e)
        {
            Print();
            callback(carrierTextBox.Text);
            Close();
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

        private void continueBtn_Click(object sender, EventArgs e)
        {
            string carrier = carrierTextBox.Text;

            if (TntRadioButton.Checked && carrier.Length >= 21)
            {
                carrier = @"http://www.tntexpress.com.au/interaction/ASPs/Trackcon_tntau.asp?id=TRACK.ASPX&con=ECN" + carrier.Substring(12, 9);
            }

            callback(carrier);
            Close();
        }

        // Hot keys initialization here
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Return))
            {
                if (ActiveControl.Name == carrierTextBox.Name)
                {
                    continueBtn_Click(continueBtn, new EventArgs());
                }
                else
                {
                    printLabelBtn_Click(printLabelBtn, new EventArgs());
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void printOnlyBtn_Click(object sender, EventArgs e)
        {
            Print();
            Close();
        }

        private void numericUpDown1_Enter(object sender, EventArgs e)
        {
            numericUpDown1.Select(0, numericUpDown1.Text.Length);
        }

        private void carrierTextBox_TextChanged(object sender, EventArgs e)
        {
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Carrier", carrierTextBox.Text));
            reportViewer1.RefreshReport();
        }
    }
}
