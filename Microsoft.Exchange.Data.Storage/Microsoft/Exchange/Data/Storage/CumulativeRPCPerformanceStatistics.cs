using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct CumulativeRPCPerformanceStatistics
	{
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
