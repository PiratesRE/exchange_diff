using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ValidatingTaskResult
	{
		public ValidatingTaskResult()
		{
			this.conditionDescription = null;
			this.result = ValidatingTaskResult.ResultType.NotRun;
			this.failureDetails = null;
		}

		public string ConditionDescription
		{
			get
			{
				return this.conditionDescription;
			}
			set
			{
				this.conditionDescription = value;
			}
		}

		public ValidatingTaskResult.ResultType Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		public Exception FailureDetails
		{
			get
			{
				return this.failureDetails;
			}
			set
			{
				this.failureDetails = value;
			}
		}

		private string conditionDescription;

		private ValidatingTaskResult.ResultType result;

		private Exception failureDetails;

		public enum ResultType
		{
			Passed,
			Failed,
			NotRun
		}
	}
}
