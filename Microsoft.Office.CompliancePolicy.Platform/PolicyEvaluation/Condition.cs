using System;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public abstract class Condition
	{
		public static TrueCondition True
		{
			get
			{
				return Condition.trueCondition;
			}
		}

		public static FalseCondition False
		{
			get
			{
				return Condition.falseCondition;
			}
		}

		public ConditionEvaluationMode EvaluationMode
		{
			get
			{
				return this.evaluationMode;
			}
			set
			{
				this.evaluationMode = value;
			}
		}

		public abstract ConditionType ConditionType { get; }

		public virtual Version MinimumVersion
		{
			get
			{
				return Condition.BaseVersion;
			}
		}

		public abstract bool Evaluate(PolicyEvaluationContext context);

		public virtual void GetSupplementalData(SupplementalData data)
		{
		}

		public static readonly Version BaseVersion = new Version("1.00.0000.000");

		private static readonly TrueCondition trueCondition = new TrueCondition();

		private static readonly FalseCondition falseCondition = new FalseCondition();

		private ConditionEvaluationMode evaluationMode = ConditionEvaluationMode.Optimized;
	}
}
