using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelUI.Models.Basics;

namespace AmetekLabelPrinterApplication.Resources.Services.Printers
{
    public interface IZPLPrintService
    {
        void Print(string printerName, List<string> zplFile, List<DataMapItem> data);
        ValueTask<bool> Print(string printerName, string fileName, List<DataMapItem> data);
        void PrintToDriver(string printerName, string zplFile, List<DataMapItem> data);
        void TCPPrint(string printerName, string zplFile, ConfiguredPrinter printerConfig, List<DataMapItem> data);
        void TCPPrint(string printerName, string zplFile, List<DataMapItem> data);
    }
}