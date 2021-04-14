using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LocalServerHeatMapConstructionRequest : HeatMapConstructionRequest
	{
		public LocalServerHeatMapConstructionRequest(LoadBalanceAnchorContext context) : base(context)
		{
		}

		protected override LoadContainer BuildTopology(TopologyExtractorFactoryContext topologyExtractorContext)
		{
			TopologyExtractorFactory loadBalancingLocalFactory = topologyExtractorContext.GetLoadBalancingLocalFactory(false);
			DirectoryServer localServer = base.ServiceContext.Directory.GetLocalServer();
			TopologyExtractor extractor = loadBalancingLocalFactory.GetExtractor(localServer);
			LoadContainer loadContainer = extractor.ExtractTopology();
			ExAssert.RetailAssert(loadContainer != null, "Extracted toplogy for server '{0}' should never be null.", new object[]
			{
				localServer
			});
			DatabaseCollector databaseCollector = new DatabaseCollector();
			loadContainer.Accept(databaseCollector);
			IOperationRetryManager operationRetryManager = LoadBalanceOperationRetryManager.Create(1, TimeSpan.Zero, base.ServiceContext.Logger);
			foreach (LoadContainer loadContainer2 in databaseCollector.Databases)
			{
				DirectoryDatabase directoryDatabase = loadContainer2.DirectoryObject as DirectoryDatabase;
				if (directoryDatabase != null)
				{
					DatabaseProcessor @object = new DatabaseProcessor(base.ServiceContext.Settings, base.ServiceContext.DrainControl, base.ServiceContext.Logger, directoryDatabase);
					operationRetryManager.TryRun(new Action(@object.ProcessDatabase));
				}
			}
			return loadContainer;
		}
	}
}
