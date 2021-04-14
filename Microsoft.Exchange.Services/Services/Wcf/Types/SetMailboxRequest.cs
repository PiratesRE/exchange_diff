using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetMailboxRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public MailboxOptions Mailbox { get; set; }

		public override string ToString()
		{
			return string.Format("SetMailboxRequest: Mailbox={0}", this.Mailbox);
		}
	}
}
