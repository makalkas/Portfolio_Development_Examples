//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Brokers;
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelPrinterApplication.Resources.Data.Models;
using AmetekLabelUI.Models.Basics;
using AmetekLabelUI.Models.LabelsData.LabelViews;

namespace AmetekLabelPrinterApplication.Resources.Services.Labels
{
    public class LabelService : ILabelService
    {
        #region Enums

        #endregion Enums
        #region Declarations
        private readonly IConfiguration _configuration = default!;
        private readonly ILoggingBroker _logger;
        private readonly IExceptionService _exceptions;
        private readonly ITemplateBroker _templateBroker;
        private LocalConfigurations _localConfigurations = default!;
        #endregion Declarations
        #region Constructors
        public LabelService(
            IConfiguration configuration,
            ILoggingBroker logger,
            IExceptionService exceptions,
            ITemplateBroker templateBroker)
        {
            _configuration = configuration;
            _logger = logger;
            _exceptions = exceptions;
            _templateBroker = templateBroker;
            if (_configuration != null) //&& _configuration.Get<LocalConfigurations>() != null
            {
                _localConfigurations = _configuration.Get<LocalConfigurations>()!;
            }
            else
            {
                NullOrEmptyConfigurationException configException = new NullOrEmptyConfigurationException();
                LabelServiceException exception = new LabelServiceException(configException);
                logger.LogError(exception);
                _exceptions.AddException(exception);
            }
        }
        #endregion Constructors
        #region Properties

        #endregion Properties
        #region Public Methods
        public async ValueTask<LabelModel> InsertDataIntoLabelTemplateAsync(LabelTemplateView template, string templatePathAndName, List<DataMapItem> Data)
        {
            if (!templatePathAndName.Contains(@"\") || !templatePathAndName.Contains(@"."))
            {
                templatePathAndName = EnsureGoodFilePath(templatePathAndName);
            }
            LabelModel model = template.ToLabelModel();

            LabelModel newModel = await _templateBroker.GetLabelTemplateFromFile(templatePathAndName);

            if (model.Properties.Template_Name == newModel.Properties.Template_Name)
            {
                model = MapDataToLabelTemplateModel(newModel, Data);
            }

            return model;
        }
        #endregion Public Methods
        #region Private Methods
        private LabelModel MapDataToLabelTemplateModel(LabelModel model, List<DataMapItem> data)
        {
            data.Add(new DataMapItem("DATE", DateTime.Today.ToString("d")));
            foreach (DataMapItem item in data)
            {
                string searchItem = "--" + item.ColumnName + "--";
                foreach (LabelSectionModel section in model.Sections)
                {
                    foreach (LineModel line in section.Lines)
                    {
                        if (line.MapsTo.Contains(searchItem) | line.Text.Contains(searchItem))
                        {
                            line.Text = line.Text.Replace(searchItem, item.Data);
                        }
                    }
                }
            }

            return model;
        }

        private string EnsureGoodFilePath(string testString)
        {
            if (string.IsNullOrEmpty(testString)) return string.Empty;

            string defaultPath = string.Empty;
            string defaultExtension = string.Empty;
            if (_localConfigurations != null &&
            _localConfigurations.templateFilePath != null &&
                !string.IsNullOrEmpty(_localConfigurations.templateFilePath.DefaultPath) &&
                !string.IsNullOrEmpty(_localConfigurations.templateFilePath.DefaultExtension)
                )
            {
                defaultPath = _localConfigurations.templateFilePath.DefaultPath;
                defaultExtension = _localConfigurations.templateFilePath.DefaultExtension!;
            }
            if (!string.IsNullOrEmpty(defaultPath) && !testString.Contains(@"\"))
            {
                if (!testString.Contains(@".") && !string.IsNullOrEmpty(defaultExtension))
                {
                    testString += defaultExtension;
                }
                else if (!testString.Contains(@"."))
                {
                    testString += ".tmplt";
                }

                testString = defaultPath + testString;
            }

            return testString;
        }
        #endregion Private Methods
    }
}
