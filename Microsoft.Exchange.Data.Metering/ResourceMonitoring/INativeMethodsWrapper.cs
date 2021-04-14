using System;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal interface INativeMethodsWrapper
	{
		bool GetDiskFreeSpaceEx(string directoryName, out ulong freeBytesAvailable, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes);

		bool GetSystemMemoryUsePercentage(out uint systemMemoryUsage);

		bool GetTotalSystemMemory(out ulong systemMemory);

		bool GetTotalVirtualMemory(out ulong virtualMemory);

		bool GetProcessPrivateBytes(out ulong privateBytes);
	}
}
