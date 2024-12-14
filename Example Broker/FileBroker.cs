//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelPrinterApplication.Resources.Services;
using System.Text;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public partial class FileBroker : IFileBroker
    {

        #region Enums

        #endregion Enums
        #region Declarations
        private readonly ILoggingBroker _logger;
        private readonly IExceptionService _exceptions;
        #endregion Declarations
        #region Constructors
        public FileBroker(
            ILoggingBroker logger,
            IExceptionService exceptions)
        {
            _logger = logger;
            _exceptions = exceptions;
        }
        #endregion Constructors
        #region Properties

        #endregion Properties
        #region Public Methods
        public async ValueTask<List<string>> GetLabelTemplateFileNames(string filepath)
        {
            return await Task.Run(() =>
            GetAllTemplateFilePathsAndNames(filepath));
        }

        public ValueTask<bool> DeleteTemplate(string fullpath, string name) =>
        TryCatch(() =>
        {
            string fileToDelete = Path.Combine(fullpath, name);
            CheckFilePath(fileToDelete);

            System.IO.File.Delete(fileToDelete);
            return ValueTask.FromResult(true);
        });

        public ValueTask<bool> UpdateTemplateName(string CurrentFileName, string NewFileName) =>
        TryCatch(() =>
        {

            CheckFilePath(CurrentFileName);

            System.IO.File.Move(CurrentFileName, NewFileName);
            return ValueTask.FromResult(true);
        });

        public async ValueTask<StringBuilder> OpenZPLFile(string fullPathAndName)
        {
            StringBuilder sb = new StringBuilder();
            if (await ValidateFilePathAsync(fullPathAndName))
            {
                sb = await GetZPLFileData(fullPathAndName);
            }

            return sb;
        }
        public async ValueTask<string> OpenZPLFileAsString(string fullPathAndName)
        {
            string zPLFile = string.Empty;
            if (ValidateFilePath(fullPathAndName))
            {
                zPLFile = File.ReadAllText(fullPathAndName);
            }
            return await Task.FromResult(zPLFile);
        }

        #endregion Public Methods
        #region Private Methods
        private ValueTask<bool> CheckFilePath(string filepath) =>
        TryCatch(() =>
        {
            bool result = false;
            if (ValidateFilePath(filepath) == true)
            {
                result = true;

            }

            return ValueTask.FromResult(result);
        });

        private List<string> GetAllTemplateFilePathsAndNames(string filepath)
        {
            List<string> filenames = new List<string>();
            if (CheckFilePath(filepath).Result)
            {
                DirectoryInfo dir = new DirectoryInfo(filepath);
                FileInfo[] files = dir.GetFiles("*.tmplt");

                foreach (FileInfo file in files)
                {
                    filenames.Add(file.FullName);
                }

                return filenames;
            };
            return filenames;
        }

        private async ValueTask<StringBuilder> GetZPLFileData(string filePathAndName)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                await Task.Run(() =>
                {
                    using (StreamReader sr = new StreamReader(filePathAndName))
                    {
                        string? line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            sb.Append(line);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                FileBrokerException fbException = new FileBrokerException(ex);
                _exceptions.AddException(fbException);
            }
            return sb;
        }
        #endregion Private Methods
    }
}
