//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelAPI.Models;
using AmetekLabelPrinterApplication.Resources.Brokers;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelUI.Models.LabelsData.LabelViews;

namespace AmetekLabelPrinterApplication.Resources.Services.Data
{
    /// <summary>
    /// This class is for retrieving specific data based on the Label Template.
    /// </summary>
    public class DataService : IDataService
    {
        #region Enums

        #endregion Enums
        #region Declarations
        private readonly IConfiguration _configuration;
        private LocalConfigurations? _localconfigurations;
        private readonly ISQLBroker _sQLBroker;
        private readonly IExceptionService _exceptions;
        private readonly ITemplateBroker _templateBroker;
        #endregion Declarations
        #region Constructors

        public DataService(
            IConfiguration configuration,
            ISQLBroker sQLBroker,
            IExceptionService exceptions,
            ITemplateBroker templateBroker)
        {
            _configuration = configuration;
            _sQLBroker = sQLBroker;
            _exceptions = exceptions;
            _templateBroker = templateBroker;
            _localconfigurations = _configuration!.Get<LocalConfigurations>();
        }
        #endregion Constructors
        #region Properties

        private LocalConfigurations LocalConfigurations
        {
            get
            {
                if (_localconfigurations == null)
                {
                    _localconfigurations = _configuration.Get<LocalConfigurations>();
                }
                if (_localconfigurations == null)
                {
                    string message = "Local Configurations object was null even after attempting to reinitialize it.";
                    message += " Please check the Appsettings file for corruption and/or modifications not reflected in the application.";
                    _exceptions.AddException(new DataServiceException(message));
                }
                return _localconfigurations!;
            }
        }
        #endregion Properties
        #region Public Methods
        public async ValueTask<List<ViPAKLabelView>> GetViPAKLabelData()
        {
            string templateName = GetTemplateName("ViPAKData");
            string filepath = GetDefaultTemplateFilePath();
            string fullPathAndName = Path.Combine(filepath, templateName);

            // Get SQLScript from the proper Template File.
            string SQLScript = await _templateBroker.GetSQLScript(fullPathAndName);

            SetConnectionstring();

            return ConvertToViews(await _sQLBroker.LoadListData<SchedViPAKModel, dynamic>(SQLScript, new { }));
        }

        #endregion Public Methods
        #region Private Methods
        private string GetTemplateName(string key)
        {
            if (key == "ViPAKData" && LocalConfigurations != null && LocalConfigurations.configuredLabels != null)
            {
                return LocalConfigurations!.configuredLabels!.ViPAKData;
            }
            else
            {
                return string.Empty;
            }
        }
        private void SetConnectionstring()
        {
            if (string.IsNullOrEmpty(_sQLBroker.ConnectionString))
            {
                _sQLBroker.ConnectionString = LocalConfigurations.connectionStrings.Default;
            }
        }
        private string GetDefaultTemplateFilePath()
        {
            if (LocalConfigurations != null && LocalConfigurations.templateFilePath != null && !string.IsNullOrEmpty(LocalConfigurations.templateFilePath.DefaultPath))
            {
                return LocalConfigurations!.templateFilePath!.DefaultPath;
            }
            return string.Empty;
        }

        private List<ViPAKLabelView> ConvertToViews(List<SchedViPAKModel> listToConvert)
        {
            List<ViPAKLabelView> views = new List<ViPAKLabelView>();
            foreach (var modelItem in listToConvert)
            {
                views.Add(modelItem.ToView());
            }
            return views;
        }
        #endregion Private Methods
    }
}
