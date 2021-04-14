using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetConnectSubscriptionRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public SetConnectSubscriptionData ConnectSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("SetConnectSubscriptionRequest: {0}", this.ConnectSubscription);
		}
	}
}
