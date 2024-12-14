//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public interface IFontBroker
    {
        List<string> GetFonts();

        ValueTask<List<string>> GetFontsAsync();
    }
}