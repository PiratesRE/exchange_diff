using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class WellKnownPropertySet
	{
		public static readonly Guid MAPI = WellKnownNamedPropertyGuid.MAPI;

		public static readonly Guid Meeting = WellKnownNamedPropertyGuid.Meeting;

		public static readonly Guid Appointment = WellKnownNamedPropertyGuid.Appointment;

		public static readonly Guid Task = WellKnownNamedPropertyGuid.Task;

		public static readonly Guid Common = WellKnownNamedPropertyGuid.Common;

		public static readonly Guid PublicStrings = WellKnownNamedPropertyGuid.PublicStrings;

		public static readonly Guid LinkedFolder = WellKnownNamedPropertyGuid.LinkedFolder;

		public static readonly Guid Address = WellKnownNamedPropertyGuid.Address;

		public static readonly Guid InternetHeaders = WellKnownNamedPropertyGuid.InternetHeaders;

		public static readonly Guid Messaging = WellKnownNamedPropertyGuid.Messaging;

		public static readonly Guid CalendarAssistant = WellKnownNamedPropertyGuid.CalendarAssistant;

		public static readonly Guid UnifiedMessaging = WellKnownNamedPropertyGuid.UnifiedMessaging;

		public static readonly Guid AirSync = WellKnownNamedPropertyGuid.AirSync;

		public static readonly Guid Attachment = WellKnownNamedPropertyGuid.Attachment;

		public static readonly Guid Location = WellKnownNamedPropertyGuid.Location;

		public static readonly Guid Sharing = WellKnownNamedPropertyGuid.Sharing;

		public static readonly Guid Elc = WellKnownNamedPropertyGuid.Elc;

		public static readonly Guid Conversations = WellKnownNamedPropertyGuid.Conversations;

		public static readonly Guid ExternalSharing = WellKnownNamedPropertyGuid.ExternalSharing;

		public static readonly Guid Tracking = WellKnownNamedPropertyGuid.Tracking;

		public static readonly Guid IMAPMsg = WellKnownNamedPropertyGuid.IMAPMsg;

		public static readonly Guid Inference = WellKnownNamedPropertyGuid.Inference;

		public static readonly Guid PICW = WellKnownNamedPropertyGuid.PICW;

		public static readonly Guid UnifiedContactStore = WellKnownNamedPropertyGuid.UnifiedContactStore;

		public static readonly Guid Search = WellKnownNamedPropertyGuid.Search;

		public static readonly Guid Remote = WellKnownNamedPropertyGuid.Remote;

		public static readonly Guid PushNotificationSubscription = WellKnownNamedPropertyGuid.PushNotificationSubscription;

		public static readonly Guid GroupNotifications = WellKnownNamedPropertyGuid.GroupNotifications;

		public static readonly Guid Reminders = WellKnownNamedPropertyGuid.Reminders;

		public static readonly Guid Compliance = WellKnownNamedPropertyGuid.Compliance;

		public static readonly Guid OutlookService = WellKnownNamedPropertyGuid.OutlookService;

		public static readonly Guid WorkingSet = WellKnownNamedPropertyGuid.WorkingSet;

		public static readonly Guid UnifiedPolicy = WellKnownNamedPropertyGuid.UnifiedPolicy;

		public static readonly Guid ConsumerCalendar = WellKnownNamedPropertyGuid.ConsumerCalendar;
	}
}
