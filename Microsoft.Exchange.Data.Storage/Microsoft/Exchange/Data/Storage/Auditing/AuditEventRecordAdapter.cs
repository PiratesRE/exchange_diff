using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Compliance.Audit.Schema.Mailbox;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AuditEventRecordAdapter : IAuditEvent
	{
		protected AuditEventRecordAdapter(ExchangeMailboxAuditBaseRecord record, string displayOrganizationId)
		{
			this.record = record;
			Util.ThrowOnNullArgument(record, "record");
			MailboxAuditOperations mailboxAuditOperations;
			this.AuditOperation = (Enum.TryParse<MailboxAuditOperations>(record.Operation, out mailboxAuditOperations) ? mailboxAuditOperations : MailboxAuditOperations.None);
			this.Result = ((record.OperationResult == null) ? OperationResult.Succeeded : ((record.OperationResult == 2) ? OperationResult.PartiallySucceeded : OperationResult.Failed));
			this.OrganizationId = displayOrganizationId;
			this.MailboxGuid = record.MailboxGuid;
			this.OperationName = record.Operation;
			this.LogonTypeName = record.LogonType.ToString();
			this.OperationSucceeded = ((record.OperationResult == null) ? OperationResult.Succeeded : ((record.OperationResult == 2) ? OperationResult.PartiallySucceeded : OperationResult.Failed));
			this.ExternalAccess = record.ExternalAccess;
			this.CreationTime = record.CreationTime;
			this.RecordId = record.Id;
		}

		internal MailboxAuditOperations AuditOperation { get; private set; }

		internal OperationResult Result { get; private set; }

		public DateTime CreationTime { get; private set; }

		public Guid RecordId { get; private set; }

		public string OrganizationId { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public string OperationName { get; private set; }

		public string LogonTypeName { get; private set; }

		public OperationResult OperationSucceeded { get; private set; }

		public bool ExternalAccess { get; private set; }

		public IAuditLogRecord GetLogRecord()
		{
			return new AuditEventRecordAdapter.AuditLogRecord(this);
		}

		protected virtual IEnumerable<KeyValuePair<string, string>> InternalGetEventDetails()
		{
			yield return this.MakePair("Operation", this.AuditOperation);
			yield return this.MakePair("OperationResult", this.Result);
			yield return this.MakePair("LogonType", this.record.LogonType);
			yield return this.MakePair("ExternalAccess", this.ExternalAccess);
			yield return this.MakePair("UtcTime", this.CreationTime.ToString("s"));
			yield return this.MakePair("InternalLogonType", AuditEventRecordAdapter.GetInternalLogonType(this.record.InternalLogonType));
			yield return this.MakePair("MailboxGuid", this.record.MailboxGuid);
			yield return this.MakePair("MailboxOwnerUPN", this.record.MailboxOwnerUPN);
			KeyValuePair<string, string> p;
			if (!string.IsNullOrEmpty(this.record.MailboxOwnerSid))
			{
				yield return this.MakePair("MailboxOwnerSid", this.record.MailboxOwnerSid);
				if (this.TryMakePair("MailboxOwnerMasterAccountSid", this.record.MailboxOwnerMasterAccountSid, out p))
				{
					yield return p;
				}
			}
			if (this.TryMakePair("LogonUserSid", this.record.LogonUserSid, out p))
			{
				yield return p;
			}
			if (this.TryMakePair("LogonUserDisplayName", this.record.LogonUserDisplayName, out p))
			{
				yield return p;
			}
			if (this.TryMakePair("ClientInfoString", this.record.ClientInfoString, out p))
			{
				yield return p;
			}
			yield return this.MakePair("ClientIPAddress", this.record.ClientIPAddress);
			if (this.TryMakePair("ClientMachineName", this.record.ClientMachineName, out p))
			{
				yield return p;
			}
			if (this.TryMakePair("ClientProcessName", this.record.ClientProcessName, out p))
			{
				yield return p;
			}
			if (this.TryMakePair("ClientVersion", this.record.ClientVersion, out p))
			{
				yield return p;
			}
			if (this.TryMakePair("OriginatingServer", this.record.OriginatingServer, out p))
			{
				yield return p;
			}
			yield break;
		}

		protected KeyValuePair<string, string> MakePair(string name, string value)
		{
			return new KeyValuePair<string, string>(name, value);
		}

		protected KeyValuePair<string, string> MakePair(string name, object value)
		{
			return new KeyValuePair<string, string>(name, string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				value
			}));
		}

		protected bool TryMakePair(string name, string value, out KeyValuePair<string, string> pair)
		{
			bool flag = !string.IsNullOrWhiteSpace(value);
			pair = (flag ? new KeyValuePair<string, string>(name, value) : default(KeyValuePair<string, string>));
			return flag;
		}

		protected bool TryMakePair<T>(string name, T? value, out KeyValuePair<string, string> pair) where T : struct
		{
			bool flag = value != null;
			pair = (flag ? new KeyValuePair<string, string>(name, string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				value.Value
			})) : default(KeyValuePair<string, string>));
			return flag;
		}

		protected bool TryMakePair<T>(string name, T value, out KeyValuePair<string, string> pair) where T : class
		{
			bool flag = value != null;
			pair = (flag ? new KeyValuePair<string, string>(name, string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
			{
				value
			})) : default(KeyValuePair<string, string>));
			return flag;
		}

		private static string GetInternalLogonType(LogonType logonType)
		{
			if (logonType == 2)
			{
				return "Delegated";
			}
			return logonType.ToString();
		}

		internal const int InitialCapacityEstimate = 1024;

		private readonly ExchangeMailboxAuditBaseRecord record;

		private class AuditLogRecord : IAuditLogRecord
		{
			public AuditLogRecord(AuditEventRecordAdapter eventData)
			{
				this.UserId = eventData.record.LogonUserSid;
				this.CreationTime = eventData.CreationTime;
				this.Operation = eventData.AuditOperation.ToString();
				this.eventDetails = new List<KeyValuePair<string, string>>(eventData.InternalGetEventDetails());
			}

			public AuditLogRecordType RecordType
			{
				get
				{
					return AuditLogRecordType.MailboxAudit;
				}
			}

			public DateTime CreationTime { get; private set; }

			public string Operation { get; private set; }

			public string ObjectId
			{
				get
				{
					return null;
				}
			}

			public string UserId { get; private set; }

			public IEnumerable<KeyValuePair<string, string>> GetDetails()
			{
				return this.eventDetails;
			}

			private readonly List<KeyValuePair<string, string>> eventDetails;
		}
	}
}
