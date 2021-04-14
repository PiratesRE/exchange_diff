using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PipelineResource
	{
		protected PipelineResource(int totalCount)
		{
			this.totalCount = totalCount;
			this.numberOfResourcesRemaining = totalCount;
			this.acquisitionTimes = new Dictionary<Guid, ExDateTime>(this.totalCount);
		}

		public static PipelineResource CreatePipelineResource(PipelineDispatcher.PipelineResourceType resourceType)
		{
			int processorCount = Environment.ProcessorCount;
			switch (resourceType)
			{
			case PipelineDispatcher.PipelineResourceType.LowPriorityCpuBound:
				return new PipelineResource(processorCount * AppConfig.Instance.Service.PipelineScaleFactorCPU);
			case PipelineDispatcher.PipelineResourceType.CpuBound:
				return new PipelineResource(processorCount * AppConfig.Instance.Service.PipelineScaleFactorCPU);
			case PipelineDispatcher.PipelineResourceType.NetworkBound:
				return new PipelineResourceNetworkBound(Math.Max(100, processorCount * AppConfig.Instance.Service.PipelineScaleFactorNetworkBound));
			default:
				throw new InvalidOperationException("Unknown PipelineResourceType.");
			}
		}

		public int TotalCount
		{
			get
			{
				return this.totalCount;
			}
		}

		public virtual bool TryAcquire(PipelineWorkItem workItem)
		{
			bool result;
			lock (this.lockObj)
			{
				if (this.numberOfResourcesRemaining > 0)
				{
					this.numberOfResourcesRemaining--;
					this.acquisitionTimes[workItem.WorkId] = ExDateTime.UtcNow;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public virtual void Release(PipelineWorkItem workItem)
		{
			lock (this.lockObj)
			{
				this.numberOfResourcesRemaining++;
				ExAssert.RetailAssert(this.acquisitionTimes.ContainsKey(workItem.WorkId), "Request to release resource for workitem={0} that doesn't have that resource!", new object[]
				{
					workItem.WorkId
				});
				this.acquisitionTimes.Remove(workItem.WorkId);
			}
		}

		private Dictionary<Guid, ExDateTime> acquisitionTimes;

		private int numberOfResourcesRemaining;

		private int totalCount;

		private object lockObj = new object();
	}
}
