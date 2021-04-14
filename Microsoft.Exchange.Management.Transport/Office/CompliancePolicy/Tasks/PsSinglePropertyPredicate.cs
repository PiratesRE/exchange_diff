using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public abstract class PsSinglePropertyPredicate<T> : PsComplianceRulePredicateBase
	{
		protected abstract string PropertyNameForEnginePredicate { get; }

		public T PropertyValue { get; protected set; }

		protected PsSinglePropertyPredicate(T value)
		{
			this.PropertyValue = value;
		}

		internal override PredicateCondition ToEnginePredicate()
		{
			Property property = new Property(this.PropertyNameForEnginePredicate, typeof(T));
			List<string> list = new List<string>();
			List<string> list2 = list;
			T propertyValue = this.PropertyValue;
			list2.Add(propertyValue.ToString());
			return new EqualPredicate(property, list);
		}
	}
}
