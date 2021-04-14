using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ReportingWebService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.ReportingTask.Query;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class ReportProvider<TReportObject> where TReportObject : ReportObject
	{
		public ReportProvider(ITaskContext taskContext, IReportContextFactory reportContextFactory)
		{
			this.queryDecorators = new List<QueryDecorator<TReportObject>>();
			this.TaskContext = taskContext;
			this.reportContextFactory = reportContextFactory;
			this.expressionDecorator = new ExpressionDecorator<TReportObject>(this.TaskContext);
			this.AddQueryDecorator(this.expressionDecorator);
		}

		public event Action<TReportObject> ReportReceived;

		public event Action<string, string> StatementLogged;

		public Expression Expression
		{
			get
			{
				return this.expressionDecorator.Expression;
			}
			set
			{
				this.expressionDecorator.Expression = value;
			}
		}

		public bool IsExpressionEnforced
		{
			get
			{
				return this.Expression != null;
			}
		}

		private protected ITaskContext TaskContext { protected get; private set; }

		protected IDbConnection OpenConnections(IDbConnection connection, IDbConnection connectionBackup)
		{
			try
			{
				this.LogSqlConnection(connection);
				connection.Open();
				return connection;
			}
			catch (SqlException sqlException)
			{
				if (connectionBackup != null)
				{
					try
					{
						this.HandleSqlConnectionFailOverWarning(connection, sqlException, connectionBackup);
						this.LogSqlConnection(connectionBackup);
						connectionBackup.Open();
						return connectionBackup;
					}
					catch (SqlException sqlException2)
					{
						this.HandleSqlConnectionError(connectionBackup, sqlException2);
						goto IL_4E;
					}
					catch (InvalidOperationException invalidOperationException)
					{
						this.HandleTimeoutError(invalidOperationException);
						throw;
					}
				}
				this.HandleSqlConnectionError(connection, sqlException);
				IL_4E:;
			}
			catch (InvalidOperationException invalidOperationException2)
			{
				this.HandleTimeoutError(invalidOperationException2);
				throw;
			}
			return null;
		}

		public void Execute()
		{
			try
			{
				ReportingTaskFaultInjection.FaultInjectionTracer.TraceTest(4140182845U);
				using (IDbConnection dbConnection = this.reportContextFactory.CreateConnection(false))
				{
					using (IDbConnection dbConnection2 = this.reportContextFactory.CreateConnection(true))
					{
						IDbConnection connection = this.OpenConnections(dbConnection, dbConnection2);
						using (IReportContext reportContext = this.reportContextFactory.CreateReportContext(connection))
						{
							IQueryable<TReportObject> reportQuery = this.GetReportQuery(reportContext);
							this.LogSqlStatement(reportContext, reportQuery, 1);
							foreach (TReportObject reportObject in reportQuery)
							{
								this.RaiseReporReceivedtEvent(reportObject);
							}
						}
					}
				}
			}
			catch (SqlException sqlException)
			{
				this.HandleSqlError(sqlException);
			}
			catch (SqlTypeException sqlTypeException)
			{
				this.HandleSqlError(sqlTypeException);
			}
			catch (ArgumentException argumentException)
			{
				this.HandleLinqError(argumentException);
			}
			catch (InvalidOperationException invalidOperationException)
			{
				this.HandleTimeoutError(invalidOperationException);
				throw;
			}
		}

		public void AddQueryDecorator(QueryDecorator<TReportObject> queryDecorator)
		{
			if (!this.queryDecorators.Contains(queryDecorator))
			{
				this.queryDecorators.Add(queryDecorator);
			}
		}

		public void Validate(bool isPipeline)
		{
			foreach (QueryDecorator<TReportObject> queryDecorator in this.queryDecorators)
			{
				if (queryDecorator.IsPipeline == isPipeline)
				{
					queryDecorator.Validate();
				}
			}
		}

		protected virtual IQueryable<TReportObject> GetReportQuery(IReportContext reportContext)
		{
			IQueryable<TReportObject> reports = reportContext.GetReports<TReportObject>();
			return this.DecorateQuery(reports);
		}

		private void OpenConnection(IDbConnection connection)
		{
			try
			{
				this.LogSqlConnection(connection);
				connection.Open();
			}
			catch (SqlException sqlException)
			{
				this.HandleSqlConnectionError(connection, sqlException);
			}
			catch (InvalidOperationException invalidOperationException)
			{
				this.HandleTimeoutError(invalidOperationException);
				throw;
			}
		}

		private void RaiseReporReceivedtEvent(TReportObject reportObject)
		{
			if (this.ReportReceived != null)
			{
				this.ReportReceived(reportObject);
			}
		}

		protected IQueryable<TReportObject> DecorateQuery(IQueryable<TReportObject> query)
		{
			IOrderedEnumerable<QueryDecorator<TReportObject>> source = from decorator in this.queryDecorators
			where !this.IsExpressionEnforced || decorator.IsEnforced
			orderby decorator.QueryOrder
			select decorator;
			return source.Aggregate(query, (IQueryable<TReportObject> current, QueryDecorator<TReportObject> decorator) => decorator.GetQuery(current));
		}

		private void LogSqlConnection(IDbConnection connection)
		{
			ExTraceGlobals.ReportingWebServiceTracer.Information<string>(0L, "SQL Connection: {0}", connection.ConnectionString);
			this.TaskContext.WriteVerbose(Strings.InformationSqlConnection(connection.ConnectionString));
			if (this.StatementLogged != null)
			{
				this.StatementLogged("SQLConnection", Strings.InformationSqlConnection(connection.ConnectionString));
			}
		}

		protected void LogSqlStatement(IReportContext reportContext, IQueryable<TReportObject> query, int logTag)
		{
			string text = query.Expression.ToString();
			string sqlCommandText = reportContext.GetSqlCommandText(query);
			this.TaskContext.WriteVerbose(Strings.InformationSqlStatement(sqlCommandText));
			this.TaskContext.WriteVerbose(Strings.InformationQueryExpression(text));
			ExTraceGlobals.ReportingWebServiceTracer.Information<string>(0L, "SQL Query: {0}", sqlCommandText);
			ExTraceGlobals.ReportingWebServiceTracer.Information<string>(0L, "Expression: {0}", text);
			if (this.StatementLogged != null)
			{
				this.StatementLogged("SQLStatement" + logTag.ToString(), Strings.InformationSqlStatement(sqlCommandText));
				this.StatementLogged("QueryExpression" + logTag.ToString(), Strings.InformationQueryExpression(text));
			}
		}

		private void HandleSqlConnectionFailOverWarning(IDbConnection connection, SqlException sqlException, IDbConnection backupConnection)
		{
			try
			{
				this.TraceSqlException(sqlException);
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_DataMartConnectionFailOverToBackupServer, new string[]
				{
					connection.ConnectionString,
					backupConnection.ConnectionString,
					sqlException.Number.ToString(CultureInfo.InvariantCulture),
					sqlException.Message
				});
			}
			catch (Exception)
			{
			}
		}

		private void HandleSqlConnectionError(IDbConnection connection, SqlException sqlException)
		{
			this.TraceSqlException(sqlException);
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_DataMartConnectionFailed, new string[]
			{
				connection.ConnectionString,
				sqlException.Number.ToString(CultureInfo.InvariantCulture),
				sqlException.Message
			});
			LocalizedException localizedException = SqlErrorHandler.TrasnlateConnectionError(sqlException);
			this.TaskContext.WriteError(localizedException, ExchangeErrorCategory.ServerOperation, null);
		}

		private void HandleLinqError(ArgumentException argumentException)
		{
			ExTraceGlobals.ReportingWebServiceTracer.TraceError<string>(0L, "LinqException. Message: {0}", argumentException.Message);
			LocalizedException localizedException = new InvalidQueryException(0, argumentException);
			this.TaskContext.WriteError(localizedException, ExchangeErrorCategory.ServerOperation, null);
		}

		private void HandleSqlError(SqlException sqlException)
		{
			this.TraceSqlException(sqlException);
			if (SqlErrorHandler.IsObjectNotFoundError(sqlException))
			{
				this.TaskContext.WriteWarning(Strings.WarningReportNotAvailable);
				return;
			}
			LocalizedException localizedException = SqlErrorHandler.TrasnlateError(sqlException);
			this.TaskContext.WriteError(localizedException, ExchangeErrorCategory.ServerOperation, null);
		}

		private void HandleTimeoutError(InvalidOperationException invalidOperationException)
		{
			if (invalidOperationException.StackTrace.Contains("System.Data.SqlClient"))
			{
				LocalizedException localizedException = new DataMartTimeoutException(invalidOperationException);
				this.TaskContext.WriteError(localizedException, ExchangeErrorCategory.ServerOperation, null);
			}
		}

		private void HandleSqlError(SqlTypeException sqlTypeException)
		{
			ExTraceGlobals.ReportingWebServiceTracer.TraceError<string>(0L, "SqlTypeException. Message: {0}", sqlTypeException.Message);
			LocalizedException localizedException = SqlErrorHandler.TrasnlateError(sqlTypeException);
			this.TaskContext.WriteError(localizedException, ExchangeErrorCategory.ServerOperation, null);
		}

		private void TraceSqlException(SqlException sqlException)
		{
			if (ExTraceGlobals.ReportingWebServiceTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.ReportingWebServiceTracer.TraceError(0L, "SqlException. Class: {0}, Number: {1}, Procedure: {2}, LineNumber: {3}, Server: {4}, Source: {5}, State: {6}, Message: {7}", new object[]
				{
					sqlException.Class,
					sqlException.Number,
					sqlException.Procedure,
					sqlException.LineNumber,
					sqlException.Server,
					sqlException.Source,
					sqlException.State,
					sqlException.Message
				});
			}
		}

		protected readonly List<QueryDecorator<TReportObject>> queryDecorators;

		private readonly IReportContextFactory reportContextFactory;

		private readonly ExpressionDecorator<TReportObject> expressionDecorator;
	}
}
