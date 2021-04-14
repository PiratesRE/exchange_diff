using System;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	[Flags]
	public enum ParameterId
	{
		Unknown = 1,
		AlternateRepresentation = 2,
		CommonName = 4,
		CalendarUserType = 8,
		Delegator = 16,
		Delegatee = 32,
		Directory = 64,
		Encoding = 128,
		FormatType = 256,
		FreeBusyType = 512,
		Language = 1024,
		Membership = 2048,
		ParticipationStatus = 4096,
		RecurrenceRange = 8192,
		TriggerRelationship = 16384,
		RelationshipType = 32768,
		ParticipationRole = 65536,
		RsvpExpectation = 131072,
		SentBy = 262144,
		TimeZoneId = 524288,
		ValueType = 1048576
	}
}
