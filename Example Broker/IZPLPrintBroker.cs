using AmetekLabelPrinterApplication.Resources.Configurations;
using System.Text;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public interface IZPLPrintBroker
    {
        void PrintToThermalPrinterDriver(string printerName, string zplCommandText);
        void PrintUsingTCPIPClient(string ZPLString, ConfiguredPrinter printerConfig);
        void PrintZPL(string printerName, List<string> zplFileToPrint);
        void PrintZPL(string printerName, StringBuilder zplFileToPrint);
    }
}