//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelAPI.Resources;
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using System.Drawing;
using System.Runtime.Versioning;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    [SupportedOSPlatform("windows")]
    public class FontBroker : IFontBroker
    {
        #region Enums

        #endregion Enums
        #region Declarations
        private readonly ILoggingBroker _logger;
        #endregion Declarations
        #region Constructors
        public FontBroker(ILoggingBroker logger) => _logger = logger;

        #endregion Constructors
        #region Properties

        #endregion Properties
        #region Public Methods
        public List<string> GetFonts()
        {
            return FontFamily.Families.FFToList();
        }

        public async ValueTask<List<string>> GetFontsAsync()
        {
            return await Task.Run(() => FontFamily.Families.FFToList());
        }
        #endregion Public Methods
        #region Private Methods

        #endregion Private Methods

    }

}
