//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Brokers;
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelPrinterApplication.Resources.Data.Models;
using AmetekLabelPrinterApplication.Resources.Data.Views;

namespace AmetekLabelPrinterApplication.Resources.Services.Printers
{
    public class PrinterService : IPrinterService
    {
        #region Enums

        #endregion Enums
        #region Declarations
        private readonly ILoggingBroker _logging;
        private readonly IPrinterBroker _printerBroker;
        private readonly IExceptionService _exceptions;
        private readonly IConfiguration? _config;
        private LocalConfigurations? _localconfiguration;

        #endregion Declarations
        #region Constructors
        public PrinterService(
            ILoggingBroker logging,
            IPrinterBroker printerBroker,
            IExceptionService exceptions,
            IConfiguration config)
        {
            _logging = logging;
            _printerBroker = printerBroker;
            _exceptions = exceptions;
            _config = config;
            if (_config != null)
            {
                _localconfiguration = _config.Get<LocalConfigurations>()!;
            }
        }
        #endregion Constructors
        #region Properties

        #endregion Properties
        #region Public Methods
        public async ValueTask<PrinterNamesView> GetPrinterNamesViewAsync()
        {
            PrinterNamesView view = new PrinterNamesView();

            List<string> printerNames = await _printerBroker.GetPrinters();
            foreach (string printerName in printerNames)
            {
                view.AddPrinterName(printerName);
            }

            return view;
        }

        public async ValueTask<bool> PrintLabel(string printerName, LabelModel labelToPrint)
        {
            await _printerBroker.PrintLabel(labelToPrint, printerName);

            return true;
        }

        public string GetDefaultPrinter(string userName)
        {
            string? DefaultPrinterName = string.Empty;
            try
            {
                List<DefaultPrinters> printers = new();

                if (_localconfiguration != null && _localconfiguration.defaultPrintSettings != null)
                {
                    printers = _localconfiguration.defaultPrintSettings.DefaultPrinters!;
                }
                if (printers.Count == 0)
                {
                    HandleConfigurationException();
                }
                else
                {
                    var defaultPrinter = printers.Find(x => x.UserName == userName)!.DefaultPrinter;

                    if (defaultPrinter != null && !string.IsNullOrEmpty(defaultPrinter))
                    {
                        DefaultPrinterName = defaultPrinter;
                    }
                    else
                    {
                        HandleUserNotFoundException(userName);
                    }
                }
            }
            catch (Exception ex)
            {
                PrinterServiceException pse = new PrinterServiceException(ex);
                _exceptions.AddException(pse);
            }
            return DefaultPrinterName;
        }
        #endregion Public Methods
        #region Private Methods
        private void HandleConfigurationException()
        {
            string msg = "The Printer Service encountered an error while accessing the defaultPrinterSettings configuration. "
                + "Please verify that the default printers list is properly configured in the appsettings.Json file.";
            EmptyConfigurationsException configException = new EmptyConfigurationsException(msg);
            PrinterServiceException parent = new PrinterServiceException(configException);
            _exceptions.AddException(parent);
        }
        private void HandleUserNotFoundException(string userName)
        {
            UserNotFoundException notFoundEx =
                new UserNotFoundException(
                    $"The user {userName} was not found in the configured default printers list.");
            PrinterServiceException parent = new PrinterServiceException(notFoundEx);
            _exceptions.AddException(parent);
        }
        #endregion Private Methods
    }
}
