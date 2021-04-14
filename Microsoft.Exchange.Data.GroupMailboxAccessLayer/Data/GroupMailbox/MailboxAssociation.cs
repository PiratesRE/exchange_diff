using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAssociation
	{
		public GroupMailboxLocator Group { get; set; }

		public UserMailboxLocator User { get; set; }

		public SmtpAddress GroupSmtpAddress { get; set; }

		public SmtpAddress UserSmtpAddress { get; set; }

		public bool IsMember { get; set; }

		public string JoinedBy { get; set; }

		public bool IsPin { get; set; }

		public bool ShouldEscalate { get; set; }

		public bool IsAutoSubscribed { get; set; }

		public ExDateTime JoinDate { get; set; }

		public ExDateTime LastVisitedDate { get; set; }

		public ExDateTime PinDate { get; set; }

		public ExDateTime LastModified { get; set; }

		public string SyncedIdentityHash { get; set; }

		public int CurrentVersion { get; set; }

		public int SyncedVersion { get; set; }

		public string LastSyncError { get; set; }

		public int SyncAttempts { get; set; }

		public string SyncedSchemaVersion { get; set; }

		public bool IsOutOfSync(string expectedIdentityHash)
		{
			if (this.CurrentVersion > this.SyncedVersion)
			{
				MailboxAssociation.Tracer.TraceDebug((long)this.GetHashCode(), "MailboxAssociation::IsOutOfSync. Association {0}/{1} is out of sync because current version ({2}) is greater than synced version ({3})", new object[]
				{
					this.User,
					this.Group,
					this.CurrentVersion,
					this.SyncedVersion
				});
				return true;
			}
			if (!StringComparer.OrdinalIgnoreCase.Equals(this.SyncedIdentityHash, expectedIdentityHash))
			{
				MailboxAssociation.Tracer.TraceDebug((long)this.GetHashCode(), "MailboxAssociation::IsOutOfSync. Association {0}/{1} is out of sync because current identity hash of mailbox ({2}) is different than the one synced ({3})", new object[]
				{
					this.User,
					this.Group,
					expectedIdentityHash,
					this.SyncedIdentityHash
				});
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("Group={");
			stringBuilder.Append(this.Group);
			stringBuilder.Append("}, User={");
			stringBuilder.Append(this.User);
			stringBuilder.Append("}, GroupSmtpAddress=");
			stringBuilder.Append(this.GroupSmtpAddress);
			stringBuilder.Append(", UserSmtpAddress=");
			stringBuilder.Append(this.UserSmtpAddress);
			stringBuilder.Append(", IsMember=");
			stringBuilder.Append(this.IsMember);
			stringBuilder.Append(", JoinedBy=");
			stringBuilder.Append(this.JoinedBy);
			stringBuilder.Append(", JoinDate=");
			stringBuilder.Append(this.JoinDate);
			stringBuilder.Append(", IsPin=");
			stringBuilder.Append(this.IsPin);
			stringBuilder.Append(", ShouldEscalate=");
			stringBuilder.Append(this.ShouldEscalate);
			stringBuilder.Append(", IsAutoSubscribed=");
			stringBuilder.Append(this.IsAutoSubscribed);
			stringBuilder.Append(", LastVisitedDate=");
			stringBuilder.Append(this.LastVisitedDate);
			stringBuilder.Append(", PinDate=");
			stringBuilder.Append(this.PinDate);
			stringBuilder.Append(", SyncedIdentityHash=");
			stringBuilder.Append(this.SyncedIdentityHash);
			stringBuilder.Append(", CurrentVersion=");
			stringBuilder.Append(this.CurrentVersion);
			stringBuilder.Append(", SyncedVersion=");
			stringBuilder.Append(this.SyncedVersion);
			stringBuilder.Append(", LastSyncError=");
			stringBuilder.Append(this.LastSyncError);
			stringBuilder.Append(", SyncAttempts =");
			stringBuilder.Append(this.SyncAttempts);
			stringBuilder.Append(", SyncedSchemaVersion=");
			stringBuilder.Append(this.SyncedSchemaVersion);
			stringBuilder.Append(", LastModified=");
			stringBuilder.Append(this.LastModified);
			return stringBuilder.ToString();
		}

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;
	}
}
