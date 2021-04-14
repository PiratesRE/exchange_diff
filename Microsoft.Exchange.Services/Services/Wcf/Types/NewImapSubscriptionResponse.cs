using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class NewImapSubscriptionResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public ImapSubscription ImapSubscription { get; set; }

		public override string ToString()
		{
			return string.Format("NewImapSubscriptionResponse: {0}", this.ImapSubscription);
		}
	}
}
