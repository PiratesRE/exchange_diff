using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OlcRuleCategoryRestriction : OlcRuleRestrictionBase
	{
		[DataMember]
		public int CategoryInt { get; set; }

		public OlcMessageCategory Category
		{
			get
			{
				return (OlcMessageCategory)this.CategoryInt;
			}
			set
			{
				this.CategoryInt = (int)value;
			}
		}
	}
}
