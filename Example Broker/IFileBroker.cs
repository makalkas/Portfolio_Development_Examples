//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using System.Text;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public interface IFileBroker
    {
        ValueTask<bool> DeleteTemplate(string fullpath, string name);
        ValueTask<List<string>> GetLabelTemplateFileNames(string filepath);
        ValueTask<StringBuilder> OpenZPLFile(string fullPathAndName);
        ValueTask<string> OpenZPLFileAsString(string fullPathAndName);
        ValueTask<bool> UpdateTemplateName(string CurrentFileName, string NewFileName);
    }
}