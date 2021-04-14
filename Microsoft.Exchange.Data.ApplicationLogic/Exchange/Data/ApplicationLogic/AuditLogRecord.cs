using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit.Schema;
using Microsoft.Office.Compliance.Audit.Schema.Admin;
using Microsoft.Office.Compliance.Audit.Schema.Mailbox;
using Microsoft.Office.Compliance.Audit.Schema.Monitoring;
using Microsoft.Office.Compliance.Audit.Schema.SharePoint;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct AuditLogRecord : IAuditLogRecord
	{
		public AuditLogRecord(AuditRecord record, Trace trace)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}
			if (trace == null)
			{
				throw new ArgumentNullException("trace");
			}
			this.record = record;
			this.trace = trace;
		}

		public AuditLogRecordType RecordType
		{
			get
			{
				switch (this.record.RecordType)
				{
				case 1:
					return AuditLogRecordType.AdminAudit;
				case 2:
				case 3:
					return AuditLogRecordType.MailboxAudit;
				default:
					throw new NotSupportedException(this.record.RecordType.ToString());
				}
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.record.CreationTime;
			}
		}

		public string Operation
		{
			get
			{
				return this.record.Operation;
			}
		}

		public string ObjectId
		{
			get
			{
				return this.record.ObjectId;
			}
		}

		public string UserId
		{
			get
			{
				return this.record.UserId;
			}
		}

		public IEnumerable<KeyValuePair<string, string>> GetDetails()
		{
			AuditLogRecord.RecordVisitor recordVisitor = new AuditLogRecord.RecordVisitor(this.trace);
			this.record.Visit(recordVisitor);
			IAuditLogRecord result = recordVisitor.Result;
			return result.GetDetails();
		}

		public static IAuditLogRecord FillAdminRecordDetails(ExchangeAdminAuditRecord auditRecord, Trace trace)
		{
			AdminAuditLogRecord adminAuditLogRecord = new AdminAuditLogRecord(trace)
			{
				Verbose = true,
				Cmdlet = auditRecord.Operation,
				ExternalAccess = auditRecord.ExternalAccess,
				ObjectModified = auditRecord.ObjectId,
				UserId = auditRecord.UserId,
				RunDate = auditRecord.CreationTime,
				Succeeded = (auditRecord.Succeeded ?? false),
				Error = auditRecord.Error,
				ModifiedObjectResolvedName = auditRecord.ModifiedObjectResolvedName,
				Parameters = new Hashtable(),
				ModifiedPropertyValues = new Hashtable(),
				OriginalPropertyValues = new Hashtable()
			};
			if (auditRecord.Parameters != null)
			{
				foreach (NameValuePair nameValuePair in auditRecord.Parameters)
				{
					adminAuditLogRecord.Parameters.Add(nameValuePair.Name, nameValuePair.Value);
				}
			}
			if (auditRecord.ModifiedProperties != null)
			{
				foreach (ModifiedProperty modifiedProperty in auditRecord.ModifiedProperties)
				{
					adminAuditLogRecord.ModifiedPropertyValues.Add(modifiedProperty.Name, modifiedProperty.NewValue);
					adminAuditLogRecord.OriginalPropertyValues.Add(modifiedProperty.Name, modifiedProperty.OldValue);
				}
			}
			return adminAuditLogRecord;
		}

		public static IAuditLogRecord FillMailboxAuditRecordDetails(ExchangeMailboxAuditRecord auditRecord, Trace trace)
		{
			ItemOperationAuditEventRecordAdapter itemOperationAuditEventRecordAdapter = new ItemOperationAuditEventRecordAdapter(auditRecord, auditRecord.OrganizationId);
			return itemOperationAuditEventRecordAdapter.GetLogRecord();
		}

		public static IAuditLogRecord FillMailboxAuditGroupRecordDetails(ExchangeMailboxAuditGroupRecord auditRecord, Trace trace)
		{
			GroupOperationAuditEventRecordAdapter groupOperationAuditEventRecordAdapter = new GroupOperationAuditEventRecordAdapter(auditRecord, auditRecord.OrganizationId);
			return groupOperationAuditEventRecordAdapter.GetLogRecord();
		}

		private readonly AuditRecord record;

		private readonly Trace trace;

		private class RecordVisitor : IAuditRecordVisitor
		{
			public RecordVisitor(Trace trace)
			{
				this.trace = trace;
			}

			public IAuditLogRecord Result { get; private set; }

			public void Visit(ExchangeAdminAuditRecord record)
			{
				this.Result = AuditLogRecord.FillAdminRecordDetails(record, this.trace);
			}

			public void Visit(ExchangeMailboxAuditRecord record)
			{
				this.Result = AuditLogRecord.FillMailboxAuditRecordDetails(record, this.trace);
			}

			public void Visit(ExchangeMailboxAuditGroupRecord record)
			{
				this.Result = AuditLogRecord.FillMailboxAuditGroupRecordDetails(record, this.trace);
			}

			public void Visit(SharePointAuditRecord record)
			{
				throw new NotImplementedException();
			}

			public void Visit(SyntheticProbeAuditRecord record)
			{
				throw new NotImplementedException();
			}

			private readonly Trace trace;
		}
	}
}
