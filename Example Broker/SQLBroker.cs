//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
using AmetekLabelPrinterApplication.Resources.Brokers.Logging;
using AmetekLabelPrinterApplication.Resources.Data.Exceptions;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public partial class SQLBroker : ISQLBroker
    {
        #region Declarations

        private readonly IConfiguration _config;
        private readonly ILoggingBroker _logger;
        private string _connectionStringName = "Default";

        /// <summary>
        /// Name used to retrieve the connection string information. 
        /// </summary>
        public string ConnectionStringName
        {
            get { return _connectionStringName; }
            set
            {
                _connectionStringName = value;
                if (!string.IsNullOrEmpty(_connectionStringName) & _config.GetConnectionString(value) != null)
                {
                    this.ConnectionString = _config.GetConnectionString(value)!;
                }
            }
        }


        /// <summary>
        /// Actual connection string information if not included in setup.
        /// </summary>
        public string ConnectionString { get; set; } = "";
        #endregion Declarations

        #region Constructors
        /// <summary>
        /// Default constructor that requires a configuration object and a logging object.
        /// </summary>
        /// <param name="config">configuration object of type IConfiguration</param>
        /// <param name="Logger">logging object of type ILoggingBroker</param>        
        public SQLBroker(
            IConfiguration config,
            ILoggingBroker Logger)
        {
            this._config = config;
            this._logger = Logger;
            this.ConnectionString = config.GetValue<string>("ConnectionStrings:Default") ?? string.Empty;
        }

        #endregion Constructors

        #region public methods

        /// <summary>
        /// Method for returning data from SQL Data Table.
        /// </summary>
        /// <typeparam name="T">Model data type for returning a list of data</typeparam>
        /// <typeparam name="U">Parameters object defined at run time</typeparam>
        /// <param name="sql">SQL Code to be exicuted.</param>
        /// <param name="parameters">Parameters object to cusomize queried information to SQL command.</param>
        /// <returns>ValueTask with List of data objects of type specified on input.</returns>
        /// <exception cref="NullReferenceException"></exception>
        public async ValueTask<List<T>> LoadListData<T, U>(string sql, U parameters)
        {
            string connectionString = TryCatch(() =>
            {
                return ValidateConnectionString(ConnectionStringName, this.ConnectionString);
            });

            if ((string.IsNullOrEmpty(this.ConnectionString) & !string.IsNullOrEmpty(ConnectionStringName)) && _config.GetConnectionString(ConnectionStringName) != null)
            {
                this.ConnectionString = _config.GetConnectionString(ConnectionString)!;
            }

            try
            {
                using (IDbConnection connection = new SqlConnection(this.ConnectionString))
                {
                    var data = await connection.QueryAsync<T>(sql, parameters);

                    return data.ToList();
                }
            }
            catch (Exception ex)
            {

                throw new SQLBrokerSQLException(ex);
            }
        }

        #endregion public methods

        #region Private methods

        #endregion Private methods
    }
}
