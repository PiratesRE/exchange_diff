using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal struct WLMResourceStatsData
	{
		public string OwnerResourceName { get; set; }

		public Guid OwnerResourceGuid { get; set; }

		public string OwnerResourceType { get; set; }

		public string WlmResourceKey { get; set; }

		public string LoadState { get; set; }

		public double LoadRatio { get; set; }

		public string Metric { get; set; }

		public double DynamicCapacity { get; set; }

		public string IsDisabled { get; set; }

		public string DynamicThrottingDisabled { get; set; }

		public TimeSpan TimeInterval { get; set; }

		public uint UnderloadedCount { get; set; }

		public uint FullCount { get; set; }

		public uint OverloadedCount { get; set; }

		public uint CriticalCount { get; set; }

		public uint UnknownCount { get; set; }
	}
}
