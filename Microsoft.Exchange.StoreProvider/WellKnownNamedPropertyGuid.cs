using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class WellKnownNamedPropertyGuid
	{
		public static readonly Guid InternetHeaders = new Guid("00020386-0000-0000-C000-000000000046");

		public static readonly Guid MAPI = new Guid("00020328-0000-0000-C000-000000000046");

		public static readonly Guid PublicStrings = new Guid("00020329-0000-0000-C000-000000000046");

		public static readonly Guid IConverterSession = new Guid("4E3A7680-B77A-11D0-9DA5-00C04FD65685");

		public static readonly Guid PkmCharacterization = new Guid("560c36c0-503a-11cf-baa1-00004c752a9a");

		public static readonly Guid PkmDocSummaryInformation = new Guid("d5cdd502-2e9c-101b-9397-08002b2cf9ae");

		public static readonly Guid PkmGatherer = new Guid("0b63e343-9ccc-11d0-bcdb-00805fccce04");

		public static readonly Guid PkmHTMLInformation = new Guid("70eb7a10-55d9-11cf-b75b-00aa0051fe20");

		public static readonly Guid PkmIndexServerQuery = new Guid("49691c90-7e17-101a-a91c-08002b2ecda9");

		public static readonly Guid PkmLinkInformation = new Guid("c82bf597-b831-11d0-b733-00aa00a1ebd2");

		public static readonly Guid PkmMetaInformation = new Guid("d1b5d3f0-c0b3-11cf-9a92-00a0c908dbf1");

		public static readonly Guid PkmNetLibraryInfo = new Guid("c82bf596-b831-11d0-b733-00aa00a1ebd2");

		public static readonly Guid PkmScriptInfo = new Guid("31f400a0-fd07-11cf-b9bd-00aa003db18e");

		public static readonly Guid Search = new Guid("0b63e350-9ccc-11d0-bcdb-00805fccce04");

		public static readonly Guid PkmSummaryInformation = new Guid("f29f85e0-4ff9-1068-ab91-08002b27b3d9");

		public static readonly Guid Address = new Guid("00062004-0000-0000-C000-000000000046");

		public static readonly Guid AirSync = new Guid("71035549-0739-4DCB-9163-00F0580DBBDF");

		public static readonly Guid Appointment = new Guid("00062002-0000-0000-C000-000000000046");

		public static readonly Guid Attachment = new Guid("96357F7F-59E1-47D0-99A7-46515C183B54");

		public static readonly Guid CalendarAssistant = new Guid("11000E07-B51B-40D6-AF21-CAA85EDAB1D0");

		public static readonly Guid Common = new Guid("00062008-0000-0000-C000-000000000046");

		public static readonly Guid Conversations = new Guid("33eba41f-7aa8-422e-be7b-79e1a98e54b3");

		public static readonly Guid DAV = new Guid("29F3aB60-554D-11D0-A97C-00A0C911F50A");

		public static readonly Guid Elc = new Guid("C7A4569B-F7AE-4DC2-9279-A8FE2F3CAF89");

		public static readonly Guid ExternalSharing = new Guid("F52A8693-C34D-4980-9E20-9D4C1EABB6A7");

		public static readonly Guid IMAPFold = new Guid("29f3AB52-554D-11D0-A97C-00A0C911F50A");

		public static readonly Guid IMAPMsg = new Guid("29F3AB53-554D-11D0-A97C-00A0C911F50A");

		public static readonly Guid IMAPStore = new Guid("29F3AB55-554D-11D0-A97C-00A0C911F50A");

		public static readonly Guid InboxFolderLazyStream = new Guid("94FAEF10-F947-11D0-800E-0000C90DC8DB");

		public static readonly Guid Location = new Guid("A719E259-2A9A-4FB8-BAB3-3A9F02970E4B");

		public static readonly Guid Log = new Guid("0006200A-0000-0000-C000-000000000046");

		public static readonly Guid Meeting = new Guid("6ED8DA90-450B-101B-98DA-00AA003F1305");

		public static readonly Guid Messaging = new Guid("41F28F13-83F4-4114-A584-EEDB5A6B0BFF");

		public static readonly Guid Note = new Guid("00062008-000E-0000-C000-000000000046");

		public static readonly Guid PostRss = new Guid("00062041-0000-0000-C000-000000000046");

		public static readonly Guid Proxy = new Guid("29f3AB56-554D-11D0-A97C-00A0C911F50A");

		public static readonly Guid Remote = new Guid("00062014-0000-0000-C000-000000000046");

		public static readonly Guid Report = new Guid("00062013-0000-0000-C000-000000000046");

		public static readonly Guid Sharing = new Guid("00062040-0000-0000-C000-000000000046");

		public static readonly Guid Task = new Guid("00062003-0000-0000-C000-000000000046");

		public static readonly Guid Tracking = new Guid("0006200B-0000-0000-C000-000000000046");

		public static readonly Guid LinkedFolder = new Guid("E1226F08-3D31-441C-86F2-2A5757AF056A");

		public static readonly Guid UnifiedMessaging = new Guid("4442858E-A9E3-4E80-B900-317A210CC15B");

		public static readonly Guid Storage = new Guid("B725F130-47EF-101A-A5F1-02608C9EEBAC");

		public static readonly Guid Drm = new Guid("AEAC19E4-89AE-4508-B9B7-BB867ABEE2ED");

		public static readonly Guid Inference = new Guid("23239608-685D-4732-9C55-4C95CB4E8E33");

		public static readonly Guid PICW = new Guid("CAF1337F-3C60-47C6-B8F9-3B50113D046B");

		public static readonly Guid UnifiedContactStore = new Guid("A523DF5E-3405-48F0-A410-ABBC6205381D");

		public static readonly Guid PushNotificationSubscription = new Guid("BB8D823E-582D-4A68-B1FB-180B32E3B53E");

		public static readonly Guid GroupNotifications = new Guid("4D240CD1-F947-44EE-8F8A-B0E5FF29C18A");

		public static readonly Guid Reminders = new Guid("1A15A70E-6248-4CBA-9194-92AA60304A35");

		public static readonly Guid Compliance = new Guid("403FC56B-CD30-47C5-86F8-EDE9E35A022B");

		public static readonly Guid OutlookService = new Guid("FF023B15-B696-475E-8ACF-DBCD47C1C735");

		public static readonly Guid WorkingSet = new Guid("95A4668D-CFBE-4D15-B4AE-3E61B9EF078B");

		public static readonly Guid UnifiedPolicy = new Guid("bd1c30fe-2a2d-4494-8fbb-986467b48c55");

		public static readonly Guid ConsumerCalendar = new Guid("58B6F260-0251-4293-9737-2EF23187F89D");
	}
}
