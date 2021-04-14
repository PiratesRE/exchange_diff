using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class SinglePropertyContainsPredicate : ContainsWordsPredicate
	{
		protected SinglePropertyContainsPredicate(string internalPropertyName)
		{
			this.internalPropertyName = internalPropertyName;
		}

		internal static TransportRulePredicate CreateFromInternalCondition<T>(Condition condition, string internalPropertyName) where T : SinglePropertyContainsPredicate, new()
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals("contains") || !predicateCondition.Property.Name.Equals(internalPropertyName))
			{
				return null;
			}
			Word[] array = new Word[predicateCondition.Value.RawValues.Count];
			for (int i = 0; i < predicateCondition.Value.RawValues.Count; i++)
			{
				try
				{
					array[i] = new Word(predicateCondition.Value.RawValues[i]);
				}
				catch (ArgumentOutOfRangeException)
				{
					return null;
				}
			}
			T t = Activator.CreateInstance<T>();
			t.Words = array;
			return t;
		}

		internal virtual Property CreateProperty()
		{
			return TransportRuleParser.Instance.CreateProperty(this.internalPropertyName);
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (Word word in this.Words)
			{
				shortList.Add(word.ToString());
			}
			return TransportRuleParser.Instance.CreatePredicate("contains", this.CreateProperty(), shortList);
		}

		private readonly string internalPropertyName;
	}
}
