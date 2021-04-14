using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
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
				return Rule.BaseVersion;
			}
		}

		public abstract bool Evaluate(RulesEvaluationContext context);

		public virtual void GetSupplementalData(SupplementalData data)
		{
		}

		private static readonly TrueCondition trueCondition = new TrueCondition();

		private static readonly FalseCondition falseCondition = new FalseCondition();

		private ConditionEvaluationMode evaluationMode = ConditionEvaluationMode.Optimized;
	}
}
