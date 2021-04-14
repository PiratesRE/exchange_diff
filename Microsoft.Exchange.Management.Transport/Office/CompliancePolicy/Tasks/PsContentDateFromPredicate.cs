using System;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public sealed class PsContentDateFromPredicate : PsSinglePropertyPredicate<DateTime>
	{
		protected override string PropertyNameForEnginePredicate
		{
			get
			{
				return "Item.WhenCreated";
			}
		}

		public PsContentDateFromPredicate(DateTime contentDate) : base(contentDate)
		{
		}

		internal static PsContentDateFromPredicate FromEnginePredicate(GreaterThanOrEqualPredicate condition)
		{
			return new PsContentDateFromPredicate((DateTime)condition.Value.ParsedValue);
		}
	}
}
