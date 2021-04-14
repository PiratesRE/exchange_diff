using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	internal static class SqlClientFactory
	{
		internal static ISqlClientFactory Instance
		{
			get
			{
				return SqlClientFactory.hookableFactory.Value;
			}
		}

		public static ISqlCommand CreateSqlCommand()
		{
			return SqlClientFactory.Instance.CreateSqlCommand();
		}

		public static ISqlCommand CreateSqlCommand(SqlCommand command)
		{
			return SqlClientFactory.Instance.CreateSqlCommand(command);
		}

		public static ISqlCommand CreateSqlCommand(string commandText, ISqlConnection connection, ISqlTransaction transaction)
		{
			return SqlClientFactory.Instance.CreateSqlCommand(commandText, connection, transaction);
		}

		public static ISqlConnection CreateSqlConnection(SqlConnection connection)
		{
			return SqlClientFactory.Instance.CreateSqlConnection(connection);
		}

		public static ISqlConnection CreateSqlConnection(string connectionString)
		{
			return SqlClientFactory.Instance.CreateSqlConnection(connectionString);
		}

		public static ISqlDataReader CreateSqlDataReader(SqlDataReader reader)
		{
			return SqlClientFactory.Instance.CreateSqlDataReader(reader);
		}

		public static ISqlTransaction CreateSqlTransaction(SqlTransaction transaction)
		{
			return SqlClientFactory.Instance.CreateSqlTransaction(transaction);
		}

		internal static IDisposable SetTestHook(ISqlClientFactory factory)
		{
			return SqlClientFactory.hookableFactory.SetTestHook(factory);
		}

		private static Hookable<ISqlClientFactory> hookableFactory = Hookable<ISqlClientFactory>.Create(true, new SqlClientFactory.Factory());

		private class Factory : ISqlClientFactory
		{
			public ISqlCommand CreateSqlCommand()
			{
				return new SqlClientFactory.SqlCommandWrapper();
			}

			public ISqlCommand CreateSqlCommand(SqlCommand command)
			{
				if (command != null)
				{
					return new SqlClientFactory.SqlCommandWrapper(command);
				}
				return null;
			}

			public ISqlCommand CreateSqlCommand(string commandText, ISqlConnection connection, ISqlTransaction transaction)
			{
				return new SqlClientFactory.SqlCommandWrapper(commandText, connection, transaction);
			}

			public ISqlConnection CreateSqlConnection(SqlConnection connection)
			{
				if (connection != null)
				{
					return new SqlClientFactory.SqlConnectionWrapper(connection);
				}
				return null;
			}

			public ISqlConnection CreateSqlConnection(string connectionString)
			{
				return new SqlClientFactory.SqlConnectionWrapper(connectionString);
			}

			public ISqlDataReader CreateSqlDataReader(SqlDataReader reader)
			{
				if (reader != null)
				{
					return new SqlClientFactory.SqlDataReaderWrapper(reader);
				}
				return null;
			}

			public ISqlTransaction CreateSqlTransaction(SqlTransaction transaction)
			{
				if (transaction != null)
				{
					return new SqlClientFactory.SqlTransactionWrapper(transaction);
				}
				return null;
			}
		}

		private class SqlCommandWrapper : DisposableBase, ISqlCommand, IDisposable
		{
			public SqlCommandWrapper() : this(new SqlCommand())
			{
			}

			public SqlCommandWrapper(SqlCommand command)
			{
				this.command = command;
			}

			public SqlCommandWrapper(string commandText, ISqlConnection connection, ISqlTransaction transaction)
			{
				try
				{
					SqlConnection connection2 = (connection == null) ? null : connection.WrappedConnection;
					SqlTransaction transaction2 = (transaction == null) ? null : transaction.WrappedTransaction;
					this.command = new SqlCommand(commandText, connection2, transaction2);
				}
				finally
				{
					if (this.command == null)
					{
						base.Dispose();
					}
				}
			}

			public string CommandText
			{
				get
				{
					return this.command.CommandText;
				}
				set
				{
					this.command.CommandText = value;
				}
			}

			public int CommandTimeout
			{
				get
				{
					return this.command.CommandTimeout;
				}
				set
				{
					this.command.CommandTimeout = value;
				}
			}

			public CommandType CommandType
			{
				get
				{
					return this.command.CommandType;
				}
				set
				{
					this.command.CommandType = value;
				}
			}

			public ISqlConnection Connection
			{
				get
				{
					SqlConnection connection = this.command.Connection;
					if (connection == null)
					{
						return null;
					}
					if (this.activeConnection == null)
					{
						this.activeConnection = SqlClientFactory.CreateSqlConnection(connection);
					}
					else if (!object.ReferenceEquals(this.activeConnection.WrappedConnection, connection))
					{
						this.activeConnection.WrappedConnection.Dispose();
						this.activeConnection.WrappedConnection = connection;
					}
					return this.activeConnection;
				}
				set
				{
					this.command.Connection = ((value == null) ? null : value.WrappedConnection);
				}
			}

			public SqlParameterCollection Parameters
			{
				get
				{
					return this.command.Parameters;
				}
			}

			public ISqlTransaction Transaction
			{
				get
				{
					SqlTransaction transaction = this.command.Transaction;
					if (transaction == null)
					{
						return null;
					}
					if (this.activeTransaction == null)
					{
						this.activeTransaction = SqlClientFactory.CreateSqlTransaction(transaction);
					}
					else if (!object.ReferenceEquals(this.activeTransaction.WrappedTransaction, transaction))
					{
						this.activeTransaction.WrappedTransaction.Dispose();
						this.activeTransaction.WrappedTransaction = transaction;
					}
					return this.activeTransaction;
				}
				set
				{
					this.command.Transaction = ((value == null) ? null : value.WrappedTransaction);
				}
			}

			public override string ToString()
			{
				return this.command.ToString();
			}

			public override bool Equals(object other)
			{
				return this.command.Equals(other);
			}

			public override int GetHashCode()
			{
				return this.command.GetHashCode();
			}

			public int ExecuteNonQuery()
			{
				return this.command.ExecuteNonQuery();
			}

			public ISqlDataReader ExecuteReader()
			{
				return SqlClientFactory.CreateSqlDataReader(this.command.ExecuteReader());
			}

			public ISqlDataReader ExecuteReader(CommandBehavior behavior)
			{
				return SqlClientFactory.CreateSqlDataReader(this.command.ExecuteReader(behavior));
			}

			public object ExecuteScalar()
			{
				return this.command.ExecuteScalar();
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose)
				{
					if (this.command != null)
					{
						this.command.Dispose();
						this.command = null;
					}
					if (this.activeConnection != null)
					{
						this.activeConnection.Dispose();
						this.activeConnection = null;
					}
					if (this.activeTransaction != null)
					{
						this.activeTransaction.Dispose();
						this.activeTransaction = null;
					}
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<SqlClientFactory.SqlCommandWrapper>(this);
			}

			private SqlCommand command;

			private ISqlConnection activeConnection;

			private ISqlTransaction activeTransaction;
		}

		private class SqlConnectionWrapper : DisposableBase, ISqlConnection, IDisposable
		{
			public SqlConnectionWrapper(SqlConnection connection)
			{
				this.connection = connection;
			}

			public SqlConnectionWrapper(string connectionString)
			{
				try
				{
					this.connection = new SqlConnection(connectionString);
				}
				finally
				{
					if (this.connection == null)
					{
						base.Dispose();
					}
				}
			}

			public event SqlInfoMessageEventHandler InfoMessage
			{
				add
				{
					this.connection.InfoMessage += value;
				}
				remove
				{
					this.connection.InfoMessage -= value;
				}
			}

			public ConnectionState State
			{
				get
				{
					return this.connection.State;
				}
			}

			public SqlConnection WrappedConnection
			{
				get
				{
					return this.connection;
				}
				set
				{
					this.connection = value;
				}
			}

			public override string ToString()
			{
				return this.connection.ToString();
			}

			public override bool Equals(object other)
			{
				return this.connection.Equals(other);
			}

			public override int GetHashCode()
			{
				return this.connection.GetHashCode();
			}

			public ISqlTransaction BeginTransaction(IsolationLevel iso)
			{
				return SqlClientFactory.CreateSqlTransaction(this.connection.BeginTransaction(iso));
			}

			public void ClearPool()
			{
				SqlConnection.ClearPool(this.connection);
			}

			public void Close()
			{
				this.connection.Close();
			}

			public ISqlCommand CreateCommand()
			{
				return SqlClientFactory.CreateSqlCommand(this.connection.CreateCommand());
			}

			public void Open()
			{
				this.connection.Open();
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose && this.connection != null)
				{
					this.connection.Dispose();
					this.connection = null;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<SqlClientFactory.SqlConnectionWrapper>(this);
			}

			private SqlConnection connection;
		}

		private class SqlDataReaderWrapper : DisposableBase, ISqlDataReader, IDisposable
		{
			public SqlDataReaderWrapper(SqlDataReader reader)
			{
				this.reader = reader;
			}

			public int FieldCount
			{
				get
				{
					return this.reader.FieldCount;
				}
			}

			public bool IsClosed
			{
				get
				{
					return this.reader.IsClosed;
				}
			}

			public override string ToString()
			{
				return this.reader.ToString();
			}

			public override bool Equals(object other)
			{
				return this.reader.Equals(other);
			}

			public override int GetHashCode()
			{
				return this.reader.GetHashCode();
			}

			public void Close()
			{
				this.reader.Close();
			}

			public bool GetBoolean(int i)
			{
				return this.reader.GetBoolean(i);
			}

			public long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
			{
				return this.reader.GetBytes(i, dataIndex, buffer, bufferIndex, length);
			}

			public long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
			{
				return this.reader.GetChars(i, dataIndex, buffer, bufferIndex, length);
			}

			public DateTime GetDateTime(int i)
			{
				return this.reader.GetDateTime(i);
			}

			public Type GetFieldType(int i)
			{
				return this.reader.GetFieldType(i);
			}

			public Guid GetGuid(int i)
			{
				return this.reader.GetGuid(i);
			}

			public short GetInt16(int i)
			{
				return this.reader.GetInt16(i);
			}

			public int GetInt32(int i)
			{
				return this.reader.GetInt32(i);
			}

			public long GetInt64(int i)
			{
				return this.reader.GetInt64(i);
			}

			public string GetName(int i)
			{
				return this.reader.GetName(i);
			}

			public int GetOrdinal(string name)
			{
				return this.reader.GetOrdinal(name);
			}

			public SqlBinary GetSqlBinary(int i)
			{
				return this.reader.GetSqlBinary(i);
			}

			public string GetString(int i)
			{
				return this.reader.GetString(i);
			}

			public object GetValue(int i)
			{
				return this.reader.GetValue(i);
			}

			public bool IsDBNull(int i)
			{
				return this.reader.IsDBNull(i);
			}

			public bool NextResult()
			{
				return this.reader.NextResult();
			}

			public bool Read()
			{
				return this.reader.Read();
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose)
				{
					this.reader.Dispose();
					this.reader = null;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<SqlClientFactory.SqlDataReaderWrapper>(this);
			}

			private SqlDataReader reader;
		}

		private class SqlTransactionWrapper : DisposableBase, ISqlTransaction, IDisposable
		{
			public SqlTransactionWrapper(SqlTransaction transaction)
			{
				this.transaction = transaction;
			}

			public SqlTransaction WrappedTransaction
			{
				get
				{
					return this.transaction;
				}
				set
				{
					this.transaction = value;
				}
			}

			public override string ToString()
			{
				return this.transaction.ToString();
			}

			public override bool Equals(object other)
			{
				return this.transaction.Equals(other);
			}

			public override int GetHashCode()
			{
				return this.transaction.GetHashCode();
			}

			public void Commit()
			{
				this.transaction.Commit();
			}

			public void Rollback()
			{
				this.transaction.Rollback();
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose)
				{
					this.transaction.Dispose();
					this.transaction = null;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<SqlClientFactory.SqlTransactionWrapper>(this);
			}

			private SqlTransaction transaction;
		}
	}
}
