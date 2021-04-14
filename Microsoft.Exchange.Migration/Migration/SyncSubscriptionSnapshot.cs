using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncSubscriptionSnapshot : SubscriptionSnapshot
	{
		public SyncSubscriptionSnapshot(ISubscriptionId id, SnapshotStatus status, bool isInitialSyncComplete, ExDateTime createTime, ExDateTime? lastUpdateTime, ExDateTime? lastSyncTime, LocalizedString? errorMessage, string batchName, SyncProtocol protocol, string userName, SmtpAddress emailAddress) : base(id, status, isInitialSyncComplete, createTime, lastUpdateTime, lastSyncTime, errorMessage, batchName)
		{
			this.Protocol = protocol;
			this.UserName = userName;
			this.EmailAddress = emailAddress;
		}

		public SyncProtocol Protocol { get; private set; }

		public string UserName { get; private set; }

		public SmtpAddress EmailAddress { get; private set; }
	}
}
