using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RemoveConnectSubscriptionRequest : BaseJsonRequest
	{
		[DataMember]
		public Identity Identity { get; set; }

		public override string ToString()
		{
			return string.Format("RemoveConnectSubscriptionRequest: {0}", this.Identity);
		}
	}
}
