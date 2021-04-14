using System;
using System.CodeDom;
using System.Web.Compilation;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RegexExpressionBuilder : ExpressionBuilder
	{
		public override bool SupportsEvaluate
		{
			get
			{
				return true;
			}
		}

		public override object EvaluateExpression(object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
		{
			return CommonRegex.GetRegexExpressionById(entry.Expression.Trim()).ToString();
		}

		public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
		{
			return new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(CommonRegex)), entry.Expression.Trim()), "ToString", new CodeExpression[0]);
		}
	}
}
