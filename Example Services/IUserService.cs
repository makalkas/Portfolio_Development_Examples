namespace AmetekLabelPrinterApplication.Resources.Services
{
    public interface IUserService
    {
        string CurrentUser { get; }
        bool IsLoggedIn { get; }
    }
}