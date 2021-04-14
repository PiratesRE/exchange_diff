using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public sealed class SyncRequestIdParameter : MRSRequestIdParameter
	{
		public SyncRequestIdParameter()
		{
		}

		public SyncRequestIdParameter(SyncRequest request) : base(request)
		{
			base.MailboxId = request.Mailbox;
		}

		public SyncRequestIdParameter(RequestJobObjectId requestJobId) : base(requestJobId)
		{
			if (requestJobId.User != null)
			{
				base.MailboxId = requestJobId.User.ObjectId;
				return;
			}
			if (requestJobId.TargetUser != null)
			{
				base.MailboxId = requestJobId.TargetUser.ObjectId;
				return;
			}
			if (requestJobId.IndexEntry != null)
			{
				base.MailboxId = (requestJobId.IndexEntry.TargetUserId ?? requestJobId.IndexEntry.RequestIndexId.Mailbox);
			}
		}

		public SyncRequestIdParameter(SyncRequestStatistics requestStats) : base(requestStats)
		{
			base.MailboxId = requestStats.TargetUserId;
		}

		public SyncRequestIdParameter(RequestIndexEntryObjectId identity) : base(identity)
		{
			if (identity.IndexId != null)
			{
				base.MailboxId = identity.IndexId.Mailbox;
				return;
			}
			if (identity.RequestObject != null)
			{
				base.MailboxId = identity.RequestObject.TargetMailbox;
			}
		}

		public SyncRequestIdParameter(string request) : base(request)
		{
		}

		internal SyncRequestIdParameter(Guid requestGuid, OrganizationId orgId, string mailboxName) : base(requestGuid, orgId, mailboxName)
		{
		}

		public static SyncRequestIdParameter Parse(string request)
		{
			return new SyncRequestIdParameter(request);
		}

		public static SyncRequestIdParameter Create(ADUser requestOwner, string requestName)
		{
			ArgumentValidator.ThrowIfNull("requestName", requestName);
			ArgumentValidator.ThrowIfNull("requestOwner", requestOwner);
			ArgumentValidator.ThrowIfNull("requestOwner.OrganizationId", requestOwner.OrganizationId);
			ArgumentValidator.ThrowIfNull("requestOwner.OrganizationId.OrganizationalUnit", requestOwner.OrganizationId.OrganizationalUnit);
			return new SyncRequestIdParameter(string.Format("{0}/{1}/{2}\\{3}", new object[]
			{
				requestOwner.OrganizationId.OrganizationalUnit.Parent,
				requestOwner.OrganizationId.OrganizationalUnit.Name,
				requestOwner.Name,
				requestName
			}));
		}
	}
}
