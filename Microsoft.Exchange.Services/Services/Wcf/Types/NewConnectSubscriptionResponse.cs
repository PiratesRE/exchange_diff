using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewConnectSubscriptionResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public ConnectSubscription ConnectSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("NewConnectSubscriptionResponse: {0}", this.ConnectSubscription);
		}
	}
}
