using AmetekLabelPrinterApplication.Resources.Data.Exceptions;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public partial class FileBroker
    {
        private delegate ValueTask<bool> ReturningFileNameFunction();
        private async ValueTask<bool> TryCatch(ReturningFileNameFunction returningFileNameFunction)
        {
            try
            {
                return await returningFileNameFunction();
            }
            catch (BadFilePathException badFilePathException)
            {

                throw CreatAndLogValidationException(badFilePathException);
            }
        }

        private FileAccessException CreatAndLogValidationException(Exception badFilePathException)
        {
            var fileAccessException = new FileAccessException(badFilePathException);
            this._logger.LogError(fileAccessException);
            this._exceptions.AddException(fileAccessException);

            return fileAccessException;
        }
    }
}
