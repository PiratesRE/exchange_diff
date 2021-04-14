using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(OlcRuleActionRemoveCategory))]
	[DataContract]
	[KnownType(typeof(OlcRuleActionAssignCategory))]
	internal abstract class OlcRuleCategoryActionBase : OlcRuleActionBase
	{
		[DataMember]
		public ushort CategoryId { get; set; }

		public OlcMessageCategory SystemCategory
		{
			get
			{
				return (OlcMessageCategory)this.CategoryId;
			}
			set
			{
				this.CategoryId = (ushort)value;
			}
		}

		[DataMember]
		public bool IsUserCategory { get; set; }
	}
}
