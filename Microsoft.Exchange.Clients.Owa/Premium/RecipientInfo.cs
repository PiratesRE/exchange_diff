using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventStruct("Rcp")]
	internal sealed class RecipientInfo
	{
		public bool ToParticipant(out Participant participant)
		{
			return Utilities.CreateExchangeParticipant(out participant, this.DisplayName, this.RoutingAddress, this.RoutingType, this.AddressOrigin, this.StoreObjectId, this.EmailAddressIndex);
		}

		public ProxyAddress ToProxyAddress()
		{
			string addressString = this.RoutingAddress ?? string.Empty;
			string prefixString = this.RoutingType ?? string.Empty;
			return ProxyAddress.Parse(prefixString, addressString);
		}

		public const string StructNamespace = "Rcp";

		public const string RoutingAddressName = "EM";

		public const string DisplayNameName = "DN";

		public const string RoutingTypeName = "RT";

		public const string AddressOriginName = "AO";

		public const string PendingChunkName = "PND";

		public const string StoreObjectIdName = "ID";

		public const string RecipientFlagsName = "RF";

		public const string EmailAddressIndexName = "EI";

		[OwaEventField("EM", true, "")]
		public string RoutingAddress;

		[OwaEventField("DN", true, null)]
		public string DisplayName;

		[OwaEventField("RT", true, null)]
		public string RoutingType;

		[OwaEventField("AO", true, null)]
		public AddressOrigin AddressOrigin;

		[OwaEventField("PND", true, null)]
		public string PendingChunk;

		[OwaEventField("ID", true, null)]
		public StoreObjectId StoreObjectId;

		[OwaEventField("RF", true, null)]
		public int RecipientFlags;

		[OwaEventField("EI", true, null)]
		public EmailAddressIndex EmailAddressIndex;
	}
}
