using System;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AspPerformanceData
	{
		public static PerformanceDataProvider GetStepData(RequestNotification notification)
		{
			if (AspPerformanceData.stepData == null)
			{
				AspPerformanceData.stepData = new Dictionary<RequestNotification, PerformanceDataProvider>(13);
			}
			PerformanceDataProvider performanceDataProvider;
			if (!AspPerformanceData.stepData.TryGetValue(notification, out performanceDataProvider))
			{
				performanceDataProvider = new PerformanceDataProvider(notification.ToString());
				AspPerformanceData.stepData[notification] = performanceDataProvider;
			}
			return performanceDataProvider;
		}

		public void StepStarted(RequestNotification notification)
		{
			this.StepCompleted();
			this.activeNotificationTimer = AspPerformanceData.GetStepData(notification).StartRequestTimer();
		}

		public void StepCompleted()
		{
			try
			{
				if (this.activeNotificationTimer != null)
				{
					this.activeNotificationTimer.Dispose();
				}
			}
			finally
			{
				this.activeNotificationTimer = null;
			}
		}

		[ThreadStatic]
		private static Dictionary<RequestNotification, PerformanceDataProvider> stepData;

		private IDisposable activeNotificationTimer;
	}
}
