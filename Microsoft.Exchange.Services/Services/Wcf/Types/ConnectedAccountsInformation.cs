using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ConnectedAccountsInformation : OptionsPropertyChangeTracker
	{
		[DataMember]
		public Identity DefaultReplyAddress { get; set; }

		[DataMember]
		public SendAddressData[] SendAddresses { get; set; }

		[DataMember]
		public Subscription[] Subscriptions { get; set; }
	}
}
