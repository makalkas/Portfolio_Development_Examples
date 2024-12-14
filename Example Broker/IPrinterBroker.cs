//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Data.Models;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public interface IPrinterBroker
    {
        ValueTask<List<string>> GetPrinters();
        void LoadLabelTemplate(string TemplateName);
        void PrintDoc(LabelModel labelToPrint, string printerName, bool cancel = false);
        ValueTask<bool> PrintLabel(LabelModel labelToPrint, string printerName, bool cancel = false);
    }
}