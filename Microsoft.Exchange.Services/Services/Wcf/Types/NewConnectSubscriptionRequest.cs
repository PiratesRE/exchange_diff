using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewConnectSubscriptionRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public NewConnectSubscriptionData ConnectSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("NewConnectSubscriptionRequest: {0}", this.ConnectSubscription);
		}
	}
}
