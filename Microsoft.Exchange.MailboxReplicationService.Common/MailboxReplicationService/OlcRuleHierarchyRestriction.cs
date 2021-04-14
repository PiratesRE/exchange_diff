using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(OlcRuleAndRestriction))]
	[KnownType(typeof(OlcRuleOrRestriction))]
	[DataContract]
	internal abstract class OlcRuleHierarchyRestriction : OlcRuleRestrictionBase
	{
		[DataMember]
		public OlcRuleRestrictionBase[] Conditions { get; set; }
	}
}
