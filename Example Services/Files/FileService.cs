//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelAPI.Resources;
using AmetekLabelPrinterApplication.Resources.Brokers;
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelPrinterApplication.Resources.Data.Views;
using System.Text;

namespace AmetekLabelPrinterApplication.Resources.Services.Files
{
    public partial class FileService : IFileService
    {
        #region Enums

        #endregion Enums
        #region Declarations
        private readonly ILoggingBroker _logger;
        private readonly IConfiguration _configuration;
        private readonly IFileBroker _fileBroker;
        private readonly IExceptionService _exceptions;
        private protected string? _defaultTemplateFilePath;
        #endregion Declarations
        #region Constructors
        public FileService(
            ILoggingBroker logger,
            IConfiguration configuration,
            IFileBroker fileBroker,
            IExceptionService exceptions)
        {
            _logger = logger;
            _configuration = configuration;
            _fileBroker = fileBroker;
            _exceptions = exceptions;
            LocalConfigurations? config = configuration.Get<LocalConfigurations>();
            if (config != null)
            {
                _defaultTemplateFilePath = config.templateFilePath.DefaultPath ?? string.Empty;
            }
        }
        #endregion Constructors
        #region Properties

        private string TemplateFileDefaultPath
        {
            get
            {
                if (_defaultTemplateFilePath == null)
                {
                    LocalConfigurations? config = _configuration.Get<LocalConfigurations>();
                    if (config != null)
                    {
                        _defaultTemplateFilePath = config.templateFilePath.DefaultPath ?? string.Empty;
                    }
                }
                return _defaultTemplateFilePath ?? string.Empty;
            }
        }
        #endregion Properties
        #region Public Methods
        public async ValueTask<TemplateFileNamesView> GetLabelTemplateFileNamesView()
        {
            List<string> names = await _fileBroker.GetLabelTemplateFileNames(TemplateFileDefaultPath);
            TemplateFileNamesView views = new TemplateFileNamesView();

            foreach (string name in names)
            {
                views.AddTemplateFileName(name);
            }
            return views;
        }

        public bool DeleteTemplate(string name)
        {

            string fileToDelete = Path.Combine(TemplateFileDefaultPath, name);


            System.IO.File.Delete(fileToDelete);
            return true;

        }

        /// <summary>
        /// This method renames a file.
        /// </summary>
        /// <param name="CurrentFileName">The current file name including the full path.</param>
        /// <param name="NewFileName">The new file name with the current full file path.</param>
        /// <returns>Boolean value of true if operation was successful.</returns>
        public bool UpdateTemplateName(string CurrentFileName, string NewFileName)
        {
            CurrentFileName = EnsureNameAndExtensionOnly(CurrentFileName);
            NewFileName = EnsureNameAndExtensionOnly(NewFileName);
            string startFileName = Path.Combine(TemplateFileDefaultPath, CurrentFileName);
            string newFileName = Path.Combine(TemplateFileDefaultPath, NewFileName);


            System.IO.File.Move(startFileName, newFileName);
            return true;

        }

        public async ValueTask<StringBuilder> OpenZPLFile(string fullPathAndName)
        {
            if (fullPathAndName.IsGoodPathAndFileName())
            {
                return await _fileBroker.OpenZPLFile(fullPathAndName);
            }
            else
            {
                return await ValueTask.FromResult(new StringBuilder());
            }
        }

        public async ValueTask<string> OpenZPLFileAsString(string fullPathAndName)
        {
            if (fullPathAndName.IsGoodPathAndFileName())
            {
                return await _fileBroker.OpenZPLFileAsString(fullPathAndName);
            }
            else
            {
                return await ValueTask.FromResult(string.Empty);
            }
        }
        #endregion Public Methods
        #region Private Methods
        private string EnsureNameAndExtensionOnly(string StringToCheck)
        {
            try
            {
                if (StringToCheck.Contains("/") || !StringToCheck.Contains(@"."))
                {
                    throw new InvalidPathStringException();
                }
                string[] checkItems = new string[] { @"C:\", @"\", "\\" };
                bool needsTruncated = false;
                foreach (string checkItem in checkItems)
                {
                    if (StringToCheck.Contains(checkItem))
                    {
                        needsTruncated = true;
                        break;
                    }
                }

                if (needsTruncated)
                {
                    StringToCheck = StringToCheck.Substring(StringToCheck.LastIndexOf(@"\"), StringToCheck.Length);
                }
                return StringToCheck;
            }
            catch (Exception e)
            {
                FileServiceException fileServiceException = new FileServiceException(e);
                _exceptions.AddException(fileServiceException);

                //Put code to insert exception into exception handling class.
                StringToCheck = string.Empty;

            }
            return StringToCheck;
        }
        #endregion Private Methods
    }
}
