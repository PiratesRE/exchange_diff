using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class NameValuesPairConfigurationPredicate : PredicateCondition
	{
		internal NameValuesPairConfigurationPredicate(Property property, List<string> values) : base(property, values)
		{
			if ((base.Property.Type != typeof(string) && !base.Property.IsCollectionOfType(typeof(string))) || (base.Value.Type != typeof(string) && !base.Value.IsCollectionOfType(typeof(string))))
			{
				throw new CompliancePolicyValidationException(string.Format("Predicate '{0}' requires string property and value", this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "NameValuesPairConfiguration";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return new Version("1.00.0001.000");
			}
		}

		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.Predicate;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			throw new CompliancePolicyException("Evaluate is not support on NameValuesPairConfigurationPredicate!");
		}
	}
}
