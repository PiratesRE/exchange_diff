using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class AuditOperationsPredicate : PredicateCondition
	{
		public AuditOperationsPredicate(List<string> operations) : this(new Property("NotUsed", typeof(string)), operations)
		{
		}

		internal AuditOperationsPredicate(Property property, List<string> operations) : base(property, operations)
		{
		}

		public override string Name
		{
			get
			{
				return "auditOperations";
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
			return false;
		}
	}
}
