using AmetekLabelAPI.Resources.Exceptions;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public partial class SQLBroker
    {
        private string ValidateConnectionString(string connectionstringName, string connectionString)
        {
            // This method should trhow up an exception if the connection string name is null or blank and
            // it should throw an exception if the connection string is malformed.


            if (string.IsNullOrEmpty(connectionstringName) & string.IsNullOrEmpty(connectionString))
            {
                throw new NullOrEmptyConnectionStringException("Both connectionstringName and connectionString are null");
            }
            else if (!string.IsNullOrEmpty(connectionString))
            {
                if (!connectionString.Contains("Source=") || !connectionString.Contains("Catalog="))
                {
                    throw new ConnectionStringFormatException();
                }
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = string.Empty;
            }
            return connectionString!;
        }
    }
}
