using AmetekLabelAPI.Resources;
using AmetekLabelPrinterApplication.Resources.Brokers;
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelPrinterApplication.Resources.Services.Files;
using AmetekLabelUI.Models.Basics;
using System.Text;

namespace AmetekLabelPrinterApplication.Resources.Services.Printers
{
    public partial class ZPLPrintService : IZPLPrintService
    {
        #region Declarations
        private readonly ILoggingBroker _logger;
        private readonly IConfiguration? _config;
        private readonly IExceptionService _exceptionService;
        private readonly IZPLPrintBroker _zplPrintBroker;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        LocalConfigurations? localConfigurations = default!;
        #endregion Declarations
        #region Constructors
        public ZPLPrintService(
            ILoggingBroker logger,
            IConfiguration config,
            IExceptionService exceptionService,
            IZPLPrintBroker zplPrintBroker,
            IUserService userService,
            IFileService fileService
            )
        {
            _logger = logger;
            _config = config;
            _exceptionService = exceptionService;
            _zplPrintBroker = zplPrintBroker;
            _userService = userService;
            _fileService = fileService;
            if (_config != null)
            {
                localConfigurations = _config.Get<LocalConfigurations>();
            }

        }
        #endregion Constructors

        #region Public Methods
        /// <summary>
        /// This is a general Print method that uses an installed printer driver and a memory stream and stream writer to send ZPL text to the driver.
        /// </summary>
        /// <param name="printerName">Installed printer name as listed in printers.</param>
        /// <param name="fileName">ZPL Template file to be opened and data inserted into.</param>
        /// <param name="data">a list of columns and data to be inserted into the ZPL file.</param>
        /// <returns>Task of boolean indication success or failure of method.</returns>
        public async ValueTask<bool> Print(string printerName, string fileName, List<DataMapItem> data)
        {
            if (string.IsNullOrEmpty(printerName) || !PrinterIsValidPrinterInPrinters(printerName)) return await Task.FromResult(false);
            if (!fileName.IsGoodPathAndFileName() || data.Count == 0) return await Task.FromResult(false);

            StringBuilder zplFile;
            zplFile = await _fileService.OpenZPLFile(fileName);
            if (zplFile.Length == 0) return await Task.FromResult(false);

            string result = InsertDataIntoString(data, zplFile.ToString());
            zplFile.Clear();
            zplFile.Append(result);

            _zplPrintBroker.PrintZPL(printerName, zplFile);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// This is a general Print method that allows the ZPL file to be provided as a list of strings and the data to be inserted as a list of DataMapItem(Data pairs).
        /// </summary>
        /// <param name="printerName">Installed printer name as listed in printers.</param>
        /// <param name="zplFile">ZPL Template file to be opened and data inserted into.</param>
        /// <param name="data">a list of columns and data to be inserted into the ZPL file.</param>
        public void Print(string printerName, List<string> zplFile, List<DataMapItem> data)
        {
            if (string.IsNullOrEmpty(printerName) || !PrinterIsValidPrinterInPrinters(printerName)) return;

            if (zplFile.Count == 0) { return; }
            zplFile = InsertDataIntoString(data, zplFile);
            _zplPrintBroker.PrintZPL(printerName, zplFile);
        }

        /// <summary>
        /// This is a print method that uses a ConfiguredPrinter object and the zpl file template as a string.
        /// </summary>
        /// <param name="printerName">Installed printer name as listed in printers.</param>
        /// <param name="zplFile">ZPL Template file to be opened and data inserted into.</param>
        /// <param name="printerConfig">Configuration object that holds name, IPAddress, and port info.</param>
        /// <param name="data">a list of columns and data to be inserted into the ZPL file.</param>
        public void TCPPrint(string printerName, string zplFile, ConfiguredPrinter printerConfig, List<DataMapItem> data)
        {
            if (string.IsNullOrEmpty(printerName) || !PrinterIsValidPrinterInPrinters(printerName)) return;
            if (string.IsNullOrEmpty(zplFile)) return;
            if (string.IsNullOrEmpty(printerConfig.IPAddress) || string.IsNullOrEmpty(printerConfig.Port) || string.IsNullOrEmpty(printerConfig.PrinterName))
            {
                printerConfig = GetDefaultPrinterConfig();
                if (string.IsNullOrEmpty(printerConfig.IPAddress) || string.IsNullOrEmpty(printerConfig.Port) || string.IsNullOrEmpty(printerConfig.PrinterName)) return;
            }

            zplFile = InsertDataIntoString(data, zplFile);
            _zplPrintBroker.PrintUsingTCPIPClient(zplFile, printerConfig);
        }

        public void TCPPrint(string printerName, string zplFile, List<DataMapItem> data)
        {
            if (string.IsNullOrEmpty(printerName) || !PrinterIsValidPrinterInPrinters(printerName)) return;
            if (string.IsNullOrEmpty(zplFile)) return;

            ConfiguredPrinter printerConfig = GetDefaultPrinterConfig();
            if (string.IsNullOrEmpty(printerConfig.IPAddress) || string.IsNullOrEmpty(printerConfig.Port) || string.IsNullOrEmpty(printerConfig.PrinterName)) return;


            zplFile = InsertDataIntoString(data, zplFile);
            _zplPrintBroker.PrintUsingTCPIPClient(zplFile, printerConfig);
        }

        public void PrintToDriver(string printerName, string zplFile, List<DataMapItem> data)
        {
            if (string.IsNullOrEmpty(printerName) || !PrinterIsValidPrinterInPrinters(printerName)) return;
            if (string.IsNullOrEmpty(zplFile)) return;

            zplFile = InsertDataIntoString(data, zplFile);
            _zplPrintBroker.PrintToThermalPrinterDriver(printerName, zplFile);
        }
        #endregion Public Methods
        #region Private Methods

        private ConfiguredPrinter GetDefaultPrinterConfig()
        {
            string currentUserName = _userService.CurrentUser;
            string defaultPrinterName = string.Empty;
            ConfiguredPrinter? printerConfig = default!;
            DefaultPrinters? dp = null;
            if (!string.IsNullOrEmpty(currentUserName))
            {
                dp = localConfigurations!.defaultPrintSettings!.DefaultPrinters!.Where(x => x.UserName == currentUserName).FirstOrDefault();

                if (!string.IsNullOrEmpty(defaultPrinterName) && dp != null && localConfigurations.defaultPrintSettings.ConfiguredPrinters != null)
                {
                    defaultPrinterName = dp.DefaultPrinter;
                    printerConfig = localConfigurations.defaultPrintSettings.ConfiguredPrinters.Where(x => x.PrinterName == defaultPrinterName).FirstOrDefault();
                }
                else
                {

                    if (_exceptionService != null)
                    {
                        PrinterNotFoundException pnfException = new PrinterNotFoundException();
                        ZPLPrintBrokerException zplPrintBrokerException = new ZPLPrintBrokerException(pnfException);
                        _exceptionService.AddException(zplPrintBrokerException);
                    }
                }
            }
            else
            {

                if (_exceptionService != null)
                {
                    UnknownUserException unknownUserException = new UnknownUserException();
                    ZPLPrintBrokerException zplPrintBrokerException = new ZPLPrintBrokerException(unknownUserException);
                    _exceptionService.AddException(zplPrintBrokerException);
                }
            }


            return printerConfig!;
        }

        #endregion Private Methods
    }
}
