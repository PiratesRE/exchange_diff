using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal abstract class BasicPostconditionAttribute : ConditionAttribute
	{
		public BasicPostconditionAttribute()
		{
		}

		public BasicPostconditionAttribute(ConditionAttribute.EvaluationType evaluationStrategy)
		{
			base.EvaluationStrategy = evaluationStrategy;
		}
	}
}
