using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal struct ReceiveRequestKeys
	{
		internal const string ItemId = "ItemId";

		internal const string NeedToReclassify = "NeedToReclassify";

		internal const string BodyChanged = "BodyChanged";

		internal const string Recipients = "Recipients";

		internal const string CustomizedStringsNeeded = "CustomizedStringsNeeded";

		internal const string EventTrigger = "EventTrigger";

		internal const string InvalidRecipients = "InvalidRecipients";

		internal const string Ping = "Ping";

		internal const string ItemAlreadyBeingProcessed = "ItemAlreadyBeingProcessed";

		internal const string ClientSupportsScanResultData = "ClientSupportsScanResultData";

		internal const string ScanResultData = "ScanResultData";
	}
}
