using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Data_StorageTags
	{
		public const int Storage = 0;

		public const int Interop = 1;

		public const int MeetingMessage = 2;

		public const int Event = 3;

		public const int Dispose = 4;

		public const int ServiceDiscovery = 5;

		public const int Context = 6;

		public const int ContextShadow = 7;

		public const int CcGeneric = 8;

		public const int CcOle = 9;

		public const int CcBody = 10;

		public const int CcInboundGeneric = 11;

		public const int CcInboundMime = 12;

		public const int CcInboundTnef = 13;

		public const int CcOutboundGeneric = 14;

		public const int CcOutboundMime = 15;

		public const int CcOutboundTnef = 16;

		public const int CcPFD = 17;

		public const int Session = 18;

		public const int DefaultFolders = 19;

		public const int UserConfiguration = 20;

		public const int PropertyBag = 21;

		public const int Task = 22;

		public const int Recurrence = 23;

		public const int WorkHours = 24;

		public const int Sync = 25;

		public const int ICal = 26;

		public const int ActiveManagerClient = 27;

		public const int CcOutboundVCard = 28;

		public const int CcInboundVCard = 29;

		public const int Sharing = 30;

		public const int RightsManagement = 31;

		public const int DatabaseAvailabilityGroup = 32;

		public const int FaultInjection = 33;

		public const int SmtpService = 34;

		public const int MapiConnectivity = 35;

		public const int Xtc = 36;

		public const int CalendarLogging = 38;

		public const int CalendarSeries = 39;

		public const int BirthdayCalendar = 40;

		public const int PropertyMapping = 50;

		public const int MdbResourceHealth = 51;

		public const int ContactLinking = 52;

		public const int UserPhotos = 53;

		public const int ContactFoldersEnumerator = 54;

		public const int MyContactsFolder = 55;

		public const int Aggregation = 56;

		public const int OutlookSocialConnectorInterop = 57;

		public const int Person = 58;

		public const int DatabasePinger = 60;

		public const int ContactsEnumerator = 62;

		public const int ContactChangeLogging = 63;

		public const int ContactExporter = 64;

		public const int SiteMailboxPermissionCheck = 70;

		public const int SiteMailboxDocumentSync = 71;

		public const int SiteMailboxMembershipSync = 72;

		public const int SiteMailboxClientOperation = 73;

		public const int SiteMailboxMessageDedup = 74;

		public const int Reminders = 81;

		public const int PeopleIKnow = 82;

		public const int AggregatedConversations = 83;

		public const int Delegate = 84;

		public const int GroupMailboxSession = 85;

		public const int SyncProcess = 86;

		public const int Conversation = 87;

		public const int ReliableTimer = 88;

		public const int FavoritePublicFolders = 89;

		public const int PublicFolders = 90;

		public static Guid guid = new Guid("6d031d1d-5908-457a-a6c4-cdd0f6e74d81");
	}
}
