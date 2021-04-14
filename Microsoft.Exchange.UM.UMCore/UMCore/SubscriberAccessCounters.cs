using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal static class SubscriberAccessCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (SubscriberAccessCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in SubscriberAccessCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchangeUMSubscriberAccess";

		public static readonly ExPerformanceCounter SubscriberLogons = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Subscriber Logons", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SubscriberAuthenticationFailures = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Subscriber Authentication Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter SubscriberLogonFailures = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Subscriber Logon Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSubscriberCallDuration = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Average Subscriber Call Duration", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRecentSubscriberCallDuration = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Average Recent Subscriber Call Duration", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter VoiceMessageQueueAccessed = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Voice Message Queue Accessed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter VoiceMessagesHeard = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Voice Messages Heard", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProtectedVoiceMessagesHeard = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Protected Voice Messages Heard", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter VoiceMessagesSent = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Voice Messages Sent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ProtectedVoiceMessagesSent = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Protected Voice Messages Sent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter VoiceMessageProtectionFailures = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Voice Message Protection Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter VoiceMessageDecryptionFailures = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Voice Message Decryption Failures", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageSentVoiceMessageSize = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Average Sent Voice Message Size", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter AverageRecentSentVoiceMessageSize = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Average Recent Sent Voice Message Size", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter VoiceMessagesDeleted = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Voice Messages Deleted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ReplyMessagesSent = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Reply Messages Sent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ForwardMessagesSent = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Forward Messages Sent", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EmailMessageQueueAccessed = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Email Message Queue Accessed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EmailMessagesHeard = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Email Messages Heard", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter EmailMessagesDeleted = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Email Messages Deleted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CalendarAccessed = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Calendar Accessed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CalendarItemsHeard = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Calendar Items Heard", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CalendarLateAttendance = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Calendar Late Attendance", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CalendarItemsDetailsRequested = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Calendar Items Details Requested", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MeetingsDeclined = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Meetings Declined", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter MeetingsAccepted = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Meetings Accepted", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CalledMeetingOrganizer = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Called Meeting Organizer", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RepliedToOrganizer = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Replied to Organizer", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ContactsAccessed = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Contacts Accessed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ContactItemsHeard = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Contact Items Heard", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsDisconnectedByCallersDuringUMAudioHourglass = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Calls Disconnected by Callers During UM Audio Hourglass", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter LaunchedCalls = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Launched Calls", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DirectoryAccessed = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Directory Accessed", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DirectoryAccessedByExtension = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Directory Accessed by Extension", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DirectoryAccessedByDialByName = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Directory Accessed by Dial by Name", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DirectoryAccessedSuccessfullyByDialByName = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Directory Accessed Successfully by Dial by Name", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DirectoryAccessedBySpokenName = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Directory Accessed by Spoken Name", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter DirectoryAccessedSuccessfullyBySpokenName = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Directory Accessed Successfully by Spoken Name", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CallsDisconnectedOnIrrecoverableExternalError = new ExPerformanceCounter("MSExchangeUMSubscriberAccess", "Calls Disconnected by UM on Irrecoverable External Error", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			SubscriberAccessCounters.SubscriberLogons,
			SubscriberAccessCounters.SubscriberAuthenticationFailures,
			SubscriberAccessCounters.SubscriberLogonFailures,
			SubscriberAccessCounters.AverageSubscriberCallDuration,
			SubscriberAccessCounters.AverageRecentSubscriberCallDuration,
			SubscriberAccessCounters.VoiceMessageQueueAccessed,
			SubscriberAccessCounters.VoiceMessagesHeard,
			SubscriberAccessCounters.ProtectedVoiceMessagesHeard,
			SubscriberAccessCounters.VoiceMessagesSent,
			SubscriberAccessCounters.ProtectedVoiceMessagesSent,
			SubscriberAccessCounters.VoiceMessageProtectionFailures,
			SubscriberAccessCounters.VoiceMessageDecryptionFailures,
			SubscriberAccessCounters.AverageSentVoiceMessageSize,
			SubscriberAccessCounters.AverageRecentSentVoiceMessageSize,
			SubscriberAccessCounters.VoiceMessagesDeleted,
			SubscriberAccessCounters.ReplyMessagesSent,
			SubscriberAccessCounters.ForwardMessagesSent,
			SubscriberAccessCounters.EmailMessageQueueAccessed,
			SubscriberAccessCounters.EmailMessagesHeard,
			SubscriberAccessCounters.EmailMessagesDeleted,
			SubscriberAccessCounters.CalendarAccessed,
			SubscriberAccessCounters.CalendarItemsHeard,
			SubscriberAccessCounters.CalendarLateAttendance,
			SubscriberAccessCounters.CalendarItemsDetailsRequested,
			SubscriberAccessCounters.MeetingsDeclined,
			SubscriberAccessCounters.MeetingsAccepted,
			SubscriberAccessCounters.CalledMeetingOrganizer,
			SubscriberAccessCounters.RepliedToOrganizer,
			SubscriberAccessCounters.ContactsAccessed,
			SubscriberAccessCounters.ContactItemsHeard,
			SubscriberAccessCounters.CallsDisconnectedByCallersDuringUMAudioHourglass,
			SubscriberAccessCounters.LaunchedCalls,
			SubscriberAccessCounters.DirectoryAccessed,
			SubscriberAccessCounters.DirectoryAccessedByExtension,
			SubscriberAccessCounters.DirectoryAccessedByDialByName,
			SubscriberAccessCounters.DirectoryAccessedSuccessfullyByDialByName,
			SubscriberAccessCounters.DirectoryAccessedBySpokenName,
			SubscriberAccessCounters.DirectoryAccessedSuccessfullyBySpokenName,
			SubscriberAccessCounters.CallsDisconnectedOnIrrecoverableExternalError
		};
	}
}
