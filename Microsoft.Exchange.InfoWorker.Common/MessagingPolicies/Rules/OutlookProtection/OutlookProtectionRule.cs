using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection
{
	internal sealed class OutlookProtectionRule : Rule
	{
		public OutlookProtectionRule(string name) : base(name)
		{
			this.userOverridable = false;
		}

		public bool UserOverridable
		{
			get
			{
				return this.userOverridable;
			}
			set
			{
				this.userOverridable = value;
			}
		}

		private static Action FindActionByName(IEnumerable<Action> actions, string name)
		{
			if (actions == null)
			{
				return null;
			}
			foreach (Action action in actions)
			{
				if (string.Equals(action.Name, name, StringComparison.Ordinal))
				{
					return action;
				}
			}
			return null;
		}

		private static PredicateCondition FindPredicateConditionByName(Condition root, string name)
		{
			if (root == null)
			{
				return null;
			}
			ConditionType conditionType = root.ConditionType;
			if (conditionType != ConditionType.And)
			{
				switch (conditionType)
				{
				case ConditionType.True:
					return null;
				case ConditionType.Predicate:
				{
					PredicateCondition predicateCondition = (PredicateCondition)root;
					if (string.Equals(predicateCondition.Name, name, StringComparison.Ordinal))
					{
						return predicateCondition;
					}
					return null;
				}
				}
				return null;
			}
			AndCondition andCondition = (AndCondition)root;
			foreach (Condition root2 in andCondition.SubConditions)
			{
				PredicateCondition predicateCondition2 = OutlookProtectionRule.FindPredicateConditionByName(root2, name);
				if (predicateCondition2 != null)
				{
					return predicateCondition2;
				}
			}
			return null;
		}

		public RightsProtectMessageAction GetRightsProtectMessageAction()
		{
			return (RightsProtectMessageAction)this.FindActionByName("RightsProtectMessage");
		}

		public PredicateCondition GetAllInternalPredicate()
		{
			return this.FindPredicateConditionByName("allInternal");
		}

		public PredicateCondition GetRecipientIsPredicate()
		{
			return this.FindPredicateConditionByName("recipientIs");
		}

		public PredicateCondition GetSenderDepartmentPredicate()
		{
			return this.FindPredicateConditionByName("is");
		}

		private PredicateCondition FindPredicateConditionByName(string name)
		{
			return OutlookProtectionRule.FindPredicateConditionByName(base.Condition, name);
		}

		private Action FindActionByName(string name)
		{
			return OutlookProtectionRule.FindActionByName(base.Actions, name);
		}

		private bool userOverridable;
	}
}
