using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LocalDatabaseSoftDeletedCleanupRequest : BaseRequest
	{
		public LocalDatabaseSoftDeletedCleanupRequest(DirectoryIdentity databaseIdentity, ByteQuantifiedSize targetSize, LoadBalanceAnchorContext context)
		{
			this.databaseIdentity = databaseIdentity;
			this.targetSize = targetSize;
			this.context = context;
		}

		protected override void ProcessRequest()
		{
			if (this.context.Settings.SoftDeletedCleanupEnabled)
			{
				this.context.CleanupSoftDeletedMailboxesOnDatabase(this.databaseIdentity, this.targetSize);
			}
		}

		private readonly LoadBalanceAnchorContext context;

		private readonly DirectoryIdentity databaseIdentity;

		private readonly ByteQuantifiedSize targetSize;
	}
}
