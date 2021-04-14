using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	internal class PerformanceCounterRestartResponderChecker : RestartResponderChecker
	{
		internal PerformanceCounterRestartResponderChecker(ResponderDefinition definition, string categoryName, string counterName, string instanceName, int minThreshold, int maxThreshold, string reasonToSkip) : base(definition)
		{
			this.categoryName = categoryName;
			this.counterName = counterName;
			this.instanceName = instanceName;
			this.minThreshold = minThreshold;
			this.maxThreshold = maxThreshold;
			this.reasonToSkip = reasonToSkip;
		}

		protected override bool IsWithinThreshold()
		{
			this.skipReasonOrException = null;
			try
			{
				float performanceCounterValue = this.GetPerformanceCounterValue();
				if (performanceCounterValue > (float)this.maxThreshold || performanceCounterValue < (float)this.minThreshold)
				{
					this.skipReasonOrException = string.Format("{0}. Real value = {1}, Threshold = [{2}, {3}]", new object[]
					{
						this.reasonToSkip,
						performanceCounterValue,
						this.minThreshold,
						this.maxThreshold
					});
					return false;
				}
			}
			catch (Exception ex)
			{
				this.skipReasonOrException = ex.ToString();
			}
			return true;
		}

		internal override string SkipReasonOrException
		{
			get
			{
				return this.skipReasonOrException;
			}
		}

		protected override bool Enabled
		{
			get
			{
				return true;
			}
		}

		protected override bool CheckChangedSetting()
		{
			return true;
		}

		private float GetPerformanceCounterValue()
		{
			float num = 0f;
			using (PerformanceCounter performanceCounter = new PerformanceCounter(this.categoryName, this.counterName, this.instanceName, true))
			{
				num = performanceCounter.NextValue();
				int num2 = 0;
				while (num2 < 3 && num <= 0f)
				{
					Thread.Sleep(500);
					num = performanceCounter.NextValue();
					num2++;
				}
			}
			return num;
		}

		private const int RetryCounter = 3;

		private string reasonToSkip;

		private int minThreshold;

		private int maxThreshold;

		private string categoryName;

		private string counterName;

		private string instanceName;

		private string skipReasonOrException;
	}
}
