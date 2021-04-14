using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetConnectSubscriptionResponse : OptionsResponseBase
	{
		public GetConnectSubscriptionResponse()
		{
			this.ConnectSubscriptionCollection = new ConnectSubscriptionCollection();
		}

		[DataMember]
		public ConnectSubscriptionCollection ConnectSubscriptionCollection { get; set; }

		public override string ToString()
		{
			return string.Format("GetConnectSubscriptionResponse: {0}", this.ConnectSubscriptionCollection);
		}
	}
}
