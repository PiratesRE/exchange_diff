using System;
using System.Web;

namespace Microsoft.Exchange.Services.Core
{
	internal class ThrottlingLogInfo
	{
		private void InternalAddDataPoint(int throttleTime, float cpuPercent)
		{
			lock (this.lockObject)
			{
				double num = 1.0 - (double)this.sampleCount / ((double)this.sampleCount + 1.0);
				double num2 = 1.0 - num;
				this.averageCPUPercent = (float)((double)this.averageCPUPercent * num2 + (double)cpuPercent * num);
				this.averageThrottleTime = (int)((double)this.averageThrottleTime * num2 + (double)throttleTime * num);
				this.sampleCount++;
			}
		}

		public int AverageThrottleTime
		{
			get
			{
				return this.averageThrottleTime;
			}
		}

		public int AverageCPUPercent
		{
			get
			{
				return (int)this.averageCPUPercent;
			}
		}

		public static void AddDataPoint(int throttleTime, float cpuPercent)
		{
			HttpContext httpContext = EWSSettings.GetHttpContext();
			if (httpContext == null)
			{
				return;
			}
			ThrottlingLogInfo throttlingLogInfo;
			if (!ThrottlingLogInfo.TryGet(out throttlingLogInfo))
			{
				throttlingLogInfo = new ThrottlingLogInfo();
				httpContext.Items[ThrottlingLogInfo.Key] = throttlingLogInfo;
			}
			throttlingLogInfo.InternalAddDataPoint(throttleTime, cpuPercent);
		}

		public static bool TryGet(out ThrottlingLogInfo info)
		{
			if (HttpContext.Current == null)
			{
				info = null;
				return false;
			}
			info = (HttpContext.Current.Items[ThrottlingLogInfo.Key] as ThrottlingLogInfo);
			return info != null;
		}

		private static readonly string Key = "ThrottlingLogInfo";

		private int averageThrottleTime;

		private float averageCPUPercent;

		private int sampleCount;

		private object lockObject = new object();
	}
}
