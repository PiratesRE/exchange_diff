using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventObjectId(typeof(ADObjectId))]
	[OwaEventStruct("ADVLVS")]
	public class DirectoryVirtualListViewState : VirtualListViewState
	{
		public const string StructNamespace = "ADVLVS";

		public const string CookieName = "cki";

		public const string CookieIndexName = "ckii";

		public const string CookieLcidName = "clcid";

		public const string PreferredDCName = "cPfdDC";

		[OwaEventField("cki", true, null)]
		public string Cookie;

		[OwaEventField("ckii", true, 0)]
		public int CookieIndex;

		[OwaEventField("clcid", true, -1)]
		public int CookieLcid;

		[OwaEventField("cPfdDC", true, null)]
		public string PreferredDC;
	}
}
