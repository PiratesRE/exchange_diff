using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetHotmailSubscriptionResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public HotmailSubscription HotmailSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("GetHotmailSubscriptionResponse: {0}", this.HotmailSubscription);
		}
	}
}
