using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	[Serializable]
	internal class InvalidSenderException : StoragePermanentException
	{
		public InvalidSenderException(Participant participant) : base(LocalizedString.Empty)
		{
			if (participant != null)
			{
				this.address = participant.EmailAddress;
				this.addressType = participant.RoutingType;
			}
		}

		public InvalidSenderException(ProxyAddress proxyAddress) : base(LocalizedString.Empty)
		{
			if (proxyAddress != null)
			{
				this.address = proxyAddress.AddressString;
				this.addressType = proxyAddress.PrefixString;
			}
		}

		public string Address
		{
			get
			{
				return this.address;
			}
		}

		public string AddressType
		{
			get
			{
				return this.addressType;
			}
		}

		private readonly string address;

		private readonly string addressType;
	}
}
