using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetPopSubscriptionRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public SetPopSubscriptionData PopSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("SetPopSubscriptionRequest: {0}", this.PopSubscription);
		}
	}
}
