using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;

namespace Microsoft.Exchange.MailboxLoadBalance.QueueProcessing
{
	[DataContract]
	internal class ForestLoadBalanceRequest : BaseRequest
	{
		public ForestLoadBalanceRequest(ILoadBalance loadBalancer, bool startMoves, ILogger logger, IRebalancingRequestProcessor moveStarter, LoadContainer forestTopology, PartitionExtractor partitionExtractor)
		{
			AnchorUtil.ThrowOnNullArgument(loadBalancer, "loadBalancer");
			AnchorUtil.ThrowOnNullArgument(logger, "logger");
			this.loadBalancer = loadBalancer;
			this.startMoves = startMoves;
			this.logger = logger;
			this.moveStarter = moveStarter;
			this.forestTopology = forestTopology;
			this.partitionExtractor = partitionExtractor;
		}

		[DataMember]
		public IList<BandMailboxRebalanceData> Results { get; private set; }

		protected override void ProcessRequest()
		{
			string text = string.Format("MsExchMlb:Band:{0:yyyyMMddhhmm}", ExDateTime.UtcNow);
			using (OperationTracker.Create(this.logger, "Rebalancing forest with StartMoves={0}, using LoadBalancer={1}, BatchName={2}", new object[]
			{
				this.startMoves,
				this.loadBalancer,
				text
			}))
			{
				List<BandMailboxRebalanceData> list = new List<BandMailboxRebalanceData>();
				foreach (LoadPartition loadPartition in this.partitionExtractor.GetPartitions(this.forestTopology))
				{
					this.logger.LogInformation("Load balancing partition for MailboxProvisioningConstraint \"{0}\"", new object[]
					{
						loadPartition.ConstraintSetIdentity
					});
					IEnumerable<BandMailboxRebalanceData> enumerable = this.loadBalancer.BalanceForest(loadPartition.Root);
					foreach (BandMailboxRebalanceData bandMailboxRebalanceData in enumerable)
					{
						bandMailboxRebalanceData.RebalanceBatchName = text;
						bandMailboxRebalanceData.ConstraintSetIdentity = loadPartition.ConstraintSetIdentity;
						list.Add(bandMailboxRebalanceData);
						if (this.startMoves)
						{
							this.moveStarter.ProcessRebalanceRequest(bandMailboxRebalanceData);
						}
					}
				}
				this.Results = list;
			}
		}

		[DataMember]
		private readonly ILoadBalance loadBalancer;

		[IgnoreDataMember]
		private readonly ILogger logger;

		[IgnoreDataMember]
		private readonly IRebalancingRequestProcessor moveStarter;

		[IgnoreDataMember]
		private readonly LoadContainer forestTopology;

		[DataMember]
		private readonly bool startMoves;

		[IgnoreDataMember]
		private readonly PartitionExtractor partitionExtractor;
	}
}
