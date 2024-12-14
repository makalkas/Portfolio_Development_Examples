//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Data.Models;
using AmetekLabelUI.Models.Basics;
using AmetekLabelUI.Models.LabelsData.LabelViews;

namespace AmetekLabelPrinterApplication.Resources.Services.Labels
{
    public interface ILabelService
    {
        ValueTask<LabelModel> InsertDataIntoLabelTemplateAsync(LabelTemplateView template, string templatePathAndName, List<DataMapItem> Data);
    }
}