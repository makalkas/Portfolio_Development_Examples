//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelAPI.Models;
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelPrinterApplication.Resources.Data.Models;
using AmetekLabelPrinterApplication.Resources.Services;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning;
using System.Xml;
using System.Xml.Serialization;


namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    [SupportedOSPlatform("windows")]
    public partial class PrinterBroker : IPrinterBroker
    {
        #region Structures
        private struct PrintAreaMax
        {
            public PrintAreaMax(string paperSizeName, int maxWidth, int maxHeight)
            {
                PaperSizeName = paperSizeName;
                MaxWidth = maxWidth;
                MaxHeight = maxHeight;
            }
            public string PaperSizeName = string.Empty;
            public int MaxWidth;
            public int MaxHeight;
        }
        #endregion Structures
        #region Enums

        #endregion Enums
        #region Declarations
        private StreamReader? _streamToPrint = null;
        static string filePath = @"C:\LabelTemplates\myfile03.rtf";
        private Font? printFont = null;
        private LabelModel? LT = null;
        private PrintDocument docToPrint =
            new PrintDocument();
        private readonly ILoggingBroker _logger;
        private readonly IConfiguration _configuration = default!;
        private readonly IExceptionService _exceptionService;
        private PrintAreaMax _printAreaMax;
        private LocalConfigurations localConfigurations = default!;

        #endregion Declarations
        #region Constructors
        public PrinterBroker(
            ILoggingBroker logger,
            IConfiguration config,
            IExceptionService exceptionService)
        {
            _logger = logger;
            _configuration = config;
            _exceptionService = exceptionService;
            SetMaxPrintArea();
            if (_configuration != null)
            {
                localConfigurations = _configuration.Get<LocalConfigurations>() ?? default!;
            }

        }
        #endregion Constructors
        #region Properties
        private StreamReader streamToPrint
        {
            get
            {
                if (_streamToPrint == null)
                {
                    _streamToPrint = new StreamReader(filePath);
                }
                return _streamToPrint;
            }
            set
            {
                _streamToPrint = value;
            }
        }
        #endregion Properties
        #region Public Methods
        /// <summary>
        /// This returns all available printers on the Server.
        /// </summary>
        /// <returns>(List of) string indicating printer name</returns>
        public async ValueTask<List<string>> GetPrinters()
        {
            if (OperatingSystem.IsWindows())
            {
                List<string> prntrs = new List<string>();
                await Task.Run(() =>
                {
                    for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
                    {
                        prntrs.Add(PrinterSettings.InstalledPrinters[i].ToString());
                    }
                });
                return prntrs;
            }
            else
            {
                List<string> eRrList = new List<string>();
                eRrList.Add("Not a supported Operating system!");
                return eRrList;
            }
        }

        /// <summary>
        /// This method sends the modified (Data from Database) Label Template to the printer.
        /// </summary>
        /// <param name="labelToPrint">Endpoint model for receiving data from UI.</param>
        /// <param name="printerName">string indicating which printer to send the document to.</param>
        /// <param name="cancel">Not used yet.</param>

        public void PrintDoc(LabelModel labelToPrint, string printerName, bool cancel = false)
        {
            //This will be replaced by injectd data from the selected label from UI.
            Dictionary<string, string> ChangedValues = new Dictionary<string, string>()
            {
                {"-SALESORDER-","SO5043987" },
                {"-MODEL NUMBER-","M445721-1" },
                {"<TAGNUMBER>","8661455" }
            };

            //Deserialize the template into template object.
            LoadLabelTemplate(@"ViPAKLabels.tmplt");

            //Modify the data that will be sent to the printer.
            foreach (LabelSectionModel ls in LT!.Sections)
            {
                ModifyDataForPrint(ChangedValues, ls.Name);
            }

            //ModifyDataForPrint(ChangedValues, "Body");


            PrintCommand(printerName);

        }

        /// <summary>
        /// This method sends the incoming Label Template data to the printer.
        /// </summary>
        /// <param name="labelToPrint">Endpoint model for receiving data from UI.</param>
        /// <param name="printerName">string indicating which printer to send the document to.</param>
        /// <param name="cancel">Not used yet.</param>

        public async ValueTask<bool> PrintLabel(LabelModel labelToPrint, string printerName, bool cancel = false)
        {
            //ToDo: write print code that takes a LabelTemplate with specific data and print it.
            LT = labelToPrint;
            string TemplateName = LT.Properties.Template_Name + ".tmplt";
            if (!string.IsNullOrEmpty(LT.Properties.TemplateDirectory))
            {
                filePath = LT.Properties.TemplateDirectory + TemplateName;
            }
            else
            {
                LocalConfigurations config = _configuration.Get<LocalConfigurations>()!;
                if (config != null && !string.IsNullOrEmpty(config.templateFilePath.DefaultPath))
                {
                    filePath = config.templateFilePath.DefaultPath + TemplateName;
                }
            }
            await Task.Delay(1000);
            await Task.Run(() => PrintCommand(printerName));
            return true;
        }

        /// <summary>
        /// Loads Template data to be worked with.
        /// </summary>

        public void LoadLabelTemplate(string TemplateName)
        {
            string TempName = TemplateName;
            string TempDir = @"C:\LabelTemplates";
            string fullPath = Path.Combine(TempDir, TempName);
            LT = new LabelModel(TempName, TempDir);

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fullPath);

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
                string msg = ex.Message;
            }
        }
        #endregion Public Methods
        #region Private Methods
        /// <summary>
        /// Called by constructor set the print area boundaries.
        /// </summary>
        private void SetMaxPrintArea()
        {
            var localConfig = _configuration.Get<LocalConfigurations>();
            if (localConfig != null)
            {
                string? paperSize = localConfig.defaultPrintSettings!.PaperSize ?? "A4";
                PaperSizesModel sizes = new PaperSizesModel(paperSize);

                _printAreaMax = new PrintAreaMax(sizes.Name, sizes.WidthInPoints, sizes.HeightInPoints);
            }
        }

        /// <summary>
        /// This method is the start of the printing operation.
        /// </summary>
        private bool PrintCommand(string printerName)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                streamToPrint = new StreamReader(filePath);
            }
            else
            {
                return false;
            }

            try
            {
                printFont = new Font("Arial", 10);
                PageSettings pageSettings = new PageSettings();
                Margins margins = new Margins();
                margins.Left = Convert.ToInt16(LT!.Properties.LeftMargin * 100);
                margins.Top = Convert.ToInt16(LT!.Properties.TopMargin * 100);
                pageSettings.Margins = margins;
                pageSettings.PaperSize = new PaperSize(_printAreaMax.PaperSizeName, _printAreaMax.MaxWidth, _printAreaMax.MaxHeight);
                docToPrint.DefaultPageSettings = pageSettings;
                //int labelWidth = Convert.ToInt32(1000 * LT.Properties.scale);
                //int labelHeight = Convert.ToInt32(2000 * LT.Properties.scale);
                //PaperSize paperSize = new PaperSize("Custom", labelWidth, labelHeight);
                //docToPrint.DefaultPageSettings.PaperSize = paperSize;


                docToPrint.PrinterSettings.PrinterName = printerName;
                if (printerName.Contains("Microsoft Print to PDF"))
                {
                    docToPrint.DocumentName = "TestDirect01.pdf";
                    docToPrint.PrinterSettings = new PrinterSettings
                    {
                        PrinterName = printerName,
                        PrintToFile = true,
                        PrintFileName = Path.Combine(@"C:\LabelTemplates", (Path.GetRandomFileName() + ".pdf"))   //@"C:\Users\Michael Kalkas\Desktop\TestDirect01.pdf"
                        //PrintFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + Path.GetRandomFileName() + ".pdf"
                        //PrintFileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TestDirect01.pdf"
                    };

                    docToPrint.PrintController = new StandardPrintController();
                }

                docToPrint.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);

                docToPrint.Print();



            }
            catch (Exception ex)
            {

                PrintCommandException printException = new PrintCommandException(ex.Message);
                PrintBrokerException printBrokerException = new PrintBrokerException(printException);
                _exceptionService.AddException(printBrokerException);
            }
            finally
            {
                streamToPrint.Close();
            }
            return true;
        }

        /// <summary>
        /// private method that sets up the document for printing page by page. Currently used for testing code.
        /// </summary>
        /// <param name="sender">Control that called method</param>
        /// <param name="ev">Page event arguments object</param>

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            try
            {
                Graphics g = ev.Graphics!;
                g.PageUnit = GraphicsUnit.Pixel;
                g.PageScale = LT!.Properties.Scale;
                //Sets up useful fonts
                Font regular = new Font(FontFamily.GenericSansSerif, 25.0f, FontStyle.Regular);
                Font bold = new Font(FontFamily.GenericSansSerif, 28.0f, FontStyle.Bold);

                //page scale
                double factor = (double)LT!.Properties.Scale;  //Math.Min(((double)bitmapSize) / image.Width, ((double)bitmapSize) / image.Height);

                int spaceBetweenLines = int.Parse(LT!.Properties.LineBorder);

                float nextLineStart = 0;

                float labelscale = LT.Properties.LabelScale; //0.50f; //
                float scaleMultiplier = (1 - labelscale);
                int imgHeight = 0;
                //g.ScaleTransform(labelscale, labelscale);
                //----------------------------------------Needs to accomodate multiple sections in template.
                //----------------------------------------Needs to accomodate above and below image setting for text position
                foreach (LabelSectionModel ls in LT.Sections)
                {

                    if (ls.LayoutOptions == LabelSectionModel.TextLocation.below)
                    {
                        if (ls.HasImage)
                        {
                            //gets the header icon image
                            Image image = GetHeaderImage(ls);

                            float hardLeftMargin = (ls.LeftMargin / 100) * g.DpiX;
                            //Sets up the rectangulare area that will hold the image.

                            //Adjusts the image scale if it is too large horizontaly or vertically.
                            (int width, int height, double scale) _dimensionValues = AutoSizeDimensions(image.Width, image.Height, labelscale);
                            labelscale = (float)_dimensionValues.scale;
                            Rectangle bound = new Rectangle((int)hardLeftMargin, int.Parse(nextLineStart.ToString()), _dimensionValues.width, _dimensionValues.height);
                            int bitmapSize = Math.Min(bound.Width, bound.Height);

                            //int imgHeight = 0;
                            using (Bitmap img = new Bitmap(image, new System.Drawing.Size((int)(labelscale * image.Width), (int)(labelscale * image.Height))))
                            {
                                Point p = new Point(bound.Left, bound.Top);
                                g.DrawImage(img, p);
                                imgHeight = (int)(img.Height + bound.Top);
                            }

                            //scaledheight = (int)((imgHeight + (imgHeight * scaleMultiplier)) * labelscale);
                            //imgHeight = (int)(((scaledheight * 96) / 72) + spaceBetweenLines * labelscale);
                            int imgHeightInPixels = (int)(imgHeight * 6.20915032);
                            nextLineStart = (imgHeightInPixels / g.PageScale) + ((ls.BottomMargin / 100) * g.DpiY);//bound.Bottom + spaceBetweenLines; //(int)((scaledheight * 96) / 72)
                        }


                        //----------------------------------------Convert to a foreach of line
                        foreach (LineModel line in ls.Lines)
                        {
                            float _lblSize;
                            // add label handling code here!
                            if (line.LineLabel != null)
                            {
                                _lblSize = (line.LineLabel.FontSizeEM * 96) / 72;  //this assumes a resolution of 96 dpi for a typical printer.
                            }
                            else
                            {
                                _lblSize = (int.Parse(line.FontSize) * 96) / 72;
                            }
                            //Font lf = new Font(GetFontFamily(line.LineLabel.FontFamily), _lblSize, GetFontStyle(line.LineLabel.fontStyle.ToString()), GraphicsUnit.Point, (byte)0);
                            //g.DrawString(line.LineLabel.Text, lf, Brushes.Black, ls.LeftMargin, nextLineStart);
                            //g.Save();
                            //nextLineStart += CalculatespacingfromFonttypeSize(lf);

                            float _emSize = (float.Parse(line.FontSize) * 96) / 72;
                            Font f = new Font(GetFontFamily(line.FontFamily), _emSize, GetFontStyle(line.FontWeight), GraphicsUnit.Point, (byte)0); //

                            float hardLeftMargin = (ls.LeftMargin / 100) * g.DpiX;
                            g.DrawString(line.Text, f, Brushes.Black, hardLeftMargin, nextLineStart);
                            SizeF textSize = g.MeasureString(line.Text, f);
                            nextLineStart += textSize.Height + spaceBetweenLines;
                            g.Save();
                        }
                        ev.HasMorePages = false;
                    }
                    nextLineStart += ((ls.BottomMargin / 100) * g.DpiY);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                PDPrintException methodException = new PDPrintException(msg);
                PrintBrokerException printBrokerException = new PrintBrokerException(methodException);
                _exceptionService.AddException(printBrokerException);
            }
        }


        private (int width, int height, double scale) AutoSizeDimensions(int Width, int Height, double scaleFactor)
        {
            (int width, int height, double scale) values = new();
            int objectWidth = (int)(Width * scaleFactor);
            int objectHeight = (int)(Height * scaleFactor);
            double newScaleFactor = new();

            if (objectWidth > _printAreaMax.MaxWidth || objectHeight > _printAreaMax.MaxHeight)
            {
                int overRunWidth = Math.Abs(_printAreaMax.MaxWidth - objectWidth);
                int overRunHeight = Math.Abs(_printAreaMax.MaxHeight - objectHeight);

                if (overRunWidth > overRunHeight)
                {
                    newScaleFactor = ((double)_printAreaMax.MaxWidth / (double)objectWidth);
                }
                else if (overRunWidth < overRunHeight)
                {
                    newScaleFactor = _printAreaMax.MaxHeight / objectHeight;
                }

                values.width = _printAreaMax.MaxWidth;
                values.height = _printAreaMax.MaxHeight;
                values.scale = newScaleFactor;
            }
            else
            {
                values.width = objectWidth;
                values.height = objectHeight;
                values.scale = scaleFactor;
            }

            return values;
        }

        private int CalculatespacingfromFonttypeSize(Font font)
        {
            int size = 0;
            if (font != null && font!.Bold == true)
            {
                size = font!.Height + 3;
            }
            else
            {
                size = font!.Height + 2;
            }
            return (size * 96) / 72;
        }

        private FontStyle GetFontStyle(string LineFontStyle)
        {
            //FontStyle.Underline
            //FontStyle.Bold
            //FontStyle.Strikeout
            //FontStyle.Regular
            //FontStyle.Italic
            switch (LineFontStyle)
            {
                case "Underline":
                    return FontStyle.Underline;
                case "Bold":
                    return FontStyle.Bold;
                case "Strikeout":
                    return FontStyle.Strikeout;
                case "Italic":
                    return FontStyle.Italic;
                default:
                    //should fall through and return the regular font style.
                    return FontStyle.Regular;
            }

        }

        private FontFamily GetFontFamily(string LineFontFamily)
        {

            FontFamily[] ff = FontFamily.Families;

            foreach (FontFamily f in ff)
            {
                if (f.Name == LineFontFamily)
                {
                    return f;
                }
            }
            return FontFamily.GenericMonospace;
        }

        private Font GetCorrectFont(LineModel LineToGetFontFrom)
        {
            FontFamily ff = FontFamily.GenericSansSerif;

            switch (LineToGetFontFrom.FontFamily)
            {
                case "Currier New":
                    ff = FontFamily.GenericSansSerif;
                    break;
                default:
                    ff = FontFamily.GenericMonospace;
                    break;
            }
            float fntSize = float.Parse(LineToGetFontFrom.FontSize);

            Font regular = new Font(ff, fntSize, FontStyle.Regular);

            return regular;
        }

        private void ModifyDataForPrint(Dictionary<string, string> ValuesToChange, string SectionType)
        {
            //Need to match section type and then find correct Line with Key that needs replaced.
            foreach (LabelSectionModel LS in LT!.Sections)
            {
                if (LS.Name == SectionType)
                {
                    //Search for and replace first key-value pair data.
                    foreach (LineModel line in LS.Lines)
                    {
                        foreach (string s in ValuesToChange.Keys)
                        {
                            if (line.Text == s)
                            {
                                line.Text = ValuesToChange.First(k => k.Key == s).Value;
                            }
                        }

                    }
                }

            }
        }

        private Image GetHeaderImage(LabelSectionModel labelSection)
        {
            string imagelocation;
            if (string.IsNullOrWhiteSpace(labelSection.ImageLocation))
            {

                imagelocation = _configuration.GetSection("Logo").GetSection("Default").Value ?? _configuration.GetSection("Logo").GetSection("Default").Value ?? @"C:\Users\Michael Kalkas\Pictures\OBRIAN AMETEK.jpg";
            }
            else
            {
                imagelocation = labelSection.ImageLocation;
            }

            Image img;
            if (File.Exists(imagelocation))
            {
                img = new Bitmap(imagelocation);
                TextureBrush brush = new TextureBrush(img);
                brush.ScaleTransform(0, 0);
            }
            else
            {
                //ToDo: change out hard coded logo location for a configuration item.
                img = new Bitmap(@"C:\Users\Michael Kalkas\Pictures\OBRIAN AMETEK.jpg");
            }



            return img;
        }

        private PointF ConvertInchesToPixels(float InchesToconvertToPixels)
        {
            PointF pf = new PointF();
            pf.X = InchesToconvertToPixels * 96;
            return pf;//(PointF)(InchesToconvertToPixels * 96);
        }

        private void CreateExampleLabelTemplateObjectForTesting()
        {
            LT = new LabelModel();

            //Add settings
            LT.Properties.Template_Name = "Testlabel";
            LT.Properties.TemplateDirectory = @"C:\";
            LT.Properties.Template_Description = "Virtual Template created from Code.";
            LT.Properties.LinesPerPage = 10;
            LT.Properties.HasHeader = true;
            LT.Properties.HasFooter = false;
            LT.Properties.HasBody = true;
            LT.Properties.TemplateHeight = 800;
            LT.Properties.TemplateWidth = 300;
            LT.Properties.Scale = 0.4f;
            LT.Properties.LeftMargin = 0;
            LT.Properties.TopMargin = 0;
            LT.Properties.DefaultFont = "SansSerif";
            LT.Properties.SQLQuery = @"SELECT [SalesOrderNumber] FROM [dbo].[SchedDB_ViPAK] WHERE [Document No_ = 'SO418608'";

            //Add Sections to Label
            LabelSectionModel labelSection = new LabelSectionModel();
            labelSection.Name = "Header";
            labelSection.LayoutOptions = LabelSectionModel.TextLocation.below;
            //labelSection.sectionImage = GetHeaderImage(labelSection);
            LT.Sections.Add(labelSection);

            LabelSectionModel labelSection1 = new LabelSectionModel();
            labelSection1.Name = "Body";
            labelSection1.LayoutOptions = LabelSectionModel.TextLocation.below;
            //labelSection.HasImage = true;
            labelSection1.AddLines("Currier New", "18", "Bold", "This is an example Header Line", LabelModel.StandardFontStyle.Underlined, false);
            labelSection1.AddLines("Currier New", "12", "Normal", "This is an example standard text Line", LabelModel.StandardFontStyle.Regular, false);
            labelSection1.AddLines("Currier New", "18", "Bold", "This is another Header Line", LabelModel.StandardFontStyle.Underlined, false);
            labelSection1.AddLines("Currier New", "12", "Normal", "This is another standard text Line", LabelModel.StandardFontStyle.Regular, false);
        }
        #endregion Private Methods

    }

}
