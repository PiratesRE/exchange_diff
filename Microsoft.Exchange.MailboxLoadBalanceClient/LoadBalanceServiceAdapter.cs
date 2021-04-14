using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.Provisioning;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalanceClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceServiceAdapter : ILoadBalanceServicePort
	{
		protected LoadBalanceServiceAdapter(ILogger logger)
		{
			this.logger = logger;
		}

		protected virtual ByteQuantifiedSize AverageMailboxSize
		{
			get
			{
				return ByteQuantifiedSize.FromMB(100UL);
			}
		}

		public static ILoadBalanceServicePort Create(ILogger logger)
		{
			VersionedClientBase<ILoadBalanceService>.UseUpdatedBinding = true;
			return new LoadBalanceServiceAdapter(logger);
		}

		public virtual BatchCapacityDatum GetBatchCapacityForForest(int numberOfMailboxes)
		{
			return this.GetBatchCapacityForForest(numberOfMailboxes, ByteQuantifiedSize.FromMB((ulong)((long)numberOfMailboxes * (long)this.AverageMailboxSize.ToMB())));
		}

		public virtual BatchCapacityDatum GetBatchCapacityForForest(int numberOfMailboxes, ByteQuantifiedSize expectedBatchSize)
		{
			Func<ILoadBalanceService, BatchCapacityDatum> func = (ILoadBalanceService service) => service.GetConsumerBatchCapacity(numberOfMailboxes, expectedBatchSize);
			return this.CallClientFunction<BatchCapacityDatum>(func);
		}

		public CapacitySummary GetCapacitySummary(DirectoryIdentity objectIdentity, bool refreshData)
		{
			Func<ILoadBalanceService, HeatMapCapacityData> func = (ILoadBalanceService service) => service.GetCapacitySummary(objectIdentity, refreshData);
			HeatMapCapacityData capacityDatum = this.CallClientFunction<HeatMapCapacityData>(func);
			return CapacitySummary.FromDatum(capacityDatum);
		}

		public ADObjectId GetDatabaseForNewConsumerMailbox()
		{
			MailboxProvisioningResult provisioningResult = null;
			this.CallClientFunction<MailboxProvisioningResult>((ILoadBalanceService svc) => provisioningResult = svc.GetLocalDatabaseForProvisioning(new MailboxProvisioningData(ByteQuantifiedSize.Zero, null, null)));
			if (provisioningResult == null)
			{
				return null;
			}
			provisioningResult.ValidateSelection();
			return provisioningResult.Database.ADObjectId;
		}

		protected T CallClientFunction<T>(Func<ILoadBalanceService, T> func)
		{
			Server server = LocalServer.GetServer();
			T result;
			using (LoadBalancerClient loadBalancerClient = LoadBalancerClient.Create(server.Name, NullDirectory.Instance, this.logger))
			{
				this.logger.Log(MigrationEventType.Verbose, "Making WCF call to load balancer", new object[0]);
				result = func(loadBalancerClient);
			}
			return result;
		}

		private readonly ILogger logger;
	}
}
