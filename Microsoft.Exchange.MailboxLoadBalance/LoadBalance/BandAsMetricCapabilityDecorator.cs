using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BandAsMetricCapabilityDecorator : MissingCapabilityLoadBalanceClientDecorator
	{
		public BandAsMetricCapabilityDecorator(ILoadBalanceService service, LoadBalanceAnchorContext serviceContext, DirectoryServer targetServer) : base(service, targetServer)
		{
			AnchorUtil.ThrowOnNullArgument(serviceContext, "serviceContext");
			this.serviceContext = serviceContext;
		}

		public override DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryDatabase database)
		{
			DatabaseSizeInfo databaseSpaceData;
			using (IPhysicalDatabase physicalDatabaseConnection = this.serviceContext.ClientFactory.GetPhysicalDatabaseConnection(database))
			{
				databaseSpaceData = physicalDatabaseConnection.GetDatabaseSpaceData();
			}
			return databaseSpaceData;
		}

		public override LoadContainer GetLocalServerData(Band[] bands)
		{
			IList<Guid> nonMovableOrgsList = LoadBalanceUtils.GetNonMovableOrgsList(this.serviceContext.Settings);
			ILogger logger = this.serviceContext.Logger;
			TopologyExtractorFactoryContext context = this.serviceContext.TopologyExtractorFactoryContextPool.GetContext(this.serviceContext.ClientFactory, bands, nonMovableOrgsList, logger);
			TopologyExtractorFactory loadBalancingLocalFactory = context.GetLoadBalancingLocalFactory(true);
			return loadBalancingLocalFactory.GetExtractor(base.TargetServer).ExtractTopology();
		}

		private readonly LoadBalanceAnchorContext serviceContext;
	}
}
