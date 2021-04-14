using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewPopSubscriptionRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public NewPopSubscriptionData PopSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("NewPopSubscriptionRequest: {0}", this.PopSubscription);
		}
	}
}
