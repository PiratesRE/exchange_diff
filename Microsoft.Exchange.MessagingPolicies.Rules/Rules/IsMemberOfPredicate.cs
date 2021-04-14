using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class IsMemberOfPredicate : PredicateCondition
	{
		public IsMemberOfPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
			if (!base.Property.IsString)
			{
				throw new RulesValidationException(RulesStrings.StringPropertyOrValueRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "isMemberOf";
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			BaseTransportRulesEvaluationContext baseTransportRulesEvaluationContext = (BaseTransportRulesEvaluationContext)baseContext;
			if (baseTransportRulesEvaluationContext == null)
			{
				throw new ArgumentException("context is either null or not of type: BaseTransportRulesEvaluationContext");
			}
			baseTransportRulesEvaluationContext.PredicateName = this.Name;
			object value = base.Property.GetValue(baseTransportRulesEvaluationContext);
			object value2 = base.Value.GetValue(baseTransportRulesEvaluationContext);
			if (value == null || baseTransportRulesEvaluationContext.MembershipChecker == null)
			{
				return false;
			}
			List<string> list = new List<string>();
			bool flag = RuleUtils.CompareStringValues(value2, value, baseTransportRulesEvaluationContext.MembershipChecker, base.EvaluationMode, list);
			base.UpdateEvaluationHistory(baseContext, flag, list, 0);
			return flag;
		}
	}
}
