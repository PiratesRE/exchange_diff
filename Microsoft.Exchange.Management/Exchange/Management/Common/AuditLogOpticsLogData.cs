using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.Common
{
	internal class AuditLogOpticsLogData : IDisposable
	{
		public OrganizationId OrganizationId { get; set; }

		public string SearchType { get; set; }

		public int QueryComplexity { get; set; }

		public long ResultsReturned { get; set; }

		public bool CallResult { get; set; }

		public Exception ErrorType { get; set; }

		public int ErrorCount { get; set; }

		public bool IsAsynchronous { get; set; }

		public bool ShowDetails { get; set; }

		public int MailboxCount { get; set; }

		private Stopwatch Stopwatch { get; set; }

		public DateTime? SearchStartDateTime { get; set; }

		public DateTime? SearchEndDateTime { get; set; }

		public string CorrelationID { get; set; }

		public int Retry { get; set; }

		public bool RequestDeleted { get; set; }

		public string LastProcessedMailbox { get; set; }

		public AuditLogOpticsLogData()
		{
			this.Stopwatch = Stopwatch.StartNew();
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				this.requestId = currentActivityScope.ActivityId;
				return;
			}
			this.activityScopeToDispose = ActivityContext.Start(null);
			this.requestId = this.activityScopeToDispose.ActivityId;
		}

		public void Dispose()
		{
			this.Stopwatch.Stop();
			if (this.activityScopeToDispose != null)
			{
				this.activityScopeToDispose.Dispose();
				this.activityScopeToDispose = null;
			}
			AuditingOpticsLogger.LogAuditOpticsEntry(AuditingOpticsLoggerType.AuditSearch, AuditingOpticsLogger.GetLogColumns<AuditLogOpticsLogData>(this, AuditLogOpticsLogData.LogSchema));
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AuditLogOpticsLogData()
		{
			LogTableSchema<AuditLogOpticsLogData>[] array = new LogTableSchema<AuditLogOpticsLogData>[19];
			array[0] = new LogTableSchema<AuditLogOpticsLogData>("Tenant", delegate(AuditLogOpticsLogData data)
			{
				if (!(data.OrganizationId == null))
				{
					return data.OrganizationId.ToString();
				}
				return "First Org";
			});
			array[1] = new LogTableSchema<AuditLogOpticsLogData>("SearchType", (AuditLogOpticsLogData data) => data.SearchType);
			array[2] = new LogTableSchema<AuditLogOpticsLogData>("QueryComplexity", (AuditLogOpticsLogData data) => data.QueryComplexity.ToString(CultureInfo.InvariantCulture));
			array[3] = new LogTableSchema<AuditLogOpticsLogData>("ExecutionTime", (AuditLogOpticsLogData data) => data.Stopwatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
			array[4] = new LogTableSchema<AuditLogOpticsLogData>("CallResult", delegate(AuditLogOpticsLogData data)
			{
				if (!data.CallResult)
				{
					return "0";
				}
				return "1";
			});
			array[5] = new LogTableSchema<AuditLogOpticsLogData>("ErrorType", (AuditLogOpticsLogData data) => AuditingOpticsLogger.GetExceptionNamesForTrace(data.ErrorType, AuditLogExceptionFormatter.Instance));
			array[6] = new LogTableSchema<AuditLogOpticsLogData>("ResultsReturned", (AuditLogOpticsLogData data) => data.ResultsReturned.ToString(CultureInfo.InvariantCulture));
			array[7] = new LogTableSchema<AuditLogOpticsLogData>("ErrorCount", (AuditLogOpticsLogData data) => data.ErrorCount.ToString(CultureInfo.InvariantCulture));
			array[8] = new LogTableSchema<AuditLogOpticsLogData>("Async", delegate(AuditLogOpticsLogData data)
			{
				if (!data.IsAsynchronous)
				{
					return "0";
				}
				return "1";
			});
			array[9] = new LogTableSchema<AuditLogOpticsLogData>("ShowDetails", delegate(AuditLogOpticsLogData data)
			{
				if (!data.ShowDetails)
				{
					return "0";
				}
				return "1";
			});
			array[10] = new LogTableSchema<AuditLogOpticsLogData>("MailboxCount", (AuditLogOpticsLogData data) => data.MailboxCount.ToString(CultureInfo.InvariantCulture));
			array[11] = new LogTableSchema<AuditLogOpticsLogData>("SearchStartDateTime", (AuditLogOpticsLogData data) => data.SearchStartDateTime.ToString());
			array[12] = new LogTableSchema<AuditLogOpticsLogData>("SearchEndDateTime", (AuditLogOpticsLogData data) => data.SearchEndDateTime.ToString());
			array[13] = new LogTableSchema<AuditLogOpticsLogData>("CorrelationID", (AuditLogOpticsLogData data) => data.CorrelationID);
			array[14] = new LogTableSchema<AuditLogOpticsLogData>("Retry", (AuditLogOpticsLogData data) => data.Retry.ToString(CultureInfo.InvariantCulture));
			array[15] = new LogTableSchema<AuditLogOpticsLogData>("RequestDeleted", delegate(AuditLogOpticsLogData data)
			{
				if (!data.RequestDeleted)
				{
					return "0";
				}
				return "1";
			});
			array[16] = new LogTableSchema<AuditLogOpticsLogData>("LastProcessedMailbox", (AuditLogOpticsLogData data) => data.LastProcessedMailbox);
			array[17] = new LogTableSchema<AuditLogOpticsLogData>("DiagnosticContext", (AuditLogOpticsLogData data) => AuditingOpticsLogger.GetDiagnosticContext(data.ErrorType));
			array[18] = new LogTableSchema<AuditLogOpticsLogData>("RequestId", (AuditLogOpticsLogData data) => data.requestId.ToString("D"));
			AuditLogOpticsLogData.LogSchema = array;
		}

		public const string MailboxSearchType = "Mailbox";

		public const string AdminSearchType = "Admin";

		private const string LogTrue = "1";

		private const string LogFalse = "0";

		internal static LogTableSchema<AuditLogOpticsLogData>[] LogSchema;

		private readonly Guid requestId;

		private ActivityScope activityScopeToDispose;
	}
}
