using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedRemovalCapabilityDecorator : MissingCapabilityLoadBalanceClientDecorator
	{
		public SoftDeletedRemovalCapabilityDecorator(ILoadBalanceService service, DirectoryServer targetServer) : base(service, targetServer)
		{
		}

		public override void CleanupSoftDeletedMailboxesOnDatabase(DirectoryIdentity identity, ByteQuantifiedSize targetSize)
		{
		}

		public override SoftDeleteMailboxRemovalCheckRemoval CheckSoftDeletedMailboxRemoval(SoftDeletedRemovalData data)
		{
			return SoftDeleteMailboxRemovalCheckRemoval.DisallowRemoval("The target server '{0}' does not have the SoftDeletedRemoval capability so removal is not valid", new object[]
			{
				base.TargetServer.Name
			});
		}
	}
}
