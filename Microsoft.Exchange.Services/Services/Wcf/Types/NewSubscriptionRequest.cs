using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[KnownType(typeof(Subscription))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewSubscriptionRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public NewSubscriptionData Subscription { get; set; }

		public override string ToString()
		{
			return string.Format("NewSubscriptionRequest: {0}", this.Subscription);
		}
	}
}
