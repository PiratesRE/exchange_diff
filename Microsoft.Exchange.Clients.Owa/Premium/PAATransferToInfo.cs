using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("pxti")]
	internal sealed class PAATransferToInfo
	{
		public const string StructNamespace = "pxti";

		public const string DescriptionName = "desc";

		public const string KeyName = "k";

		public const string PhoneName = "ph";

		public const string ContactName = "rcp";

		public const string DirectlyToVoiceMailName = "VM";

		[OwaEventField("desc", true, 0)]
		public string Desc;

		[OwaEventField("k", false, 0)]
		public int Key;

		[OwaEventField("ph", true, "")]
		public string Ph;

		[OwaEventField("rcp", true, null)]
		public string Contact;

		[OwaEventField("VM", true, true)]
		public bool VM;
	}
}
