using AmetekLabelPrinterApplication.Resources.Data.Exceptions;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public partial class TemplateBroker
    {
        private bool TemplatePathIsValid(string templatePathAndName, string fileExtension)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(templatePathAndName))
                    throw new BadFilePathException($"The string containing the path and name is null or empty:{templatePathAndName}");
                if (!templatePathAndName.StartsWith(@"C:\"))
                    throw new BadFilePathException($"The string containing the path and name is missing drive information:{templatePathAndName}");
                if ((!string.IsNullOrEmpty(fileExtension) && !templatePathAndName.EndsWith(fileExtension)) || !templatePathAndName.Contains(@"."))
                    throw new BadFilePathException($"The string containing the path and name is missing the file extension information:{templatePathAndName}");
                if (!templatePathAndName.Contains(@"\"))
                    throw new BadFilePathException($"The string containing the path and name appears to be missing the path information:{templatePathAndName}");
                if (!File.Exists(templatePathAndName))
                    throw new BadFilePathException($"The file at:{templatePathAndName} could not be found. This may be because of permissions or bad path and name issues.");

                result = true;
            }
            catch (Exception ex)
            {
                string message = "The string containing the template path and name threw an exception in the Template Broker Class. Pleas see the Inner Exception for details.";
                _exceptions.AddException(new TemplateBrokerException(message, ex));
            }

            return result;
        }
    }
}
