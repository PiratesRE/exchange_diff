using System;

namespace Microsoft.Exchange.Services.Core
{
	internal static class ItemClass
	{
		public const string CalendarItem = "IPM.Appointment";

		public const string CalendarItemOccurrenceException = "IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}";

		public const string Contact = "IPM.Contact";

		public const string DistributionList = "IPM.DistList";

		public const string InformationRightsManagementMessage = "rpmsg.message";

		public const string MeetingForwardNotification = "IPM.Schedule.Meeting.Notification.Forward";

		public const string Meeting = "IPM.Schedule.Meeting";

		public const string MeetingCancellation = "IPM.Schedule.Meeting.Canceled";

		public const string MeetingRequest = "IPM.Schedule.Meeting.Request";

		public const string MeetingResponse = "IPM.Schedule.Meeting.Resp.";

		public const string Message = "IPM.Note";

		public const string Task = "IPM.Task";

		public const string ReportPrefix = "REPORT.IPM.Note.";

		public const string DeliveryReportSuffix = ".DR";

		public const string NonDeliveryReportSuffix = ".NDR";

		public const string ReadReceiptSuffix = ".IPNRN";

		public const string NonReadReceiptSuffix = ".IPNNRN";

		public const string PostItem = "IPM.Post";

		public const string BuddyListOtherContacts = "IPM.DistList.MOC.OtherContacts";

		public const string UnifiedContactStoreTaggedContacts = "IPM.DistList.MOC.Tagged";

		public const string UnifiedContactStoreOtherContacts = "IPM.DistList.MOC.OtherContacts";

		public const string UnifiedContactsStoreUserGroup = "IPM.DistList.MOC.UserGroup";

		public const string UnifiedContactsStoreDistributionGroup = "IPM.DistList.MOC.DG";

		public const string UnifiedContactsStoreFavorites = "IPM.DistList.MOC.Favorites";
	}
}
