using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class LambdaSeparatorRule : LambdaExpressionRule
	{
		protected override List<string> OnValidate(string lambdaExpression, PageConfigurableProfile profile)
		{
			List<string> list = new List<string>();
			string[] array = lambdaExpression.Split(new string[]
			{
				"=>"
			}, StringSplitOptions.None);
			if (array.Length == 1)
			{
				list.Add(string.Format("{0} separator is required for lambda expression {1}", "=>", lambdaExpression));
			}
			else if (array.Length > 2)
			{
				list.Add(string.Format("Multiply {0} separators are found in lambda expression {1}", "=>", lambdaExpression));
			}
			return list;
		}
	}
}
