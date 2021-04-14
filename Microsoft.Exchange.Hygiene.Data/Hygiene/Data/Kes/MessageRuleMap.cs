using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Kes
{
	internal class MessageRuleMap : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.FileGuid.ToString());
			}
		}

		public Guid FileGuid
		{
			get
			{
				return (Guid)this[MessageRuleMap.FileGuidProperty];
			}
			set
			{
				this[MessageRuleMap.FileGuidProperty] = value;
			}
		}

		public long RuleID
		{
			get
			{
				return (long)this[MessageRuleMap.RuleIDProperty];
			}
			set
			{
				this[MessageRuleMap.RuleIDProperty] = value;
			}
		}

		public byte ProcessingGroup
		{
			get
			{
				return (byte)this[MessageRuleMap.ProcessingGroupProperty];
			}
			set
			{
				this[MessageRuleMap.ProcessingGroupProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition FileGuidProperty = new HygienePropertyDefinition("id_FileGuid", typeof(Guid));

		public static readonly HygienePropertyDefinition RuleIDProperty = new HygienePropertyDefinition("bi_RuleId", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ProcessingGroupProperty = new HygienePropertyDefinition("ti_ProcessingGroup", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
