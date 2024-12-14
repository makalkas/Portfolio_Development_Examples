using AmetekLabelPrinterApplication.Resources.Data.Exceptions;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public partial class FileBroker
    {
        private bool ValidateFilePath(string filePath)
        {
            bool filePathIsDirectory = false;

            if (!filePath.Contains('.'))
            {
                filePathIsDirectory = true;
            }

            if (filePathIsDirectory == false)
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    throw new BadFilePathException($"The file does not exist at:{filePath}.");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(filePath) || !Directory.Exists(filePath))
                {
                    throw new BadFilePathException($"The directory does not exist at:{filePath}.");
                }
            }
            return true;
        }

        private async ValueTask<bool> ValidateFilePathAsync(string filePath)
        {
            bool filePathIsDirectory = false;
            try
            {
                if (!string.IsNullOrEmpty(filePath) && filePath.Contains(@".") && filePath.Contains(@"\"))
                {
                    await Task.Run(() =>
                    {
                        filePathIsDirectory = File.Exists(filePath);
                    });

                }
            }
            catch (Exception ex)
            {
                FileBrokerException fbException = new FileBrokerException(ex);
                _exceptions.AddException(fbException);
            }

            return filePathIsDirectory;
        }
    }
}
