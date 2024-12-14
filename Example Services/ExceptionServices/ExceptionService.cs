//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
namespace AmetekLabelPrinterApplication.Resources.Services
{
    public class ExceptionService : IExceptionService
    {
        #region Declarations
        private List<Exception> _exceptions = new List<Exception>();

        #endregion Declarations
        #region Public Properties
        public List<Exception> ListOfExceptions => _exceptions;
        #endregion Public Properties

        #region Private Properties
        private List<Exception> ExceptionsList
        {
            get
            {
                if (_exceptions == null)
                {
                    _exceptions = new List<Exception>();
                }
                return _exceptions;
            }
            set { _exceptions = value; }
        }
        #endregion Private Properties
        #region Constructors
        public ExceptionService()
        {

        }
        #endregion Constructors
        #region Public Methods
        public void AddException(Exception ex)
        {
            ExceptionsList.Add(ex);
        }

        public int Count() => ExceptionsList.Count;

        public void Clear() => ExceptionsList.Clear();

        public void Remove(Exception exception) => ExceptionsList.Remove(exception);

        public List<Exception> GetListOfExceptions() => ExceptionsList;


        public System.Collections.Generic.IEnumerable<Exception> AppExceptionsCollection()
        {
            if (_exceptions.Count > 0)
            {
                foreach (Exception e in _exceptions)
                {
                    yield return e;
                }
            }

        }
        #endregion Public Methods
        #region Private Methods

        #endregion Private Methods
    }
}
