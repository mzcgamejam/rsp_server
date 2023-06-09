//Use package Nuget MySqlConnector instead of MySql.Data
//https://stackoverflow.com/questions/50137205/androidapp-and-mysqlconnection-didnt-work-connection-open/60786792#60786792
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace GameDB
{
    public class DBConnector : IDisposable
    {
        private uint _userId;
        MySqlConnection connection;
        MySqlTransaction transaction;
        public MySqlCommand Command { get; private set; }
        public DbDataReader Cursor { get; private set; }
        bool isTransactionCompleted = false;


        string _transactionRollbackStack = "";
        Exception _transactionRollbackException;
        string _transactionRollbackLastSql = "";


        public DBConnector()
        {
            Connect(0, "Accounts");
        }

        public DBConnector(string dbVariety)
        {
            Connect(0, dbVariety);
        }

        public DBConnector(uint userId = 0, string dbVariety = "UserDBSetting")
        {
            _userId = userId;
            Connect(userId, dbVariety);
        }

        private void Connect(uint userId, string dbVariety = "UserDBSetting")
        {
            try
            {
                var shardNum = 0;
                connection = new MySqlConnection(
                    DBEnv.SettingBuilderMap[dbVariety]
                    [shardNum].ConnectionString);
                connection.Open();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public async Task<DbDataReader> ExecuteReaderAsync(string text, params object[] args)
        {
            CloseCommand();

            Command = new MySqlCommand(String.Format(text, args), connection, transaction);
            _transactionRollbackLastSql = Command.CommandText;

            Cursor = await Command.ExecuteReaderAsync();

            return Cursor;
        }

        public async Task<int> ExecuteNonQueryAsync(string text, params object[] args)
        {
            CloseCommand();

            Command = new MySqlCommand(String.Format(text, args), connection, transaction);
            _transactionRollbackLastSql = Command.CommandText;
            return await Command.ExecuteNonQueryAsync();
        }

        public long LastInsertedId()
        {
            return Command.LastInsertedId;
        }

        public async Task<int> ExecuteNonQueryAsync(string text, MySqlParameter[] sqlParam, params object[] args)
        {
            CloseCommand();

            Command = new MySqlCommand(String.Format(text, args), connection, transaction);

            if ((sqlParam?.Length ?? 0) != 0)
                Command.Parameters.AddRange(sqlParam);
            _transactionRollbackLastSql = Command.CommandText;
            return await Command.ExecuteNonQueryAsync();
        }

        public async Task<bool> BeginTransactionCallback(Func<Task<bool>> onTransactionBegin)
        {
            if (transaction != null)
            {
                //To do : Log
                //throw new UserWebException("already transaction is declared, it is not duplicated", WebErrorType.LogicError, WebSubErrorType.InvalidUsage);
            }

            if (Command != null && Cursor == null)
            {
                //To do : Log
                //throw new UserWebException("before call of BeginTransactionCallback, insert or update statement is executed, it is not invalid usage", WebErrorType.LogicError, WebSubErrorType.InvalidUsage);
            }//Update, Insert statement

            CloseCommand();

            transaction = await connection.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            try
            {
                isTransactionCompleted = await onTransactionBegin();
            }
            catch (Exception ex)
            {
                _transactionRollbackException = ex;
                _transactionRollbackStack = Environment.StackTrace;
                throw ex;
            }

            if (isTransactionCompleted == false)
            {
                _transactionRollbackStack = Environment.StackTrace;
            }
            return isTransactionCompleted;
        }

        public async Task<DbDataReader> CallStoredProcedureAsync(string name, DBParamInfo paramInfo, bool isTransactionEnable = false)
        {
            bool isInnerTransaction = false;

            try
            {
                if (isTransactionEnable)
                {
                    if (transaction != null)
                    {
                        //To do : Log
                        //throw new UserWebException("already transaction is declared, it is not duplicated", WebErrorType.LogicError, WebSubErrorType.InvalidUsage);
                    }

                    transaction = await connection.BeginTransactionAsync(IsolationLevel.RepeatableRead);
                    isInnerTransaction = true;
                }

                CloseCommand();

                Command = new MySqlCommand(name, connection);
                Command.CommandType = CommandType.StoredProcedure;
                Command.Transaction = transaction;

                _transactionRollbackLastSql = name;

                if (paramInfo.InputArgs != null)
                {
                    foreach (var arg in paramInfo.InputArgs)
                    {
                        Command.Parameters.AddWithValue(arg.Key, arg.Value);
                        Command.Parameters[arg.Key].Direction = ParameterDirection.Input;
                    }
                }

                if (paramInfo.OutputArgs != null)
                {
                    foreach (var arg in paramInfo.OutputArgs)
                    {
                        Command.Parameters.Add(arg.Key, arg.ValueType);
                        Command.Parameters[arg.Key].Direction = ParameterDirection.Output;
                    }
                }

                Cursor = await Command.ExecuteReaderAsync();

                paramInfo.Sender = this;
                //paramInfo.ResultType = ResultType.Success;

                if (isInnerTransaction)
                {
                    isTransactionCompleted = true;
                }

                return Cursor;

            }
            catch (MySqlException e)
            {
                _transactionRollbackException = e;
                _transactionRollbackStack = Environment.StackTrace;
                throw e;
            }
        }

        public void Commit()
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
        }

        public void Dispose()
        {
            CloseCommand();

            if (transaction != null)
            {
                if (isTransactionCompleted)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }

                transaction.Dispose();
                transaction = null;
            }

            connection.Dispose();
        }

        void CloseCommand()
        {
            if (Cursor != null)
            {
                if (!Cursor.IsClosed)
                {
                    Cursor.Close();
                    Cursor = null;
                }
            }

            Command?.Dispose();
            Command = null;
        }
    }
}
