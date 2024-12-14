//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Data.Views;

namespace AmetekLabelPrinterApplication.Resources.Services.Fonts
{
    public interface IFontService
    {
        ValueTask<FontsView> GetFontsAsync();
    }
}