using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OlcMessageStringPropertyComparisionRestriction : OlcRuleRestrictionBase
	{
		[DataMember]
		public uint? LCID { get; set; }

		[DataMember]
		public int PropertyInt { get; set; }

		public OlcMessageProperty Property
		{
			get
			{
				return (OlcMessageProperty)this.PropertyInt;
			}
			set
			{
				this.PropertyInt = (int)value;
			}
		}

		[DataMember]
		public int ConditionInt { get; set; }

		public OlcStringComparison Condition
		{
			get
			{
				return (OlcStringComparison)this.ConditionInt;
			}
			set
			{
				this.ConditionInt = (int)value;
			}
		}

		[DataMember]
		public string Value { get; set; }
	}
}
