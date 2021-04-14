using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class NotExistsPredicate : ExistsPredicate
	{
		public NotExistsPredicate(Property property, List<string> entries) : base(property, entries)
		{
		}

		public override string Name
		{
			get
			{
				return "notExists";
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			return !base.Evaluate(context);
		}
	}
}
