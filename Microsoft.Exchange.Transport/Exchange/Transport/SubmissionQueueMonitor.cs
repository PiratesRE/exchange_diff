using System;

namespace Microsoft.Exchange.Transport
{
	internal sealed class SubmissionQueueMonitor : ResourceMonitor
	{
		public SubmissionQueueMonitor(ResourceManagerConfiguration.ResourceMonitorConfiguration configuration) : base(string.Empty, configuration)
		{
		}

		public override string ToString(ResourceUses resourceUses, int currentPressure)
		{
			return Strings.SubmissionQueueUses(currentPressure, ResourceManager.MapToLocalizedString(resourceUses), base.LowPressureLimit, base.MediumPressureLimit, base.HighPressureLimit);
		}

		protected override bool GetCurrentReading(out int currentReading)
		{
			SubmitMessageQueue submitMessageQueue = Components.CategorizerComponent.SubmitMessageQueue;
			if (submitMessageQueue != null && !submitMessageQueue.Suspended)
			{
				currentReading = submitMessageQueue.ActiveCountExcludingPriorityNone;
			}
			else
			{
				currentReading = 0;
			}
			return true;
		}
	}
}
