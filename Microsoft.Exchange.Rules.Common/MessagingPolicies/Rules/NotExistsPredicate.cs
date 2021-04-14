using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class NotExistsPredicate : ExistsPredicate
	{
		public NotExistsPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "notExists";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return new Version("2.7");
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			object value = base.Property.GetValue(context);
			if (value == null)
			{
				return true;
			}
			IEnumerable<string> enumerable = value as IEnumerable<string>;
			if (enumerable != null)
			{
				bool flag = !enumerable.Any<string>();
				base.UpdateEvaluationHistory(context, flag, new List<string>(), 0);
				return flag;
			}
			base.UpdateEvaluationHistory(context, true, null, 0);
			return false;
		}
	}
}
