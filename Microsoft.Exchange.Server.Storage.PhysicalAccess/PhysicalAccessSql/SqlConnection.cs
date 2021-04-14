using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	internal sealed class SqlConnection : Connection
	{
		public SqlConnection(SqlDatabase database, string identification) : this(null, database, identification)
		{
		}

		public SqlConnection(IDatabaseExecutionContext outerExecutionContext, SqlDatabase database, string identification) : base(outerExecutionContext, database, identification)
		{
			string text = (database == null) ? DatabaseLocation.GetMasterConnectionString() : database.ConnectionString;
			try
			{
				this.sqlConnection = SqlClientFactory.CreateSqlConnection(text);
				base.IsValid = true;
			}
			catch (SqlException ex)
			{
				base.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("connection", text, ex);
				throw this.ProcessSqlError(ex);
			}
			finally
			{
				if (!base.IsValid)
				{
					base.Dispose();
				}
			}
		}

		public override bool TransactionStarted
		{
			get
			{
				return this.transaction != null;
			}
		}

		public override int TransactionId
		{
			get
			{
				if (this.transaction != null)
				{
					return this.transaction.GetHashCode();
				}
				return 0;
			}
		}

		public bool HasDeadlocked
		{
			set
			{
				this.hasDeadlocked = true;
			}
		}

		public override void FlushDatabaseLogs(bool force)
		{
		}

		internal static void LogSQLError(string operation, string sqlCommandText, Exception e)
		{
			string message = string.Format("{0}:Exception. Message:[{1}] Stack:[{2}] Command:[{3}]", new object[]
			{
				operation,
				e.Message,
				e.StackTrace,
				sqlCommandText
			});
			Microsoft.Exchange.Server.Storage.Common.Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_SQLExceptionDetected, new object[]
			{
				e.Message,
				e.StackTrace,
				sqlCommandText
			});
			ExTraceGlobals.DbInteractionSummaryTracer.TraceError(0L, message);
		}

		internal Reader ExecuteReader(ISqlCommand command, int numberOfStatements, Connection.TransactionOption transactionOption, int skipTo, SimpleQueryOperator simpleQueryOperator, bool disposeQueryOperator)
		{
			Reader result;
			try
			{
				command = this.BuildCommand(command, numberOfStatements, transactionOption, SqlConnection.SqlOperationType.ExecuteReader);
				using (base.TrackTimeInDatabase())
				{
					result = new SqlReader(command.ExecuteReader(), this, skipTo, simpleQueryOperator, disposeQueryOperator);
				}
			}
			catch (SqlException ex)
			{
				base.OnExceptionCatch(ex);
				SqlConnection.LogSQLError("ExecuteReader", command.CommandText, ex);
				throw this.ProcessSqlError(ex);
			}
			return result;
		}

		internal int ExecuteNonQuery(ISqlCommand command, int numberOfStatements, Connection.TransactionOption transactionOption)
		{
			int result;
			try
			{
				command = this.BuildCommand(command, numberOfStatements, transactionOption, SqlConnection.SqlOperationType.ExecuteNonQuery);
				int num;
				using (base.TrackTimeInDatabase())
				{
					num = command.ExecuteNonQuery();
				}
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(25);
					stringBuilder.Append("Rows affected: [");
					stringBuilder.Append(num);
					stringBuilder.Append("]");
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
				}
				result = num;
			}
			catch (SqlException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				SqlConnection.LogSQLError("ExecuteNonQuery", command.CommandText, ex);
				throw this.ProcessSqlError(ex);
			}
			return result;
		}

		internal object ExecuteScalar(ISqlCommand command, int numberOfStatements, Connection.TransactionOption transactionOption)
		{
			object obj = null;
			try
			{
				command = this.BuildCommand(command, numberOfStatements, transactionOption, SqlConnection.SqlOperationType.ExecuteScalar);
				if (ExTraceGlobals.DbIOTracer.IsTraceEnabled(TraceType.PerformanceTrace))
				{
					using (base.TrackTimeInDatabase())
					{
						using (SqlReader sqlReader = new SqlReader(command.ExecuteReader(), this, 0, null, false))
						{
							if (!sqlReader.Read())
							{
								obj = null;
							}
							else
							{
								obj = sqlReader.GetValueByOrdinal(0);
							}
						}
						goto IL_91;
					}
				}
				using (base.TrackTimeInDatabase())
				{
					obj = command.ExecuteScalar();
				}
				if (obj is DBNull || obj == null)
				{
					obj = null;
				}
				IL_91:;
			}
			catch (SqlException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				SqlConnection.LogSQLError("ExecuteScalar", command.CommandText, ex);
				throw this.ProcessSqlError(ex);
			}
			if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("Read Scalar: value:[");
				stringBuilder.AppendAsString(obj);
				stringBuilder.Append("]");
				ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return obj;
		}

		internal void ClearPool()
		{
			this.sqlConnection.ClearPool();
		}

		internal ISqlCommand CreateSqlCommand()
		{
			ISqlCommand sqlCommand = this.sqlConnection.CreateCommand();
			sqlCommand.CommandTimeout = 6000;
			return sqlCommand;
		}

		internal Exception ProcessSqlError(SqlException e)
		{
			Exception ex = e;
			int number = e.Number;
			bool flag;
			string message;
			if (number <= 1205)
			{
				if (number != 208)
				{
					if (number != 1205)
					{
						goto IL_7C;
					}
					flag = false;
					message = "Database deadlock";
					this.HasDeadlocked = true;
					goto IL_84;
				}
			}
			else
			{
				if (number == 1222)
				{
					flag = false;
					message = "Database lock timeout";
					goto IL_84;
				}
				if (number != 20507 && number != 25819)
				{
					goto IL_7C;
				}
			}
			flag = true;
			message = "Database schema broken";
			ex = new DatabaseSchemaBroken(base.Database.DisplayName, e.Message, e);
			goto IL_84;
			IL_7C:
			flag = true;
			message = string.Empty;
			IL_84:
			ExTraceGlobals.DbInteractionDetailTracer.TraceError<string, string>(0L, "SQLException. Message is: {0} Stack trace is: {1}", ex.Message, ex.StackTrace);
			if (flag)
			{
				return ex;
			}
			return new NonFatalDatabaseException(message);
		}

		protected override void OnCommit(byte[] logTransactionInformation)
		{
			try
			{
				if (this.transaction == null)
				{
					if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Transaction was not started - commit is a no-op");
					}
				}
				else if (!this.hasDeadlocked)
				{
					try
					{
						using (base.TrackTimeInDatabase())
						{
							this.transaction.Commit();
						}
					}
					catch (SqlException ex)
					{
						base.OnExceptionCatch(ex);
						SqlConnection.LogSQLError("Flush", "commit", ex);
						throw this.ProcessSqlError(ex);
					}
				}
			}
			finally
			{
				if (this.transaction != null)
				{
					this.transaction.Dispose();
					this.transaction = null;
				}
			}
		}

		protected override void OnAbort(byte[] logTransactionInformation)
		{
			if (this.transaction != null)
			{
				try
				{
					if (!this.hasDeadlocked && this.transaction != null)
					{
						try
						{
							using (base.TrackTimeInDatabase())
							{
								this.transaction.Rollback();
							}
						}
						catch (SqlException ex)
						{
							base.OnExceptionCatch(ex);
							base.IsValid = false;
							SqlConnection.LogSQLError("Abort", "Rollback", ex);
							throw this.ProcessSqlError(ex);
						}
					}
				}
				finally
				{
					this.transaction.Dispose();
					this.transaction = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlConnection>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.sqlConnection != null && this.transaction != null && !this.hasDeadlocked)
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Connection disposed without being committed - changes lost");
					try
					{
						base.Abort();
					}
					catch (Exception ex)
					{
						base.OnExceptionCatch(ex);
						SqlConnection.LogSQLError("Dispose", "Rollback", ex);
					}
				}
				this.Close();
				base.IsValid = false;
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("cn:[");
					stringBuilder.Append(this.GetHashCode());
					stringBuilder.Append("] ");
					stringBuilder.Append("Connection Disposed");
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
				}
			}
		}

		protected override void Close()
		{
			if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("cn:[");
				stringBuilder.Append(this.GetHashCode());
				stringBuilder.Append("] ");
				stringBuilder.Append("Connection Closed");
				ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			if (this.transaction != null)
			{
				this.transaction.Dispose();
				this.transaction = null;
			}
			if (this.sqlConnection != null)
			{
				if (this.sqlConnection.State != ConnectionState.Closed)
				{
					this.sqlConnection.Close();
				}
				this.sqlConnection.Dispose();
				this.sqlConnection = null;
			}
			if (ExTraceGlobals.DbIOTracer.IsTraceEnabled(TraceType.PerformanceTrace) && this.ioStats != null)
			{
				foreach (TableLevelIOStats tableLevelIOStats in this.ioStats.IOStats.Values)
				{
					ExTraceGlobals.DbIOTracer.TraceDebug(0L, "Stats for table {0}. PhysicalReads:{1} LogicalReads:{2} ReadAhead:{3} LobPhysicalReads:{4} LobLogicalReads:{5} LobReadAhead:{6}", new object[]
					{
						tableLevelIOStats.TableName,
						tableLevelIOStats.PhysicalReads,
						tableLevelIOStats.LogicalReads,
						tableLevelIOStats.ReadAheads,
						tableLevelIOStats.LobPhysicalReads,
						tableLevelIOStats.LobLogicalReads,
						tableLevelIOStats.LobReadAheads
					});
				}
			}
			this.ioStats = null;
			base.Close();
			base.OwningThread = null;
		}

		private void SetupToCollectIO()
		{
			if (ExTraceGlobals.DbIOTracer.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				if (this.ioStats == null)
				{
					if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Connection: Set Statistics IO ON");
					}
					using (ISqlCommand sqlCommand = SqlClientFactory.CreateSqlCommand())
					{
						sqlCommand.CommandText = "set statistics IO on";
						sqlCommand.Connection = this.sqlConnection;
						sqlCommand.CommandType = CommandType.Text;
						sqlCommand.Transaction = this.transaction;
						sqlCommand.ExecuteNonQuery();
					}
					this.ioStats = new IOStatistics();
					this.sqlConnection.InfoMessage += this.InfoMessage;
					return;
				}
			}
			else if (this.ioStats != null)
			{
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Connection: Set Statistics IO OFF");
				}
				using (ISqlCommand sqlCommand2 = SqlClientFactory.CreateSqlCommand())
				{
					sqlCommand2.CommandText = "set statistics IO off";
					sqlCommand2.Connection = this.sqlConnection;
					sqlCommand2.CommandType = CommandType.Text;
					sqlCommand2.Transaction = this.transaction;
					sqlCommand2.ExecuteNonQuery();
				}
				this.ioStats = null;
				this.sqlConnection.InfoMessage -= this.InfoMessage;
			}
		}

		private void Open()
		{
			if (this.sqlConnection.State != ConnectionState.Open)
			{
				this.sqlConnection.Open();
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("cn:[");
					stringBuilder.Append(this.GetHashCode());
					stringBuilder.Append("] ");
					stringBuilder.Append("Connection Opened");
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, stringBuilder.ToString());
				}
				base.OwningThread = Thread.CurrentThread;
				base.IsValid = true;
				this.hasDeadlocked = false;
			}
		}

		private void BeginTransaction()
		{
			try
			{
				this.transactionTimeStamp = Interlocked.Increment(ref Connection.currentTransactionTimeStamp);
				this.transaction = this.sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);
			}
			catch (SqlException ex)
			{
				base.OnExceptionCatch(ex);
				base.IsValid = false;
				SqlConnection.LogSQLError("BeginTransaction", "BeginTransaction", ex);
				throw this.ProcessSqlError(ex);
			}
			if (ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					stringBuilder.Append("cn:[");
					stringBuilder.Append(this.GetHashCode());
					stringBuilder.Append("] ");
				}
				stringBuilder.Append("Begin Transaction");
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		internal override void BeginTransactionIfNeeded(Connection.TransactionOption transactionOption)
		{
			this.Open();
			if (this.transaction == null && transactionOption == Connection.TransactionOption.NeedTransaction)
			{
				this.BeginTransaction();
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Connection Created");
				}
			}
		}

		private void InfoMessage(object sender, SqlInfoMessageEventArgs e)
		{
			for (int i = 0; i < e.Errors.Count; i++)
			{
				if (ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.DbInteractionDetailTracer.TraceDebug(0L, "Connection: InfoMessage:" + e.Errors[i].Message);
				}
				if (e.Errors[i].Number == 3615)
				{
					this.ioStats.AddStats(e.Errors[i].Message);
				}
			}
		}

		private ISqlCommand BuildCommand(ISqlCommand command, int numberOfStatements, Connection.TransactionOption transactionOption, SqlConnection.SqlOperationType sqlOperationType)
		{
			this.BeginTransactionIfNeeded(transactionOption);
			this.SetupToCollectIO();
			command.Transaction = this.transaction;
			this.TraceSQLBatch(sqlOperationType, command);
			return command;
		}

		private void TraceSQLBatch(SqlConnection.SqlOperationType sqlOperationType, ISqlCommand command)
		{
			if (ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("Connection.");
				stringBuilder.Append(sqlOperationType);
				stringBuilder.Append(": Command:[");
				stringBuilder.Append(command.CommandText);
				stringBuilder.Append("]");
				foreach (object obj in command.Parameters)
				{
					SqlParameter sqlParameter = (SqlParameter)obj;
					stringBuilder.Append(" Param:[id=[");
					stringBuilder.Append(sqlParameter.ToString());
					stringBuilder.Append("]type=[");
					stringBuilder.Append(sqlParameter.SqlDbType.ToString());
					stringBuilder.Append("]value=[");
					if (sqlParameter.Value is byte[])
					{
						stringBuilder.AppendAsString((byte[])sqlParameter.Value);
					}
					else
					{
						stringBuilder.Append(sqlParameter.Value.ToString());
					}
					stringBuilder.Append("]]");
				}
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		public const int MaxTimeout = 60000;

		private const int CommandTimeout = 6000;

		private ISqlConnection sqlConnection;

		private IOStatistics ioStats;

		private ISqlTransaction transaction;

		private bool hasDeadlocked;

		private enum SqlOperationType
		{
			ExecuteReader = 1,
			ExecuteScalar,
			ExecuteNonQuery
		}
	}
}
