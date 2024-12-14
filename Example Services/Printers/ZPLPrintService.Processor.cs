using AmetekLabelUI.Models.Basics;

namespace AmetekLabelPrinterApplication.Resources.Services.Printers
{
    /// <summary>
    /// This class holds methods for handling the conversion of View data objects to Model data objects for the ZPLPrint Service class as well as other necessary processing.
    /// </summary>
    public partial class ZPLPrintService
    {
        private string InsertDataIntoString(List<DataMapItem> data, string zplfile)
        {
            if (data.Count > 0)
            {
                foreach (DataMapItem item in data)
                {
                    zplfile.Replace("--" + item.ColumnName + "--", item.Data);
                }
            }
            return zplfile;
        }

        private List<string> InsertDataIntoString(List<DataMapItem> data, List<string> zplfile)
        {

            if (data.Count > 0)
            {
                foreach (DataMapItem item in data)
                {
                    foreach (string s in zplfile)
                    {
                        s.Replace("--" + item.ColumnName + "--", item.Data);
                    }
                }
            }

            return zplfile;
        }
    }
}
