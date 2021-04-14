using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class SinglePropertyMatchesPredicate : MatchesPatternsPredicate
	{
		protected SinglePropertyMatchesPredicate(string internalPropertyName, bool isLegacyRegex)
		{
			base.UseLegacyRegex = isLegacyRegex;
			this.internalPropertyName = internalPropertyName;
		}

		internal static TransportRulePredicate CreateFromInternalCondition<T>(Condition condition, string internalPropertyName) where T : SinglePropertyMatchesPredicate, new()
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if ((!predicateCondition.Name.Equals("matches") && !predicateCondition.Name.Equals("matchesRegex")) || !predicateCondition.Property.Name.Equals(internalPropertyName))
			{
				return null;
			}
			bool useLegacyRegex = !predicateCondition.Name.Equals("matchesRegex");
			Pattern[] array = new Pattern[predicateCondition.Value.RawValues.Count];
			for (int i = 0; i < predicateCondition.Value.RawValues.Count; i++)
			{
				try
				{
					array[i] = new Pattern(predicateCondition.Value.RawValues[i], useLegacyRegex, false);
				}
				catch (ArgumentException)
				{
					return null;
				}
			}
			T t = Activator.CreateInstance<T>();
			t.UseLegacyRegex = useLegacyRegex;
			t.patterns = array;
			return t;
		}

		internal virtual Property CreateProperty()
		{
			return TransportRuleParser.Instance.CreateProperty(this.internalPropertyName);
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (Pattern pattern in this.patterns)
			{
				shortList.Add(pattern.ToString());
			}
			string name = base.UseLegacyRegex ? "matches" : "matchesRegex";
			return TransportRuleParser.Instance.CreatePredicate(name, this.CreateProperty(), shortList);
		}

		private readonly string internalPropertyName;
	}
}
