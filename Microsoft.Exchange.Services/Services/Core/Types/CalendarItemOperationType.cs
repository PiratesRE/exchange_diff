using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	public static class CalendarItemOperationType
	{
		public const string SendCalendarInvitationsOrCancellations = "SendCalendarInvitationsOrCancellations";

		public const string SendCalendarInvitations = "SendCalendarInvitations";

		public const string SendCalendarCancellations = "SendCalendarCancellations";

		[XmlType("CalendarItemCreateOrDeleteOperationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public enum CreateOrDelete
		{
			SendToNone,
			SendOnlyToAll,
			SendToAllAndSaveCopy
		}

		[XmlType("CalendarItemUpdateOperationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public enum Update
		{
			SendToNone,
			SendOnlyToAll,
			SendOnlyToChanged,
			SendToAllAndSaveCopy,
			SendToChangedAndSaveCopy
		}
	}
}
