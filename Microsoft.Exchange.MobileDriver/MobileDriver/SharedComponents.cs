using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal static class SharedComponents
	{
		public const string InternalDeliveryAgentSignature = "Agent:TextMessagingInternalDelivery-86DB88E6-E880-4564-B1EC-25C9797FEBBE";

		public const string ExternalDeliveryAgentSignature = "Agent:TextMessagingExternalDelivery-803AF8EC-8F1B-42b3-854D-8CA8E8CB04FC";

		public const string XheaderTextMessagingMapiClass = "X-MS-Exchange-Organization-Text-Messaging-Mapi-Class";

		public const string XheaderTextMessagingOriginator = "X-MS-Exchange-Organization-Text-Messaging-Originator";

		public const string XheaderTextMessagingCountOfSettingsSegments = "X-MS-Exchange-Organization-Text-Messaging-Count-Of-Settings-Segments";

		public const string XheaderTextMessagingSettingsSegmentPrefix = "X-MS-Exchange-Organization-Text-Messaging-Settings-Segment-";

		public const string XheaderTextMessagingTimestamp = "X-MS-Exchange-Organization-Text-Messaging-Timestamp";

		public const string XheaderTextMessagingNotificationPreferredCulture = "X-MS-Exchange-Organization-Text-Messaging-Notification-PreferredCulture";

		public const string TimestampFormat = "yyyyMMddhhmmssfff";

		public const string IpmNoteMobileMms = "IPM.Note.Mobile.MMS";
	}
}
