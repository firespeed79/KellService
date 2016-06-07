using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Configuration;
using System.ServiceModel;

namespace ServiceLib.DAL
{
    /// <summary>
    /// MSSQL数据库实现类
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession)]
    public class MSSQLDataBase : IDataBase
    {
        private SqlConnection _cnx;
        private SqlTransaction _trans;

        public MSSQLDataBase()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            if (!string.IsNullOrEmpty(connStr))
                Connect(connStr);
        }

        public MSSQLDataBase(string connStr)
        {
            Connect(connStr);
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        public bool Connect(string connStr)
        {
            try
            {
                _cnx = new SqlConnection(connStr);
                _cnx.Open();
                bool flag = _cnx.State == ConnectionState.Open;
                return flag;
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:Connect\n" + e.Message);              
            }
        }

        /// <summary>
        /// 查看当前的连接状态
        /// </summary>
        public bool IsConnected
        {
            get
            {
                bool flag = false;
                if (_cnx != null)
                    flag = _cnx.State == ConnectionState.Open;
                return flag;
            }
        }

        /// <summary>
        /// 执行查询，返回数据阅读器SqlDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="behavior">默认为CloseConnection</param>
        /// <returns></returns>
        public SqlDataReader GetReader(string sql, CommandBehavior behavior = CommandBehavior.CloseConnection)
        {
            SqlCommand cmd = null;
            try
            {
                if (_trans != null) cmd = new SqlCommand(sql, _cnx, _trans);
                else cmd = new SqlCommand(sql, _cnx);
                SqlDataReader reader = cmd.ExecuteReader(behavior);
                return reader;
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:GetReader\n" + e.Message);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        /// <summary>
        /// 执行查询，返回影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public int ExecQuery(string sql)
        {
            SqlCommand cmd = null;
            try
            {
                if (_trans != null) cmd = new SqlCommand(sql, _cnx, _trans);
                else cmd = new SqlCommand(sql, _cnx);
                int RecAffected = cmd.ExecuteNonQuery();
                return RecAffected;
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:ExecQuery\n" + e.Message);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        /// <summary>
        /// 执行查询，返回首行首列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public object ExecQueryWithReturn(string sql)
        {
            SqlCommand cmd = null;
            try
            {
                object obj = null;
                if (_trans != null) cmd = new SqlCommand(sql, _cnx, _trans);
                else cmd = new SqlCommand(sql, _cnx);
                object o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                    obj = o;
                return obj;
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:ExecQueryWithReturn\n" + e.Message);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql)
        {
            SqlDataAdapter Adp = new SqlDataAdapter(sql, _cnx);
            try
            {
                DataSet DSet = new DataSet();
                Adp.Fill(DSet);
                return DSet;
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:GetDataSet\n" + e.Message);
            }
            finally
            {
                Adp.Dispose();
            }
        }
        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, string tablename)
        {
            SqlDataAdapter Adp = new SqlDataAdapter(sql, _cnx);
            try
            {
                DataTable DT = new DataTable(tablename);
                Adp.Fill(DT);
                return DT;
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:GetDataTable\n" + e.Message);
            }
            finally
            {
                Adp.Dispose();
            }
        }
        /// <summary>
        /// 填充指定的数据表
        /// </summary>
        /// <param name="startRecord"></param>
        /// <param name="maxRecords"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int FillTables(string sql, int startRecord, int maxRecords, params DataTable[] tables)
        {
            SqlDataAdapter Adp = new SqlDataAdapter(sql, _cnx);
            try
            {
                int r = Adp.Fill(0, 10, tables);
                return r;
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:GetDataSet\n" + e.Message);
            }
            finally
            {
                Adp.Dispose();
            }
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="level">默认为ReadUncommitted</param>
        public void BeginTrans(IsolationLevel level = IsolationLevel.ReadUncommitted)
        {
            try
            {
                _trans = _cnx.BeginTransaction(level);
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:BeginTrans\n" + e.Message);
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTrans()
        {
            try
            {
                if (_trans != null)
                {
                    _trans.Commit();
                }
            }
            catch (Exception e) 
            {
                throw new Exception("Class:MSSQLDataBase, Method:CommitTrans\n" + e.Message); 
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBackTrans()
        {
            try
            {
                if (_trans != null)
                {
                    _trans.Rollback();
                }
            }
            catch(Exception e) 
            {
                throw new Exception("Class:MSSQLDataBase, Method:RollBackTrans\n" + e.Message);
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_cnx != null)
                {
                    if (_cnx.State != ConnectionState.Closed)
                        _cnx.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Class:MSSQLDataBase, Method:Close\n" + e.Message);
            }
            finally
            {
                if (_cnx != null)
                {
                    _cnx.Dispose();
                }
            }
        }            
        /// <summary>
        /// 释放数据库连接以及所有资源
        /// </summary>
        public void Dispose()
        {
            this.Disconnect();
        }
    }
}
