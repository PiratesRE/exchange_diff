using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Server.Storage.DirectoryServices
{
	public static class CpuUsage
	{
		public static uint GetCurrentUsagePercentage()
		{
			return ServerCPUUsage.GetCurrentUsagePercentage();
		}
	}
}
