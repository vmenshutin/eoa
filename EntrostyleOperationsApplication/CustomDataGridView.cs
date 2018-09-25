using System.Windows.Forms;
using System;

namespace EntrostyleOperationsApplication
{
    public class CustomDataGridView : DataGridView
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                var rowIndex = CurrentCell.RowIndex;
                if (CurrentCell.OwningColumn.Name == "LabelQty" && rowIndex < Rows.Count - 1)
                {
                    try
                    {
                        CurrentCell = Rows[rowIndex + 1].Cells["ItemQty"];
                    }
                    catch (InvalidOperationException) { }
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
