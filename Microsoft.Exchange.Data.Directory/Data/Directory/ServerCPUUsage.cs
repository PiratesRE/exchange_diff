using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ServerCPUUsage
	{
		internal static uint GetCurrentUsagePercentage()
		{
			ServerCPUUsage.Refresh();
			return ServerCPUUsage.lastServerCPUUsagePercentage;
		}

		static ServerCPUUsage()
		{
			if (CPUUsage.GetCurrentCPU(ref ServerCPUUsage.lastServerCPUUsage))
			{
				ServerCPUUsage.lastUpdatedTime = DateTime.UtcNow;
				return;
			}
			ServerCPUUsage.lastServerCPUUsage = 0L;
			ServerCPUUsage.lastUpdatedTime = DateTime.MinValue;
		}

		private static void Refresh()
		{
			if (DateTime.UtcNow - ServerCPUUsage.lastUpdatedTime <= ServerCPUUsage.refreshInterval)
			{
				return;
			}
			lock (ServerCPUUsage.lockObject)
			{
				if (!(DateTime.UtcNow - ServerCPUUsage.lastUpdatedTime <= ServerCPUUsage.refreshInterval))
				{
					float num;
					if (CPUUsage.CalculateCPUUsagePercentage(ref ServerCPUUsage.lastUpdatedTime, ref ServerCPUUsage.lastServerCPUUsage, out num))
					{
						ServerCPUUsage.lastServerCPUUsagePercentage = (uint)Math.Round((double)num);
					}
				}
			}
		}

		private static readonly TimeSpan refreshInterval = TimeSpan.FromSeconds(1.0);

		private static object lockObject = new object();

		private static DateTime lastUpdatedTime;

		private static long lastServerCPUUsage;

		private static uint lastServerCPUUsagePercentage = 0U;
	}
}
