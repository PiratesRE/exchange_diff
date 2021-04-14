using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OlcRuleNotRestriction : OlcRuleRestrictionBase
	{
		[DataMember]
		public OlcRuleRestrictionBase Condition { get; set; }
	}
}
