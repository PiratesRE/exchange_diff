using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class URIRule : RuleBase
	{
		public URIRule()
		{
			base.RuleType = new byte?(1);
		}

		public int Score
		{
			get
			{
				return (int)this[URIRule.ScoreProperty];
			}
			set
			{
				this[URIRule.ScoreProperty] = value;
			}
		}

		public string URI
		{
			get
			{
				return (string)this[URIRule.URIProperty];
			}
			set
			{
				this[URIRule.URIProperty] = value;
			}
		}

		public int? URITypeID
		{
			get
			{
				return (int?)this[URIRule.URITypeIDProperty];
			}
			set
			{
				this[URIRule.URITypeIDProperty] = value;
			}
		}

		public bool Overridable
		{
			get
			{
				return (bool)this[URIRule.OverridableProperty];
			}
			set
			{
				this[URIRule.OverridableProperty] = value;
			}
		}

		public static readonly HygienePropertyDefinition ScoreProperty = new HygienePropertyDefinition("Score", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition URIProperty = new HygienePropertyDefinition("Uri", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition URITypeIDProperty = new HygienePropertyDefinition("UriTypeId", typeof(int?));

		public static readonly HygienePropertyDefinition OverridableProperty = new HygienePropertyDefinition("Overridable", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
