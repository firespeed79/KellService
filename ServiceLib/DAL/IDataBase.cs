using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ServiceLib.DAL
{
    [ServiceContract]
    [ServiceKnownType(typeof(bool))]
    [ServiceKnownType(typeof(int))]
    [ServiceKnownType(typeof(object))]
    [ServiceKnownType(typeof(DataSet))]
    [ServiceKnownType(typeof(DataTable))]
    public interface IDataBase : IDisposable
    {
        [DataMember]
        bool IsConnected { get; }

        [OperationContract]
        bool Connect(string connStr);

        [OperationContract]
        SqlDataReader GetReader(string sql, CommandBehavior behavior);

        [OperationContract]
        int ExecQuery(string sql);

        [OperationContract]
        object ExecQueryWithReturn(string sql);

        [OperationContract]
        void BeginTrans(IsolationLevel level);

        [OperationContract]
        DataSet GetDataSet(string sql);

        [OperationContract]
        int FillTables(string sql, int startRecord, int maxRecords, params DataTable[] tables);

        [OperationContract]
        DataTable GetDataTable(string sql, string tablename);

        [OperationContract]
        void CommitTrans();

        [OperationContract]
        void RollBackTrans();

        [OperationContract]
        void Disconnect();
    }
}
