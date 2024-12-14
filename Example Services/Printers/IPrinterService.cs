//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Data.Models;
using AmetekLabelPrinterApplication.Resources.Data.Views;


namespace AmetekLabelPrinterApplication.Resources.Services.Printers
{
    public interface IPrinterService
    {
        ValueTask<PrinterNamesView> GetPrinterNamesViewAsync();
        ValueTask<bool> PrintLabel(string printerName, LabelModel labelToPrint);

        string GetDefaultPrinter(string userName);
    }
}