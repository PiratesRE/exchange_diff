using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class MaxRuleID : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public long? RuleID
		{
			get
			{
				return (long?)this[MaxRuleID.RuleIDProperty];
			}
			set
			{
				this[MaxRuleID.RuleIDProperty] = value;
			}
		}

		public RuleType? RuleType
		{
			get
			{
				return (RuleType?)this[MaxRuleID.RuleIDProperty];
			}
			set
			{
				this[MaxRuleID.RuleIDProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition RuleIDProperty = new HygienePropertyDefinition("bi_RuleId", typeof(long?));

		public static readonly HygienePropertyDefinition RuleTypeProperty = new HygienePropertyDefinition("ti_RuleType", typeof(byte?));
	}
}
