using System;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public sealed class PsContentDateToPredicate : PsSinglePropertyPredicate<DateTime>
	{
		protected override string PropertyNameForEnginePredicate
		{
			get
			{
				return "Item.WhenCreated";
			}
		}

		public PsContentDateToPredicate(DateTime contentDate) : base(contentDate)
		{
		}

		internal static PsContentDateToPredicate FromEnginePredicate(LessThanOrEqualPredicate condition)
		{
			return new PsContentDateToPredicate((DateTime)condition.Value.ParsedValue);
		}
	}
}
