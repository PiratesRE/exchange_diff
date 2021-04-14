using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("pcii")]
	internal sealed class PAACallerIdInfo
	{
		public const string StructNamespace = "pcii";

		public const string HasPhones = "fph";

		public const string HasCnts = "fcnt";

		public const string IsInCntFolder = "fld";

		[OwaEventField("fph", true, false)]
		public bool HasPhoneNumbers;

		[OwaEventField("fcnt", true, false)]
		public bool HasContacts;

		[OwaEventField("fld", true, false)]
		public bool IsInContactFolder;
	}
}
