using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("rmInfo")]
	internal sealed class ReminderInfo
	{
		public const string StructNamespace = "rmInfo";

		public const string ItemIdName = "id";

		public const string ChangeKeyName = "ck";

		[OwaEventField("id", false, "")]
		public StoreObjectId ItemId;

		[OwaEventField("ck", false, "")]
		public string ChangeKey;
	}
}
