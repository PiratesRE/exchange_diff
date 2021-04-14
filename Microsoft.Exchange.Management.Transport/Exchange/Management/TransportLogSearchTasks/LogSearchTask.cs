using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.LogSearch;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.Management.TransportLogSearchTasks
{
	public abstract class LogSearchTask : Task, IProgressReport
	{
		public LogSearchTask(LocalizedString activityName, CsvTable table, string logName)
		{
			this.activityName = activityName;
			this.table = table;
			this.logName = logName;
		}

		protected CsvTable Table
		{
			get
			{
				return this.table;
			}
		}

		[Parameter(Mandatory = false)]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return this.server;
			}
			set
			{
				this.server = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return this.resultSize;
			}
			set
			{
				this.resultSize = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime End
		{
			get
			{
				return this.end;
			}
			set
			{
				this.end = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime Start
		{
			get
			{
				return this.start;
			}
			set
			{
				this.start = value;
			}
		}

		public void Report(int progress)
		{
			base.WriteProgress(this.activityName, Strings.SearchStatus(this.server.ToString()), progress);
		}

		protected abstract LogCondition GetCondition();

		protected abstract void WriteResult(LogSearchCursor cursor);

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.server == null)
			{
				this.server = new ServerIdParameter();
			}
			if (this.adSession == null)
			{
				this.adSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 197, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\LogSearch\\LogSearchTask.cs");
			}
			IEnumerable<Server> objects = this.server.GetObjects<Server>(null, this.adSession);
			IEnumerator<Server> enumerator = objects.GetEnumerator();
			Server server = null;
			if (enumerator.MoveNext())
			{
				server = enumerator.Current;
				if (enumerator.MoveNext())
				{
					base.ThrowTerminatingError(new LocalizedException(Strings.ServerNameAmbiguous(this.server.ToString())), ErrorCategory.InvalidArgument, this.server);
				}
			}
			else
			{
				base.ThrowTerminatingError(new LocalizedException(Strings.ServerNotFound(this.server.ToString())), ErrorCategory.InvalidArgument, this.server);
			}
			if (!server.IsExchange2007OrLater)
			{
				base.ThrowTerminatingError(new LocalizedException(Strings.PreE12Server(this.server.ToString())), ErrorCategory.InvalidOperation, this.server);
			}
			else if (!server.IsHubTransportServer && !server.IsEdgeServer && !server.IsMailboxServer)
			{
				base.ThrowTerminatingError(new LocalizedException(Strings.NotTransportServer(this.server.ToString())), ErrorCategory.InvalidOperation, this.server);
			}
			else
			{
				if (string.IsNullOrEmpty(server.Fqdn))
				{
					base.ThrowTerminatingError(new LocalizedException(Strings.MissingServerFQDN(this.server.ToString())), ErrorCategory.InvalidOperation, this.server);
				}
				this.serverObject = server;
			}
			if (this.end <= this.start)
			{
				base.ThrowTerminatingError(new LocalizedException(Strings.EmptyTimeRange), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			LogQuery logQuery = new LogQuery();
			logQuery.Filter = this.GetCondition();
			logQuery.Beginning = this.start.ToUniversalTime();
			logQuery.End = this.end.ToUniversalTime();
			uint num = 0U;
			try
			{
				using (LogSearchCursor logSearchCursor = new LogSearchCursor(this.table, this.serverObject.Fqdn, this.serverObject.AdminDisplayVersion, this.logName, logQuery, this))
				{
					this.cursor = logSearchCursor;
					while (logSearchCursor.MoveNext())
					{
						if (!this.resultSize.IsUnlimited && num >= this.resultSize.Value)
						{
							this.WriteWarning(Strings.WarningMoreResultsAvailable);
							break;
						}
						num += 1U;
						this.WriteResult(logSearchCursor);
					}
				}
			}
			catch (LogSearchException ex)
			{
				if (!base.Stopping || ex.ErrorCode != LogSearchErrorCode.LOGSEARCH_E_SESSION_CANCELED)
				{
					this.WriteLogSearchError(ex);
				}
			}
			catch (RpcException e)
			{
				this.WriteRpcError(e);
			}
		}

		protected override void InternalStopProcessing()
		{
			this.cursor.Cancel();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception);
		}

		private void WriteLogSearchError(LogSearchException e)
		{
			LocalizedException exception;
			ErrorCategory category;
			if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_LOG_UNKNOWN_LOG)
			{
				exception = new LocalizedException(Strings.LogNotAvailable);
				category = ErrorCategory.InvalidOperation;
			}
			else if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_UNKNOWN_SESSION_ID || e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_SESSION_CANCELED)
			{
				exception = new LocalizedException(Strings.SearchTimeout);
				category = ErrorCategory.OperationTimeout;
			}
			else if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_SERVER_TOO_BUSY)
			{
				exception = new LocalizedException(Strings.ServerTooBusy);
				category = ErrorCategory.ResourceUnavailable;
			}
			else if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_INVALID_QUERY_CONDITION || e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_INVALID_OPERAND_CLASS || e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_INCOMPATIBLE_QUERY_OPERAND_TYPES)
			{
				exception = new LocalizedException(Strings.OldServerSearchLogic);
				category = ErrorCategory.InvalidOperation;
			}
			else if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_UNRECOGNIZED_QUERY_FIELD)
			{
				exception = new LocalizedException(Strings.OldServerSchema);
				category = ErrorCategory.InvalidOperation;
			}
			else if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_MISSING_QUERY_CONDITION || e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_UNBOUND_QUERY_VARIABLE || e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_DUPLICATE_BOUND_VARIABLE_DECLARATION || e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_RESPONSE_OVERFLOW || e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_BAD_QUERY_XML || e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_MISSING_BOUND_VARIABLE_NAME)
			{
				exception = new LocalizedException(Strings.InternalError);
				category = ErrorCategory.InvalidOperation;
			}
			else if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_QUERY_TOO_COMPLEX)
			{
				exception = new LocalizedException(Strings.QueryTooComplex);
				category = ErrorCategory.InvalidOperation;
			}
			else
			{
				Win32Exception ex = new Win32Exception(e.ErrorCode);
				exception = new LocalizedException(Strings.GenericError(ex.Message), ex);
				category = ErrorCategory.InvalidOperation;
			}
			base.WriteError(exception, category, null);
		}

		private void WriteRpcError(RpcException e)
		{
			LocalizedException exception;
			ErrorCategory category;
			if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_ENDPOINT_NOT_REGISTERED)
			{
				exception = new LocalizedException(Strings.RpcNotRegistered(this.serverObject.Fqdn));
				category = ErrorCategory.ResourceUnavailable;
			}
			else if (e.ErrorCode == LogSearchErrorCode.LOGSEARCH_E_RPC_SERVER_UNAVAILABLE)
			{
				exception = new LocalizedException(Strings.RpcUnavailable(this.serverObject.Fqdn));
				category = ErrorCategory.InvalidOperation;
			}
			else
			{
				Win32Exception ex = new Win32Exception(e.ErrorCode);
				exception = new LocalizedException(Strings.GenericRpcError(ex.Message, this.serverObject.Fqdn), ex);
				category = ErrorCategory.InvalidOperation;
			}
			base.WriteError(exception, category, null);
		}

		internal IConfigurationSession adSession;

		private DateTime end = DateTime.UtcNow.AddDays(1.0);

		private DateTime start = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;

		private LocalizedString activityName;

		private readonly string logName;

		private CsvTable table;

		private ServerIdParameter server;

		private Server serverObject;

		private LogSearchCursor cursor;

		private Unlimited<uint> resultSize = 1000U;
	}
}
