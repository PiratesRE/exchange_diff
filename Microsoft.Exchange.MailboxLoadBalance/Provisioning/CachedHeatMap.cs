using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CachedHeatMap : HeatMap
	{
		public CachedHeatMap(LoadBalanceAnchorContext context, HeatMapConstructionRequest heatMapConstructor)
		{
			this.heatMapRequest = heatMapConstructor;
			ScheduledRequest request = new ScheduledRequest(this.heatMapRequest, TimeProvider.UtcNow, () => context.Settings.LocalCacheRefreshPeriod);
			context.QueueManager.MainProcessingQueue.EnqueueRequest(request);
		}

		public override bool IsReady
		{
			get
			{
				return this.heatMapRequest.Topology != null;
			}
		}

		public override LoadContainer GetLoadTopology()
		{
			while (this.heatMapRequest.Topology == null)
			{
				this.heatMapRequest.WaitExecutionAndThrowOnFailure(TimeSpan.FromMinutes(5.0));
			}
			return this.heatMapRequest.Topology;
		}

		public override void UpdateBands(Band[] bands)
		{
			this.heatMapRequest.UpdateBands(bands);
		}

		private readonly HeatMapConstructionRequest heatMapRequest;
	}
}
