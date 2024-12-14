//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Brokers;
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Views;

namespace AmetekLabelPrinterApplication.Resources.Services.Fonts
{
    public class FontService : IFontService
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly ILoggingBroker _logger;
        private readonly IExceptionService _exceptions;
        private readonly IFontBroker _fontBroker;
        private readonly LocalConfigurations? _localConfigurations;
        #endregion Declarations
        #region Constructors
        public FontService(
            IConfiguration configuration,
            ILoggingBroker logger,
            IExceptionService exceptions,
            IFontBroker fontBroker)
        {
            _configuration = configuration;
            _localConfigurations = configuration.Get<LocalConfigurations>();
            _logger = logger;
            _exceptions = exceptions;
            _fontBroker = fontBroker;
        }
        #endregion Constructors
        #region Public Methods
        public async ValueTask<FontsView> GetFontsAsync()
        {
            FontsView fontsView = new FontsView();
            List<string> fontNames = await _fontBroker.GetFontsAsync();
            foreach (string fontName in fontNames)
            {
                fontsView.AddFontName(fontName);
            }
            return fontsView;
        }
        #endregion Public Methods
        #region Private Methods

        #endregion Private Methods
    }
}
