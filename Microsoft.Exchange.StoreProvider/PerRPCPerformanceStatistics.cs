using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct PerRPCPerformanceStatistics
	{
		public static PerRPCPerformanceStatistics CreateFromNative(uint validVersion, PerRpcStats nativeStats)
		{
			PerRPCPerformanceStatistics result = default(PerRPCPerformanceStatistics);
			result.validVersion = 0U;
			if (validVersion >= 1U)
			{
				result.timeInServer = new TimeSpan(0, 0, 0, 0, (int)nativeStats.cmsecInServer);
				result.timeInCPU = new TimeSpan(0, 0, 0, 0, (int)nativeStats.cmsecInCPU);
				result.pagesRead = nativeStats.ulPageRead;
				result.pagesPreread = nativeStats.ulPagePreread;
				result.logRecords = nativeStats.ulLogRecord;
				result.logBytes = nativeStats.ulcbLogRecord;
				result.ldapReads = nativeStats.ulLdapReads;
				result.ldapSearches = nativeStats.ulLdapSearches;
				result.avgDbLatency = nativeStats.avgDbLatency;
				result.avgServerLatency = nativeStats.avgServerLatency;
				result.currentThreads = nativeStats.currentThreads;
				result.validVersion = 1U;
			}
			if (validVersion >= 2U)
			{
				result.totalDbOperations = nativeStats.totalDbOperations;
				result.validVersion = 2U;
			}
			if (validVersion >= 3U)
			{
				result.currentDbThreads = nativeStats.currentDbThreads;
				result.currentSCTThreads = nativeStats.currentSCTThreads;
				result.currentSCTSessions = nativeStats.currentSCTSessions;
				result.processID = nativeStats.processID;
				result.validVersion = 3U;
			}
			if (validVersion >= 4U)
			{
				result.dataProtectionHealth = nativeStats.dataProtectionHealth;
				result.dataAvailabilityHealth = nativeStats.dataAvailabilityHealth;
				result.validVersion = 4U;
			}
			if (validVersion >= 5U)
			{
				result.currentCpuUsage = nativeStats.currentCpuUsage;
				result.validVersion = 5U;
			}
			return result;
		}

		public uint validVersion;

		public TimeSpan timeInServer;

		public TimeSpan timeInCPU;

		public uint pagesRead;

		public uint pagesPreread;

		public uint logRecords;

		public uint logBytes;

		public ulong ldapReads;

		public ulong ldapSearches;

		public uint avgDbLatency;

		public uint avgServerLatency;

		public uint currentThreads;

		public uint totalDbOperations;

		public uint currentDbThreads;

		public uint currentSCTThreads;

		public uint currentSCTSessions;

		public uint processID;

		public uint dataProtectionHealth;

		public uint dataAvailabilityHealth;

		public uint currentCpuUsage;
	}
}
