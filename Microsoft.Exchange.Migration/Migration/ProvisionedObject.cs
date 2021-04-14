using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal struct ProvisionedObject
	{
		public ProvisionedObject(ObjectId itemId, Guid jobId, ProvisioningType type)
		{
			this.ItemId = itemId;
			this.JobId = jobId;
			this.Type = type;
			this.Succeeded = false;
			this.Error = string.Empty;
			this.MailboxData = null;
			this.IsRetryable = false;
			this.TimeAttempted = ExDateTime.UtcNow;
			this.TimeFinished = null;
			this.GroupMemberProvisioned = 0;
			this.GroupMemberSkipped = 0;
		}

		public readonly ObjectId ItemId;

		public readonly Guid JobId;

		public readonly ProvisioningType Type;

		public bool Succeeded;

		public string Error;

		public bool IsRetryable;

		public IMailboxData MailboxData;

		public int GroupMemberProvisioned;

		public int GroupMemberSkipped;

		public ExDateTime TimeAttempted;

		public ExDateTime? TimeFinished;
	}
}
