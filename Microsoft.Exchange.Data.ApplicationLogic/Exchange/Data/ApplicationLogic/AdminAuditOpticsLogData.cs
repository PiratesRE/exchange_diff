using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class AdminAuditOpticsLogData : IDisposable
	{
		public string Tenant { get; set; }

		public string CmdletName { get; set; }

		public bool ExternalAccess { get; set; }

		public long LoggingTime { get; set; }

		public bool OperationSucceeded { get; set; }

		public int RecordSize { get; set; }

		public bool AuditSucceeded { get; set; }

		public Exception LoggingError { get; set; }

		public OrganizationId ExecutingUserOrganizationId { get; set; }

		public bool Asynchronous { get; set; }

		public Guid RecordId { get; set; }

		public AdminAuditOpticsLogData()
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				this.requestId = currentActivityScope.ActivityId;
				return;
			}
			this.activityScopeToDispose = ActivityContext.Start(null);
			this.requestId = this.activityScopeToDispose.ActivityId;
		}

		public void Disable()
		{
			this.isDisabled = true;
		}

		public void Dispose()
		{
			if (this.activityScopeToDispose != null)
			{
				this.activityScopeToDispose.Dispose();
				this.activityScopeToDispose = null;
			}
			if (this.isDisabled)
			{
				return;
			}
			AuditingOpticsLogger.LogAuditOpticsEntry(AuditingOpticsLoggerType.AdminAudit, AuditingOpticsLogger.GetLogColumns<AdminAuditOpticsLogData>(this, AdminAuditOpticsLogData.logSchema));
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AdminAuditOpticsLogData()
		{
			LogTableSchema<AdminAuditOpticsLogData>[] array = new LogTableSchema<AdminAuditOpticsLogData>[14];
			array[0] = new LogTableSchema<AdminAuditOpticsLogData>("Tenant", (AdminAuditOpticsLogData data) => data.Tenant);
			array[1] = new LogTableSchema<AdminAuditOpticsLogData>("CmdletName", (AdminAuditOpticsLogData data) => data.CmdletName);
			array[2] = new LogTableSchema<AdminAuditOpticsLogData>("CmdletSucceeded", delegate(AdminAuditOpticsLogData data)
			{
				if (!data.OperationSucceeded)
				{
					return "0";
				}
				return "1";
			});
			array[3] = new LogTableSchema<AdminAuditOpticsLogData>("CmdletError", (AdminAuditOpticsLogData data) => string.Empty);
			array[4] = new LogTableSchema<AdminAuditOpticsLogData>("AuditSucceeded", delegate(AdminAuditOpticsLogData data)
			{
				if (!data.AuditSucceeded)
				{
					return "0";
				}
				return "1";
			});
			array[5] = new LogTableSchema<AdminAuditOpticsLogData>("LoggingError", (AdminAuditOpticsLogData data) => AuditingOpticsLogger.GetExceptionNamesForTrace(data.LoggingError, AuditLogExceptionFormatter.Instance));
			array[6] = new LogTableSchema<AdminAuditOpticsLogData>("LoggingTime", (AdminAuditOpticsLogData data) => data.LoggingTime.ToString());
			array[7] = new LogTableSchema<AdminAuditOpticsLogData>("RecordSize", (AdminAuditOpticsLogData data) => data.RecordSize.ToString());
			array[8] = new LogTableSchema<AdminAuditOpticsLogData>("ExternalAccess", delegate(AdminAuditOpticsLogData data)
			{
				if (!data.ExternalAccess)
				{
					return "0";
				}
				return "1";
			});
			array[9] = new LogTableSchema<AdminAuditOpticsLogData>("ExecutingUserOrganizationId", delegate(AdminAuditOpticsLogData data)
			{
				if (!(data.ExecutingUserOrganizationId != null))
				{
					return string.Empty;
				}
				return data.ExecutingUserOrganizationId.ToString();
			});
			array[10] = new LogTableSchema<AdminAuditOpticsLogData>("DiagnosticContext", (AdminAuditOpticsLogData data) => AuditingOpticsLogger.GetDiagnosticContext(data.LoggingError));
			array[11] = new LogTableSchema<AdminAuditOpticsLogData>("RequestId", (AdminAuditOpticsLogData data) => data.requestId.ToString("D"));
			array[12] = new LogTableSchema<AdminAuditOpticsLogData>("Asynchronous", delegate(AdminAuditOpticsLogData data)
			{
				if (!data.Asynchronous)
				{
					return "0";
				}
				return "1";
			});
			array[13] = new LogTableSchema<AdminAuditOpticsLogData>("RecordId", (AdminAuditOpticsLogData data) => data.RecordId.ToString());
			AdminAuditOpticsLogData.logSchema = array;
		}

		internal static LogTableSchema<AdminAuditOpticsLogData>[] logSchema;

		private bool isDisabled;

		private readonly Guid requestId;

		private ActivityScope activityScopeToDispose;
	}
}
