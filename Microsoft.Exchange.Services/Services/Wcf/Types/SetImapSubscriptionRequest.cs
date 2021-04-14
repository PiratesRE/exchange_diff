using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetImapSubscriptionRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public SetImapSubscriptionData ImapSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("SetImapSubscriptionRequest: {0}", this.ImapSubscription);
		}
	}
}
