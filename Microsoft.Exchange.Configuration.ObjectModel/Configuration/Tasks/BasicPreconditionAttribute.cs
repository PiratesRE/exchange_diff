using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal abstract class BasicPreconditionAttribute : ConditionAttribute
	{
		public BasicPreconditionAttribute()
		{
		}

		public BasicPreconditionAttribute(ConditionAttribute.EvaluationType evaluationStrategy)
		{
			base.EvaluationStrategy = evaluationStrategy;
		}

		public Type Resolver
		{
			get
			{
				return this.resolver;
			}
			set
			{
				this.resolver = value;
			}
		}

		private Type resolver;
	}
}
