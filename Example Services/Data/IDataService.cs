//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelUI.Models.LabelsData.LabelViews;

namespace AmetekLabelPrinterApplication.Resources.Services.Data
{
    public interface IDataService
    {
        ValueTask<List<ViPAKLabelView>> GetViPAKLabelData();
    }
}