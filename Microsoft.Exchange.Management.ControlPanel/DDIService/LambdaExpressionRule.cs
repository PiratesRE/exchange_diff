using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	internal abstract class LambdaExpressionRule : IDDIValidator
	{
		public List<string> Validate(object target, Service profile)
		{
			string text = target as string;
			if (string.IsNullOrEmpty(text))
			{
				return new List<string>();
			}
			return this.OnValidate(text, profile);
		}

		protected abstract List<string> OnValidate(string lambdaExpression, Service profile);
	}
}
