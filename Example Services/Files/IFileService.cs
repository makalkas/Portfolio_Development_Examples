//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Data.Views;
using System.Text;

namespace AmetekLabelPrinterApplication.Resources.Services.Files
{
    public interface IFileService
    {
        bool DeleteTemplate(string name);
        ValueTask<TemplateFileNamesView> GetLabelTemplateFileNamesView();
        ValueTask<StringBuilder> OpenZPLFile(string fullPathAndName);
        ValueTask<string> OpenZPLFileAsString(string fullPathAndName);
        bool UpdateTemplateName(string CurrentFileName, string NewFileName);
    }
}