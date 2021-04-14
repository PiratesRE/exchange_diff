using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class MwiTargetBase : IMwiTarget, IRpcTarget
	{
		protected MwiTargetBase(ADConfigurationObject configObject, string instanceNameSuffix)
		{
			this.configObject = configObject;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.UM.UMDataCenterLogging.Enabled)
			{
				this.perfCounters = null;
				return;
			}
			this.InitializePerfCounters(instanceNameSuffix);
		}

		public string Name
		{
			get
			{
				return this.ConfigObject.Name;
			}
		}

		public ADConfigurationObject ConfigObject
		{
			get
			{
				return this.configObject;
			}
		}

		protected long AverageProcessingTimeMsec
		{
			get
			{
				return this.averageProcessingTime.Value;
			}
		}

		public virtual void SendMessageAsync(MwiMessage message)
		{
			if (this.perfCounters != null)
			{
				MwiDiagnostics.IncrementCounter(this.perfCounters.TotalMwiMessages);
			}
		}

		public override bool Equals(object target)
		{
			MwiTargetBase mwiTargetBase = target as MwiTargetBase;
			return mwiTargetBase != null && this.ConfigObject.Guid.Equals(mwiTargetBase.ConfigObject.Guid);
		}

		public override int GetHashCode()
		{
			if (this.ConfigObject.Id == null)
			{
				return base.GetHashCode();
			}
			return this.ConfigObject.Id.GetHashCode();
		}

		protected void UpdatePerformanceCounters(MwiMessage message, MwiDeliveryException error)
		{
			if (this.perfCounters != null)
			{
				TimeSpan timeSpan = ExDateTime.UtcNow.Subtract(message.SentTimeUtc);
				MwiDiagnostics.SetCounterValue(this.perfCounters.AverageMwiProcessingTime, this.averageProcessingTime.Update(timeSpan.TotalMilliseconds));
				if (error != null)
				{
					MwiDiagnostics.IncrementCounter(this.perfCounters.TotalFailedMwiMessages);
				}
			}
		}

		private void InitializePerfCounters(string instanceNameSuffix)
		{
			string instanceName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				this.Name,
				instanceNameSuffix
			});
			this.perfCounters = MwiDiagnostics.GetInstance(instanceName);
			this.perfCounters.Reset();
		}

		private MwiLoadBalancerPerformanceCountersInstance perfCounters;

		private MovingAverage averageProcessingTime = new MovingAverage(50);

		private ADConfigurationObject configObject;
	}
}
