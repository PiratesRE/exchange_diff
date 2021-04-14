using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class IsPredicate : PredicateCondition
	{
		public IsPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
			if (!base.Property.IsString || !base.Value.IsString)
			{
				throw new RulesValidationException(RulesStrings.StringPropertyOrValueRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "is";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				if (string.CompareOrdinal(base.Property.Name, "Message.AttachmentTypes") == 0 || string.CompareOrdinal(base.Property.Name, "Message.AttachmentExtensions") == 0)
				{
					return IsPredicate.AttachmentPropertiesVersion;
				}
				return base.MinimumVersion;
			}
		}

		public override bool Evaluate(RulesEvaluationContext context)
		{
			object value = base.Property.GetValue(context);
			object value2 = base.Value.GetValue(context);
			List<string> list = new List<string>();
			bool flag = RuleUtils.CompareStringValues(value2, value, CaseInsensitiveStringComparer.Instance, base.EvaluationMode, list);
			base.UpdateEvaluationHistory(context, flag, list, 0);
			return flag;
		}

		public static readonly Version AttachmentPropertiesVersion = new Version("15.00.0001.001");
	}
}
