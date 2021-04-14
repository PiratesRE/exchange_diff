using System;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	public enum ConsistencyCheckType
	{
		None,
		CanValidateOwnerCheck,
		MeetingExistenceCheck,
		ValidateStoreObjectCheck,
		MeetingCancellationCheck,
		AttendeeOnListCheck,
		CorrectResponseCheck,
		MeetingPropertiesMatchCheck,
		RecurrenceBlobsConsistentCheck,
		RecurrencesMatchCheck,
		TimeZoneMatchCheck
	}
}
