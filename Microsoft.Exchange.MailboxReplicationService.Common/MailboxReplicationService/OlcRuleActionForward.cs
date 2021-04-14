using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OlcRuleActionForward : OlcRuleActionBase
	{
		[DataMember]
		public string ForwardingEmailAddress { get; set; }
	}
}
