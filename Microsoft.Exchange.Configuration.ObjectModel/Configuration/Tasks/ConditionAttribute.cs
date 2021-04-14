using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal abstract class ConditionAttribute : Attribute
	{
		public ConditionAttribute.EvaluationType EvaluationStrategy
		{
			get
			{
				return this.evaluationStrategy;
			}
			set
			{
				this.evaluationStrategy = value;
			}
		}

		public bool ExpectedResult
		{
			get
			{
				return this.expectedResult;
			}
			set
			{
				this.expectedResult = value;
			}
		}

		public LocalizedString FailureDescription
		{
			get
			{
				return this.failureDescription;
			}
			set
			{
				this.failureDescription = value;
			}
		}

		private ConditionAttribute.EvaluationType evaluationStrategy = ConditionAttribute.EvaluationType.Contextual;

		private bool expectedResult = true;

		private LocalizedString failureDescription = Strings.GenericConditionFailure;

		public enum EvaluationType
		{
			Global,
			Local,
			Contextual
		}
	}
}
