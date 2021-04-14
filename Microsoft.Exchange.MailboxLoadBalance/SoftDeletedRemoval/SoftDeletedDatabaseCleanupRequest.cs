using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedDatabaseCleanupRequest : BaseRequest
	{
		public SoftDeletedDatabaseCleanupRequest(IClientFactory clientFactory, DirectoryDatabase directoryObject, ByteQuantifiedSize targetSize)
		{
			this.clientFactory = clientFactory;
			this.directoryObject = directoryObject;
			this.targetSize = targetSize;
		}

		protected override void ProcessRequest()
		{
			using (ILoadBalanceService loadBalanceClientForDatabase = this.clientFactory.GetLoadBalanceClientForDatabase(this.directoryObject))
			{
				loadBalanceClientForDatabase.CleanupSoftDeletedMailboxesOnDatabase(this.directoryObject.Identity, this.targetSize);
			}
		}

		private readonly IClientFactory clientFactory;

		private readonly DirectoryDatabase directoryObject;

		private readonly ByteQuantifiedSize targetSize;
	}
}
