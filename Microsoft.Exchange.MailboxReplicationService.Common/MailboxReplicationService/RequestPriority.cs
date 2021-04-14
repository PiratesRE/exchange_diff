using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public enum RequestPriority
	{
		[LocDescription(MrsStrings.IDs.RequestPriorityLowest)]
		Lowest = 20,
		[LocDescription(MrsStrings.IDs.RequestPriorityLower)]
		Lower = 30,
		[LocDescription(MrsStrings.IDs.RequestPriorityLow)]
		Low = 40,
		[LocDescription(MrsStrings.IDs.RequestPriorityNormal)]
		Normal = 50,
		[LocDescription(MrsStrings.IDs.RequestPriorityHigh)]
		High = 60,
		[LocDescription(MrsStrings.IDs.RequestPriorityHigher)]
		Higher = 70,
		[LocDescription(MrsStrings.IDs.RequestPriorityHighest)]
		Highest = 80,
		[LocDescription(MrsStrings.IDs.RequestPriorityEmergency)]
		Emergency = 100
	}
}
