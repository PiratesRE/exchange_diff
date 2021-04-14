using System;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal class CachedValidationRuleResult
	{
		public CachedValidationRuleResult(string ruleName, bool evaluationResult)
		{
			this.ruleName = ruleName;
			this.result = evaluationResult;
		}

		public string RuleName
		{
			get
			{
				return this.ruleName;
			}
		}

		public bool EvaluationResult
		{
			get
			{
				return this.result;
			}
		}

		private string ruleName;

		private bool result;
	}
}
