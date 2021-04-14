using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("ItmInf")]
	internal sealed class DeleteItemInfo
	{
		public const string StructNamespace = "ItmInf";

		public const string StoreObjectIdName = "ID";

		public const string IsMeetingMessageName = "MM";

		[OwaEventField("ID", false, null)]
		public OwaStoreObjectId OwaStoreObjectId;

		[OwaEventField("MM", true, false)]
		public bool IsMeetingMessage;
	}
}
