using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetSubscriptionResponse : OptionsResponseBase
	{
		public GetSubscriptionResponse()
		{
			this.SubscriptionCollection = new SubscriptionCollection();
		}

		[DataMember(IsRequired = true)]
		public SubscriptionCollection SubscriptionCollection { get; set; }

		public override string ToString()
		{
			return string.Format("GetSubscriptionResponse: {0}", this.SubscriptionCollection);
		}
	}
}
