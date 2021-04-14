using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamRule : RuleBase
	{
		public SpamRule()
		{
			base.RuleType = new byte?(0);
		}

		public string ConditionMatchPhrase
		{
			get
			{
				return (string)this[SpamRule.ConditionMatchPhraseProperty];
			}
			set
			{
				this[SpamRule.ConditionMatchPhraseProperty] = value;
			}
		}

		public string ConditionNotMatchPhrase
		{
			get
			{
				return (string)this[SpamRule.ConditionNotMatchPhraseProperty];
			}
			set
			{
				this[SpamRule.ConditionNotMatchPhraseProperty] = value;
			}
		}

		public int? AsfID
		{
			get
			{
				return (int?)this[SpamRule.AsfIDProperty];
			}
			set
			{
				this[SpamRule.AsfIDProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition ConditionMatchPhraseProperty = new HygienePropertyDefinition("nvc_ConditionMatchPhrase", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ConditionNotMatchPhraseProperty = new HygienePropertyDefinition("nvc_ConditionNotMatchPhrase", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition AsfIDProperty = new HygienePropertyDefinition("AsfId", typeof(int?));
	}
}
