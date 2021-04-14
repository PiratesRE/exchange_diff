using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PipelineResourceNetworkBound : PipelineResource
	{
		public PipelineResourceNetworkBound(int totalCount) : base(totalCount)
		{
			this.perServerResources = new Dictionary<string, int>(2);
		}

		public override bool TryAcquire(PipelineWorkItem workItem)
		{
			IUMNetworkResource iumnetworkResource = workItem.CurrentStage as IUMNetworkResource;
			ExAssert.RetailAssert(iumnetworkResource != null, "Stages asking for a Network resource must implement IUMNetworkResource");
			bool result;
			lock (this.lockObj)
			{
				int num;
				if (!this.perServerResources.TryGetValue(iumnetworkResource.NetworkResourceId, out num))
				{
					num = AppConfig.Instance.Service.MaxRPCThreadsPerServer;
					this.perServerResources.Add(iumnetworkResource.NetworkResourceId, num);
				}
				if (num > 0 && base.TryAcquire(workItem))
				{
					num = (this.perServerResources[iumnetworkResource.NetworkResourceId] = num - 1);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public override void Release(PipelineWorkItem workItem)
		{
			IUMNetworkResource iumnetworkResource = workItem.CurrentStage as IUMNetworkResource;
			ExAssert.RetailAssert(iumnetworkResource != null, "Stages asking for a Network resource must implement IUMNetworkResource");
			lock (this.lockObj)
			{
				Dictionary<string, int> dictionary;
				string networkResourceId;
				(dictionary = this.perServerResources)[networkResourceId = iumnetworkResource.NetworkResourceId] = dictionary[networkResourceId] + 1;
				base.Release(workItem);
			}
		}

		public const int MinimumNumberOfResources = 100;

		private Dictionary<string, int> perServerResources;

		private object lockObj = new object();
	}
}
