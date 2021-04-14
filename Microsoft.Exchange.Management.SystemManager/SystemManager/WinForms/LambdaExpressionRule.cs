using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal abstract class LambdaExpressionRule : IDDIValidator
	{
		public List<string> Validate(object target, PageConfigurableProfile profile)
		{
			string text = target as string;
			if (string.IsNullOrEmpty(text))
			{
				return new List<string>();
			}
			return this.OnValidate(text, profile);
		}

		protected abstract List<string> OnValidate(string lambdaExpression, PageConfigurableProfile profile);
	}
}
