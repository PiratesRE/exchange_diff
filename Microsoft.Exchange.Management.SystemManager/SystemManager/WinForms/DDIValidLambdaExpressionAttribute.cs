using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIValidLambdaExpressionAttribute : DDIValidateAttribute
	{
		public DDIValidLambdaExpressionAttribute() : base("DDIValidLambdaExpressionAttribute")
		{
		}

		public override List<string> Validate(object target, PageConfigurableProfile profile)
		{
			if (target != null && !(target is string))
			{
				throw new ArgumentException("DDIValidLambdaExpressionAttribute can only apply to String property");
			}
			List<string> list = new List<string>();
			foreach (LambdaExpressionRule lambdaExpressionRule in DDIValidLambdaExpressionAttribute.rules)
			{
				list.AddRange(lambdaExpressionRule.Validate(target, profile));
				if (list.Count > 0)
				{
					break;
				}
			}
			return list;
		}

		private static List<LambdaExpressionRule> rules = new List<LambdaExpressionRule>
		{
			new LambdaSeparatorRule(),
			new DependentColumnExistRule(),
			new LambdaExpressionCompilableRule()
		};
	}
}
