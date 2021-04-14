using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct PerRpcStats
	{
		internal uint cmsecInServer;

		internal uint cmsecInCPU;

		internal uint ulPageRead;

		internal uint ulPagePreread;

		internal uint ulLogRecord;

		internal uint ulcbLogRecord;

		internal ulong ulLdapReads;

		internal ulong ulLdapSearches;

		internal uint avgDbLatency;

		internal uint avgServerLatency;

		internal uint currentThreads;

		internal uint totalDbOperations;

		internal uint currentDbThreads;

		internal uint currentSCTThreads;

		internal uint currentSCTSessions;

		internal uint processID;

		internal uint dataProtectionHealth;

		internal uint dataAvailabilityHealth;

		internal uint currentCpuUsage;
	}
}
