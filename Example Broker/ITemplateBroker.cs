//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Data.Models;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public interface ITemplateBroker
    {
        ValueTask<LabelModel> GetLabelTemplateFromFile(string templateFilePathAndName);
        ValueTask<string> GetSQLScript(string TemplatePathAndName);
    }
}