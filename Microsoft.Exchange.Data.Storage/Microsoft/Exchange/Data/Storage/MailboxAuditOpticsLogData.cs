using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAuditOpticsLogData : IDisposable
	{
		public string Tenant { get; set; }

		public string Mailbox { get; set; }

		public string Operation { get; set; }

		public string LogonType { get; set; }

		public OperationResult OperationSucceeded { get; set; }

		public long LoggingTime { get; set; }

		public int RecordSize { get; set; }

		public bool AuditSucceeded { get; set; }

		public Exception LoggingError { get; set; }

		public string ActionContext { get; set; }

		public string ClientRequestId { get; set; }

		public bool ExternalAccess { get; set; }

		public bool Asynchronous { get; set; }

		public Guid RecordId { get; set; }

		public void Dispose()
		{
			AuditingOpticsLogger.LogAuditOpticsEntry(AuditingOpticsLoggerType.MailboxAudit, AuditingOpticsLogger.GetLogColumns<MailboxAuditOpticsLogData>(this, MailboxAuditOpticsLogData.logSchema));
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MailboxAuditOpticsLogData()
		{
			LogTableSchema<MailboxAuditOpticsLogData>[] array = new LogTableSchema<MailboxAuditOpticsLogData>[15];
			array[0] = new LogTableSchema<MailboxAuditOpticsLogData>("Tenant", (MailboxAuditOpticsLogData data) => data.Tenant);
			array[1] = new LogTableSchema<MailboxAuditOpticsLogData>("Mailbox", (MailboxAuditOpticsLogData data) => data.Mailbox);
			array[2] = new LogTableSchema<MailboxAuditOpticsLogData>("Operation", (MailboxAuditOpticsLogData data) => data.Operation);
			array[3] = new LogTableSchema<MailboxAuditOpticsLogData>("LogonType", (MailboxAuditOpticsLogData data) => data.LogonType);
			array[4] = new LogTableSchema<MailboxAuditOpticsLogData>("OperationSucceeded", delegate(MailboxAuditOpticsLogData data)
			{
				if (data.OperationSucceeded != OperationResult.Succeeded)
				{
					return "0";
				}
				return "1";
			});
			array[5] = new LogTableSchema<MailboxAuditOpticsLogData>("AuditSucceeded", delegate(MailboxAuditOpticsLogData data)
			{
				if (!data.AuditSucceeded)
				{
					return "0";
				}
				return "1";
			});
			array[6] = new LogTableSchema<MailboxAuditOpticsLogData>("LoggingError", (MailboxAuditOpticsLogData data) => AuditingOpticsLogger.GetExceptionNamesForTrace(data.LoggingError));
			array[7] = new LogTableSchema<MailboxAuditOpticsLogData>("LoggingTime", (MailboxAuditOpticsLogData data) => data.LoggingTime.ToString());
			array[8] = new LogTableSchema<MailboxAuditOpticsLogData>("RecordSize", (MailboxAuditOpticsLogData data) => data.RecordSize.ToString());
			array[9] = new LogTableSchema<MailboxAuditOpticsLogData>("ActionContext", (MailboxAuditOpticsLogData data) => data.ActionContext);
			array[10] = new LogTableSchema<MailboxAuditOpticsLogData>("ClientRequestId", (MailboxAuditOpticsLogData data) => data.ClientRequestId);
			array[11] = new LogTableSchema<MailboxAuditOpticsLogData>("ExternalAccess", delegate(MailboxAuditOpticsLogData data)
			{
				if (!data.ExternalAccess)
				{
					return "0";
				}
				return "1";
			});
			array[12] = new LogTableSchema<MailboxAuditOpticsLogData>("DiagnosticContext", (MailboxAuditOpticsLogData data) => AuditingOpticsLogger.GetDiagnosticContext(data.LoggingError));
			array[13] = new LogTableSchema<MailboxAuditOpticsLogData>("Asynchronous", delegate(MailboxAuditOpticsLogData data)
			{
				if (!data.Asynchronous)
				{
					return "0";
				}
				return "1";
			});
			array[14] = new LogTableSchema<MailboxAuditOpticsLogData>("RecordId", (MailboxAuditOpticsLogData data) => data.RecordId.ToString());
			MailboxAuditOpticsLogData.logSchema = array;
		}

		internal static LogTableSchema<MailboxAuditOpticsLogData>[] logSchema;
	}
}
