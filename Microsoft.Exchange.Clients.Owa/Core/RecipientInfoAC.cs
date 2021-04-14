using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[OwaEventStruct("recip")]
	internal sealed class RecipientInfoAC
	{
		[OwaEventField("dn", false, null)]
		public string DisplayName;

		[OwaEventField("sa", false, "")]
		public string SmtpAddress;

		[OwaEventField("ra", false, "")]
		public string RoutingAddress;

		[OwaEventField("al", false, null)]
		public string Alias;

		[OwaEventField("rt", false, null)]
		public string RoutingType;

		[OwaEventField("ao", false, null)]
		public AddressOrigin AddressOrigin;

		[OwaEventField("rf", false, null)]
		public int RecipientFlags;

		[OwaEventField("id", false, null)]
		public string ItemId;

		[OwaEventField("ei", false, null)]
		public EmailAddressIndex EmailAddressIndex;

		[OwaEventField("uri", true, null)]
		public string SipUri;

		[OwaEventField("mo", true, null)]
		public string MobilePhoneNumber;
	}
}
