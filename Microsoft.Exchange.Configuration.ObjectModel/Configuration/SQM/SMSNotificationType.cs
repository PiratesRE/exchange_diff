using System;

namespace Microsoft.Exchange.Configuration.SQM
{
	public enum SMSNotificationType
	{
		None,
		Email = 10,
		VoiceMail = 20,
		VoiceMailAndMissedCalls,
		CalendarUpdate = 30,
		CalendarReminder,
		CalendarAgenda,
		System = 40
	}
}
