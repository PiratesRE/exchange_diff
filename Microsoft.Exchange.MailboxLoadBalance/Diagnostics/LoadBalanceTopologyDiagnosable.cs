using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceTopologyDiagnosable : LoadBalanceDiagnosableBase<LoadBalanceTopologyDiagnosableArgument, LoadContainer>
	{
		public LoadBalanceTopologyDiagnosable(LoadBalanceAnchorContext loadBalanceContext) : base(loadBalanceContext.Logger)
		{
			this.context = loadBalanceContext;
		}

		protected Band[] Bands
		{
			get
			{
				if (this.bands == null)
				{
					using (ILoadBalanceService loadBalanceClientForCentralServer = this.context.ClientFactory.GetLoadBalanceClientForCentralServer())
					{
						this.bands = loadBalanceClientForCentralServer.GetActiveBands();
					}
				}
				return this.bands;
			}
		}

		protected override LoadContainer ProcessDiagnostic()
		{
			if (base.Arguments.ShowDatabase)
			{
				return this.GetDatabaseContainer();
			}
			if (base.Arguments.ShowServer)
			{
				return this.GetServerContainer();
			}
			if (base.Arguments.ShowDag)
			{
				return this.GetDagContainer();
			}
			if (base.Arguments.ShowForest)
			{
				return this.GetForestContainer();
			}
			if (base.Arguments.ShowForestHeatMap)
			{
				return this.context.HeatMap.GetLoadTopology();
			}
			if (base.Arguments.ShowLocalServerHeatMap)
			{
				return this.context.LocalServerHeatMap.GetLoadTopology();
			}
			return null;
		}

		private LoadContainer GetDagContainer()
		{
			DirectoryDatabaseAvailabilityGroup directoryDatabaseAvailabilityGroup = this.context.Directory.GetDatabaseAvailabilityGroups().FirstOrDefault((DirectoryDatabaseAvailabilityGroup d) => d.Guid == base.Arguments.DagGuid);
			if (directoryDatabaseAvailabilityGroup == null)
			{
				throw new DagNotFoundException(base.Arguments.DagGuid.ToString());
			}
			return this.GetTopologyForDirectoryObject(directoryDatabaseAvailabilityGroup);
		}

		private LoadContainer GetDatabaseContainer()
		{
			DirectoryDatabase database = this.context.Directory.GetDatabase(base.Arguments.DatabaseGuid);
			if (database == null)
			{
				throw new DatabaseNotFoundPermanentException(base.Arguments.DatabaseGuid.ToString());
			}
			return this.GetTopologyForDirectoryObject(database);
		}

		private TopologyExtractorFactory GetExtractorFactory()
		{
			TopologyExtractorFactoryContext topologyExtractorFactoryContext = this.context.TopologyExtractorFactoryContextPool.GetContext(this.context.ClientFactory, this.Bands, Array<Guid>.Empty, base.Logger);
			if (!base.Arguments.Verbose)
			{
				return topologyExtractorFactoryContext.GetLoadBalancingCentralFactory();
			}
			return topologyExtractorFactoryContext.GetEntitySelectorFactory();
		}

		private LoadContainer GetForestContainer()
		{
			DirectoryForest localForest = this.context.Directory.GetLocalForest();
			return this.GetTopologyForDirectoryObject(localForest);
		}

		private LoadContainer GetServerContainer()
		{
			DirectoryServer server = this.context.Directory.GetServer(base.Arguments.ServerGuid);
			if (server == null)
			{
				throw new ServerNotFoundException(base.Arguments.ServerGuid.ToString());
			}
			return this.GetTopologyForDirectoryObject(server);
		}

		private LoadContainer GetTopologyForDirectoryObject(DirectoryObject directoryObject)
		{
			return this.GetExtractorFactory().GetExtractor(directoryObject).ExtractTopology();
		}

		private readonly LoadBalanceAnchorContext context;

		private Band[] bands;
	}
}
