using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class AdmissionClassificationData
	{
		public AdmissionClassificationData(WorkloadClassification classification, string id)
		{
			if (classification == WorkloadClassification.Unknown)
			{
				throw new ArgumentException("WorkloadClassification.Unknown is not acceptable value for the parameter.", "classification");
			}
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Data owner id should be non null or white space", "id");
			}
			this.Classification = classification;
			this.Id = id;
			this.AvailableAtLastStatusChange = true;
			this.FilledDuringThisCycle = true;
			this.ConcurrencyLimits = 0;
		}

		public string Id { get; private set; }

		public WorkloadClassification Classification { get; private set; }

		public int ConcurrencyLimits { get; private set; }

		public int ConcurrencyUsed { get; private set; }

		public double DelayFactor { get; private set; }

		public bool AvailableAtLastStatusChange { get; private set; }

		public bool FilledDuringThisCycle { get; private set; }

		public bool TryAquireSlot(out double delayFactor)
		{
			int num = this.ConcurrencyLimits - this.ConcurrencyUsed;
			if (num <= 0)
			{
				this.FilledDuringThisCycle = true;
				delayFactor = 0.0;
				return false;
			}
			this.ConcurrencyUsed++;
			if (num == 1)
			{
				this.FilledDuringThisCycle = true;
			}
			delayFactor = this.DelayFactor;
			return true;
		}

		public void ReleaseSlot()
		{
			this.ConcurrencyUsed--;
		}

		public void Refresh(ResourceKey resource, ResourceLoad load)
		{
			int num = this.ConcurrencyLimits;
			double num2 = 0.0;
			if (!this.FilledDuringThisCycle)
			{
				this.FilledDuringThisCycle = (this.ConcurrencyUsed >= this.ConcurrencyLimits);
			}
			switch (load.State)
			{
			case ResourceLoadState.Unknown:
				if (this.ConcurrencyLimits == 0)
				{
					num = 1;
					num2 = 0.0;
				}
				ExTraceGlobals.AdmissionControlTracer.TraceDebug<ResourceKey, int, double>((long)this.GetHashCode(), "[AdmissionClassificationData.Refresh] Resource load for {0} is unknown. Setting new concurrency to {1} and delay factor to {2}.", resource, num, num2);
				break;
			case ResourceLoadState.Underloaded:
				if (this.FilledDuringThisCycle)
				{
					if (this.DelayFactor > 0.0)
					{
						num2 = 0.0;
					}
					else
					{
						num = this.ConcurrencyLimits + 1;
					}
					ExTraceGlobals.AdmissionControlTracer.TraceDebug((long)this.GetHashCode(), "[AdmissionClassificationData.Refresh] (Underloaded, Resource: {0}, Classification: {1}) All slots were filled in this cycle so updating delay factor to {2} and newConcurrency to {3}.", new object[]
					{
						resource,
						this.Classification,
						num2,
						num
					});
				}
				else
				{
					ExTraceGlobals.AdmissionControlTracer.TraceDebug<ResourceKey, WorkloadClassification>((long)this.GetHashCode(), "[AdmissionClassificationData.Refresh] (Underloaded, Resource: {0}, Classification: {1}) Slots were NOT filled in this cycle, so we will NOT update the delay factor nor concurrency.", resource, this.Classification);
				}
				break;
			case ResourceLoadState.Full:
				if (this.ConcurrencyLimits < 1)
				{
					num = 1;
				}
				break;
			case ResourceLoadState.Overloaded:
				if (this.ConcurrencyLimits > 1)
				{
					num = (int)((double)this.ConcurrencyLimits / load.LoadRatio);
					if (num == this.ConcurrencyLimits && num > 0)
					{
						num--;
					}
					if (num <= 0)
					{
						num = 1;
					}
					ExTraceGlobals.AdmissionControlTracer.TraceDebug<ResourceKey, WorkloadClassification, int>((long)this.GetHashCode(), "[AdmissionClassificationData.Refresh] Resource {0} is overloaded for classification: {1}. Setting new concurrency to {2}.", resource, this.Classification, num);
				}
				else if (this.ConcurrencyLimits == 1)
				{
					num = 1;
					num2 = load.LoadRatio - 1.0;
					ExTraceGlobals.AdmissionControlTracer.TraceDebug<ResourceKey, WorkloadClassification, double>((long)this.GetHashCode(), "[AdmissionClassificationData.Refresh] Resource {0} is overloaded for classification {1}. Run one thread with delay factor {2}.", resource, this.Classification, num2);
				}
				break;
			case ResourceLoadState.Critical:
				num = 0;
				num2 = 0.0;
				ExTraceGlobals.AdmissionControlTracer.TraceDebug<ResourceKey, WorkloadClassification, int>((long)this.GetHashCode(), "[AdmissionClassificationData.Refresh] Resource {0} is critical for Classification {1}. Setting new concurrency to {2}.", resource, this.Classification, num);
				break;
			}
			this.FilledDuringThisCycle = (num == 0);
			this.ConcurrencyLimits = num;
			this.DelayFactor = num2;
		}

		public bool RefreshAvailable()
		{
			int num = this.ConcurrencyLimits - this.ConcurrencyUsed;
			bool flag = num > 0;
			bool availableAtLastStatusChange = this.AvailableAtLastStatusChange;
			this.AvailableAtLastStatusChange = flag;
			return availableAtLastStatusChange != flag;
		}
	}
}
