using AmetekLabelPrinterApplication.Resources.Brokers;
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Models;
using AmetekLabelUI.Models.LabelsData.LabelViews;
using OtripleS.Portal.Web.Brokers.DateTimes;

namespace AmetekLabelPrinterApplication.Resources.Services.Templates
{
    public class TemplatesViewService : ITemplatesViewService
    {
        #region Declerations
        private readonly IUserService _userService;
        private readonly IDateTimeBroker _dateTimeBroker;
        private readonly ILoggingBroker _loggingBroker;
        private readonly IConfiguration? _configuration = null;
        private readonly ITemplateBroker _templatesBroker;
        private LocalConfigurations? _localConfigurations = null;
        #endregion Declarations

        #region Constructors
        public TemplatesViewService(
             IUserService userService,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker,
            IConfiguration configuration,
            ITemplateBroker templatesBroker)
        {
            _userService = userService;
            _dateTimeBroker = dateTimeBroker;
            _loggingBroker = loggingBroker;
            _configuration = configuration;
            _templatesBroker = templatesBroker;
            if (_configuration != null)
            {
                _localConfigurations = _configuration.Get<LocalConfigurations>();
            }
        }
        #endregion Constructors

        #region Public Methods
        public ValueTask<LabelModel> GetTemplate(string templateName)
        {
            //Ensure templateName contains a proper path and file name.
            templateName = EnsureGoodFilePath(templateName);
            return _templatesBroker.GetLabelTemplateFromFile(templateName);
        }

        //public void UpdateTemplate(string templateName, string templatePath, LabelModel template) => _templatesBroker.UpdateTemplate(templateName, templatePath, template);

        //public void SaveTemplate(string templateName, string templatePath, LabelModel template) => _templatesBroker.SaveTemplate(templateName.Trim(), templatePath.Trim(), template);


        public LabelModel ConvertLabelViewToLabelModel(LabelTemplateView labelView)
        {
            LabelModel labelModel = labelView.ToLabelModel();

            return labelModel;
        }

        public LabelTemplateView ConvertToLabelTemplateView(LabelModel template)
        {
            LabelTemplateView templateView = template.ToLabelTemplateView();

            return templateView;
        }

        #endregion Public Methods

        #region Private Methods
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
