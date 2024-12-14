using AmetekLabelPrinterApplication.Resources.Data.Models;
using AmetekLabelUI.Models.LabelsData.LabelViews;

namespace AmetekLabelPrinterApplication.Resources.Services.Templates
{
    public interface ITemplatesViewService
    {
        LabelModel ConvertLabelViewToLabelModel(LabelTemplateView labelView);
        LabelTemplateView ConvertToLabelTemplateView(LabelModel template);
        ValueTask<LabelModel> GetTemplate(string templateName);
    }
}