using System.Drawing.Printing;

namespace AmetekLabelPrinterApplication.Resources.Services.Printers
{
    public partial class ZPLPrintService
    {
        private bool PrinterIsValidPrinterInPrinters(string printerNameToTest)
        {
            if (GetPrinters().Contains(printerNameToTest))
            {
                return true;
            }

            return false;
        }

        private List<string> GetPrinters()
        {

            List<string> prntrs = new List<string>();

            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                prntrs.Add(PrinterSettings.InstalledPrinters[i].ToString());
            }

            return prntrs;

        }

    }
}
