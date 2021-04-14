using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ConnectedAccountInfo
	{
		[DataMember(IsRequired = true, Order = 1)]
		public Guid SubscriptionGuid { get; set; }

		[DataMember(IsRequired = true, Order = 2)]
		public string EmailAddress { get; set; }

		[DataMember(IsRequired = true, Order = 3)]
		public string DisplayName { get; set; }
	}
}
