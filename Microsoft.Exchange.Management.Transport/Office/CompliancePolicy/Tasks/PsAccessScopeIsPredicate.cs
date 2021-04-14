using System;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public sealed class PsAccessScopeIsPredicate : PsSinglePropertyPredicate<AccessScope>
	{
		protected override string PropertyNameForEnginePredicate
		{
			get
			{
				return "Item.AccessScope";
			}
		}

		public PsAccessScopeIsPredicate(AccessScope accessScope) : base(accessScope)
		{
		}

		public static PsAccessScopeIsPredicate FromEnginePredicate(EqualPredicate condition)
		{
			return new PsAccessScopeIsPredicate((AccessScope)condition.Value.ParsedValue);
		}
	}
}
