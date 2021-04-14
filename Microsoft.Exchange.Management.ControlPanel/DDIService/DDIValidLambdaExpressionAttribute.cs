using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DDIValidLambdaExpressionAttribute : DDIValidateAttribute
	{
		public DDIValidLambdaExpressionAttribute() : base("DDIValidLambdaExpressionAttribute")
		{
		}

		public override List<string> Validate(object target, Service profile)
		{
			List<string> list = new List<string>();
			string value = target as string;
			if (DDIHelper.IsLambdaExpression(value))
			{
				foreach (LambdaExpressionRule lambdaExpressionRule in DDIValidLambdaExpressionAttribute.rules)
				{
					list.AddRange(lambdaExpressionRule.Validate(target, profile));
					if (list.Count > 0)
					{
						break;
					}
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
