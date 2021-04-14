using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class QueryPredicate : Condition
	{
		public QueryPredicate(Condition subcondition)
		{
			this.SubCondition = subcondition;
		}

		public virtual string Name
		{
			get
			{
				return "queryMatch";
			}
		}

		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.DynamicQuery;
			}
		}

		public Condition SubCondition { get; set; }

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			return true;
		}

		public override void GetSupplementalData(SupplementalData data)
		{
			this.SubCondition.GetSupplementalData(data);
		}

		public virtual string BuildQuery()
		{
			return string.Empty;
		}
	}
}
