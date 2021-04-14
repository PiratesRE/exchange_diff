using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	[DataContract(Namespace = "")]
	internal class WindowJob
	{
		[DataMember(Order = 0)]
		public DateTime StartTime { get; set; }

		[DataMember(Order = 1)]
		public DateTime EndTime { get; set; }

		[DataMember(Order = 2)]
		public int TotalOnDatabaseMailboxCount { get; set; }

		[DataMember(Order = 3)]
		public int InterestingMailboxCount { get; set; }

		[DataMember(Order = 4)]
		public int NotInterestingMailboxCount { get; set; }

		[DataMember(Order = 5)]
		public int FilteredMailboxCount { get; set; }

		[DataMember(Order = 6)]
		public int FailedFilteringMailboxCount { get; set; }

		[DataMember(Order = 7)]
		public int CompletedMailboxCount { get; set; }

		[DataMember(Order = 8)]
		public int MovedToOnDemandMailboxCount { get; set; }

		[DataMember(Order = 9)]
		public int FailedMailboxCount { get; set; }

		[DataMember(Order = 10)]
		public int FailedToOpenStoreSessionCount { get; set; }

		[DataMember(Order = 11)]
		public int RetriedMailboxCount { get; set; }
	}
}
