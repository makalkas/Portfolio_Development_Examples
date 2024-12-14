//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------

namespace AmetekLabelPrinterApplication.Resources.Services
{
    public interface IExceptionService
    {
        List<Exception> ListOfExceptions { get; }

        void AddException(Exception ex);
        IEnumerable<Exception> AppExceptionsCollection();
        void Clear();
        int Count();
        List<Exception> GetListOfExceptions();
        void Remove(Exception exception);
    }
}