using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetHotmailSubscriptionRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public SetHotmailSubscriptionData HotmailSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("SetHotmailSubscriptionRequest: {0}", this.HotmailSubscription);
		}
	}
}
