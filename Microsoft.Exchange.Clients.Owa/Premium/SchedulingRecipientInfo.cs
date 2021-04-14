using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("SRcp")]
	public sealed class SchedulingRecipientInfo
	{
		public const string StructNamespace = "SRcp";

		public const string IDName = "ID";

		public const string EmailAddressName = "EM";

		public const string RoutingTypeName = "RT";

		public const string DisplayNameName = "DN";

		public const string GetFreeBusyDataName = "FB";

		public const string AttendeeTypeName = "AT";

		[OwaEventField("ID", false, "")]
		public string ID;

		[OwaEventField("EM", false, "")]
		public string EmailAddress;

		[OwaEventField("RT", false, "")]
		public string RoutingType;

		[OwaEventField("DN", true, null)]
		public string DisplayName;

		[OwaEventField("FB", true, false)]
		public bool GetFreeBusyData;

		[OwaEventField("AT", false, MeetingAttendeeType.Required)]
		public MeetingAttendeeType AttendeeType;
	}
}
