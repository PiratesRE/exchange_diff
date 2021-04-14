using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BackCompatibleLoadBalanceClient : MissingCapabilityLoadBalanceClientDecorator
	{
		public BackCompatibleLoadBalanceClient(MailboxLoadBalanceService serviceImpl, DirectoryServer targetServer) : base(null, targetServer)
		{
			this.serviceImpl = serviceImpl;
		}

		public override void BeginMailboxMove(BandMailboxRebalanceData rebalanceData, LoadMetric metric)
		{
			this.serviceImpl.MoveMailboxes(rebalanceData);
		}

		public override void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			serverVersion = LoadBalancerVersionInformation.LoadBalancerVersion;
		}

		private readonly MailboxLoadBalanceService serviceImpl;
	}
}
