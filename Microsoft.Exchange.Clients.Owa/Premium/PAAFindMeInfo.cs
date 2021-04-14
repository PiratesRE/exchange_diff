using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("pfmi")]
	internal sealed class PAAFindMeInfo
	{
		public const string StructNamespace = "pfmi";

		public const string DescriptionName = "desc";

		public const string KeyName = "k";

		public const string Phone1Name = "ph1";

		public const string Timeout1Name = "tm1";

		public const string Phone2Name = "ph2";

		public const string Timeout2Name = "tm2";

		[OwaEventField("desc", true, 0)]
		public string Desc;

		[OwaEventField("k", false, 0)]
		public int Key;

		[OwaEventField("ph1", true, "")]
		public string Ph1;

		[OwaEventField("tm1", true, 5)]
		public int Tm1;

		[OwaEventField("ph2", true, "")]
		public string Ph2;

		[OwaEventField("tm2", true, 5)]
		public int Tm2;
	}
}
