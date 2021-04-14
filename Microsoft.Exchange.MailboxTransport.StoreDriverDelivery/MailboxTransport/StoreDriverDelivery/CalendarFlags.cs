using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	[Flags]
	internal enum CalendarFlags
	{
		None = 0,
		AutoBooking = 1,
		CalendarAssistantActive = 2,
		CalendarAssistantNoiseReduction = 4,
		CalendarAssistantAddNewItems = 8,
		CalendarAssistantProcessExternal = 16,
		SkipProcessing = 32,
		CalAssistantDefaults = 14
	}
}
