//--------------------------------------------------------------------------------
//Copyright (c) Ametek LLC. 2024
//For Company use only. Written by Michael Kalkas
//--------------------------------------------------------------------------------
namespace AmetekLabelPrinterApplication.Resources.Brokers
{
    public interface ISQLBroker
    {
        string ConnectionString { get; set; }
        string ConnectionStringName { get; set; }

        ValueTask<List<T>> LoadListData<T, U>(string sql, U parameters);
    }
}