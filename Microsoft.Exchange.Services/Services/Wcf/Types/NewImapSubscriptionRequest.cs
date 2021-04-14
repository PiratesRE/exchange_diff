using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewImapSubscriptionRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public NewImapSubscriptionData ImapSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("NewImapSubscriptionRequest: {0}", this.ImapSubscription);
		}
	}
}
