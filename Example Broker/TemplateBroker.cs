//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelPrinterApplication.Resources.Data.Models;
using AmetekLabelPrinterApplication.Resources.Services;
using System.Runtime.Versioning;
using System.Xml;
using System.Xml.Serialization;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    [SupportedOSPlatform("Windows")]
    public partial class TemplateBroker : ITemplateBroker
    {
        private readonly IExceptionService _exceptions;
        private readonly IConfiguration _configuration;
        private LocalConfigurations? _localConfigurations;

        public TemplateBroker(
            IExceptionService exceptions,
            IConfiguration configuration)
        {
            _exceptions = exceptions;
            _configuration = configuration;
            if (configuration != null)
            {
                _localConfigurations = configuration.Get<LocalConfigurations>()!;
            }
        }

        public async ValueTask<string> GetSQLScript(string TemplatePathAndName)
        {
            if (!TemplatePathIsValid(TemplatePathAndName, _localConfigurations!.templateFilePath.DefaultExtension! ?? @".tmplt")) return string.Empty;
            LabelModel model = await GetLabelTemplateFromFile(TemplatePathAndName);
            string SQLScript = model.Properties.SQLQuery;

            return SQLScript;
        }

        public async ValueTask<LabelModel> GetLabelTemplateFromFile(string templateFilePathAndName)
        {
            if (!TemplatePathIsValid(templateFilePathAndName, _localConfigurations!.templateFilePath.DefaultExtension! ?? @".tmplt")) return await ValueTask.FromResult(new LabelModel());
            return await Task.Run(() => OpenLabelTemplateFromFile(templateFilePathAndName));
        }

        /// <summary>
        /// Opens a new Label Template object and populates it from the corresponding file.
        /// </summary>
        /// <param name="fullPathAndName"></param>
        /// <returns></returns>
        private LabelModel OpenLabelTemplateFromFile(string fullPathAndName)
        {
            XmlDocument doc = new XmlDocument();
            LabelModel LT;
            try
            {

                if (string.IsNullOrEmpty(fullPathAndName)) throw new BadFilePathException(fullPathAndName);

                doc.Load(fullPathAndName);

                XmlSerializer serializer = new XmlSerializer(typeof(LabelModel));

                using (StringReader reader = new StringReader(doc.InnerXml))
                {
                    LT = (LabelModel)serializer.Deserialize(reader)!;
                }

                foreach (LabelSectionModel ls in LT.Sections)
                {
                    ls.checkForImage();
                }
            }
            catch (Exception ex)
            {
                throw new TemplateBrokerException($"Error in creating new LabelModel object:{ex.Message}", ex);
            }

            return LT;
        }
    }
}
