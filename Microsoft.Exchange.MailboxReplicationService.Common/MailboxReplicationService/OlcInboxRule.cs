using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OlcInboxRule
	{
		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public uint ExecutionSequence { get; set; }

		[DataMember]
		public OlcRuleRestrictionBase Condition { get; set; }

		[DataMember]
		public OlcRuleActionBase[] Actions { get; set; }
	}
}
