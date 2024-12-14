using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Configurations;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using AmetekLabelPrinterApplication.Resources.Services;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning;
using System.Text;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    [SupportedOSPlatform("windows")]
    public class ZPLPrintBroker : IZPLPrintBroker
    {
        #region Declarations
        private readonly ILoggingBroker _logger;
        private readonly IConfiguration _config;
        private readonly IExceptionService _exceptionService;

        #endregion Declarations

        #region Public Properties


        #endregion Public Properties

        #region Constructors
        public ZPLPrintBroker(
            ILoggingBroker logger,
            IConfiguration config,
            IExceptionService exceptionService)
        {
            _logger = logger;
            _config = config;
            _exceptionService = exceptionService;
        }

        #endregion Constructors

        #region Public Methods
        public void PrintZPL(string printerName, StringBuilder zplFileToPrint)
        {
            try
            {
                // Create a memory stream
                using (MemoryStream ms = new MemoryStream())
                {
                    // Write ZPL commands to the memory stream
                    using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8))
                    {

                        writer.Write(zplFileToPrint.ToString());
                        writer.Flush();
                        ms.Position = 0;

                        // Create a PrintDocument
                        PrintDocument pd = new PrintDocument();
                        pd.PrintPage += (sender, args) =>
                        {
                            pd.PrinterSettings.PrinterName = printerName;
                            // Read the memory stream and print its content
                            using (StreamReader reader = new StreamReader(ms))
                            {
                                string zpl = reader.ReadToEnd();
                                args.Graphics!.DrawString(zpl, new Font("Arial", 10), Brushes.Black, new PointF(100, 100));
                            }
                        };

                        // Print the document
                        pd.Print();
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(e);
                _exceptionService.AddException(brokerException);
            }
            catch (IOException e)
            {
                string message = "There was an IO Exception in the ZPLPrint Broker.Please see the inner exception for more details.";
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(message, e);
                _exceptionService.AddException(brokerException);
            }
            catch (Exception e)
            {
                string message = "There was an unexpected Exception in the ZPLPrint Broker.Please see the inner exception for more details.";
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(message, e);
                _exceptionService.AddException(brokerException);
            }
        }

        public void PrintZPL(string printerName, List<string> zplFileToPrint)
        {
            try
            {// Create a memory stream
                using (MemoryStream ms = new MemoryStream())
                {
                    // Write ZPL commands to the memory stream
                    using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8))
                    {
                        foreach (string zpl in zplFileToPrint)
                        {
                            writer.Write(zpl);
                        }
                        writer.Flush();
                        ms.Position = 0;

                        // Create a PrintDocument
                        PrintDocument pd = new PrintDocument();
                        pd.PrintPage += (sender, args) =>
                        {
                            // Read the memory stream and print its content
                            using (StreamReader reader = new StreamReader(ms))
                            {
                                string zpl = reader.ReadToEnd();
                                args.Graphics!.DrawString(zpl, new Font("Arial", 10), Brushes.Black, new PointF(100, 100));
                            }
                        };

                        // Print the document
                        pd.Print();
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(e);
                _exceptionService.AddException(brokerException);
            }
            catch (IOException e)
            {
                string message = "There was an IO Exception in the ZPLPrint Broker.Please see the inner exception for more details.";
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(message, e);
                _exceptionService.AddException(brokerException);
            }
            catch (Exception e)
            {
                string message = "There was an unexpected Exception in the ZPLPrint Broker.Please see the inner exception for more details.";
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(message, e);
                _exceptionService.AddException(brokerException);
            }
        }

        public void PrintUsingTCPIPClient(string ZPLString, ConfiguredPrinter printerConfig)
        {
            // Printer IP Address and communication port
            string ipAddress = printerConfig.IPAddress;
            int port = int.Parse(printerConfig.Port);



            try
            {
                // Open connection
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect(ipAddress, port);

                // Write ZPL String to connection
                System.IO.StreamWriter writer =
                new System.IO.StreamWriter(client.GetStream());
                writer.Write(ZPLString);
                writer.Flush();

                // Close Connection
                writer.Close();
                client.Close();
            }
            catch (FileNotFoundException e)
            {
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(e);
                _exceptionService.AddException(brokerException);
            }
            catch (IOException e)
            {
                string message = "There was an IO Exception in the ZPLPrint Broker.Please see the inner exception for more details.";
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(message, e);
                _exceptionService.AddException(brokerException);
            }
            catch (Exception e)
            {
                string message = "There was an unexpected Exception in the ZPLPrint Broker.Please see the inner exception for more details.";
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(message, e);
                _exceptionService.AddException(brokerException);
            }
        }

        public void PrintToThermalPrinterDriver(string printerName, string zplCommandText)
        {
            try
            {
                // Convert the ZPL command to a byte array
                byte[] zplBytes = Encoding.UTF8.GetBytes(zplCommandText);

                // Create a memory stream from the byte array
                using (MemoryStream ms = new MemoryStream(zplBytes))
                {
                    // Create a PrintDocument object
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += (sender, args) =>
                    {
                        pd.PrinterSettings.PrinterName = printerName;
                        // Use the Graphics object to draw the ZPL data
                        args.Graphics!.DrawImage(Image.FromStream(ms), new PointF(0, 0));
                    };

                    // Print the document
                    pd.Print();
                }
            }
            catch (FileNotFoundException e)
            {
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(e);
                _exceptionService.AddException(brokerException);
            }
            catch (IOException e)
            {
                string message = "There was an IO Exception in the ZPLPrint Broker.Please see the inner exception for more details.";
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(message, e);
                _exceptionService.AddException(brokerException);
            }
            catch (Exception e)
            {
                string message = "There was an unexpected Exception in the ZPLPrint Broker.Please see the inner exception for more details.";
                ZPLPrintBrokerException brokerException = new ZPLPrintBrokerException(message, e);
                _exceptionService.AddException(brokerException);
            }
        }

        #endregion Public Methods

        #region Private Methods


        #endregion Private Methods
    }
}
