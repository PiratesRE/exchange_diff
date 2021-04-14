using System;
using System.Web.UI.Design;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class StringsExpressionEditor : ExpressionEditor
	{
		public override object EvaluateExpression(string expression, object parseTimeData, Type propertyType, IServiceProvider serviceProvider)
		{
			return new StringsExpressionEditorSheet(expression, serviceProvider).Evaluate();
		}

		public override ExpressionEditorSheet GetExpressionEditorSheet(string expression, IServiceProvider serviceProvider)
		{
			return new StringsExpressionEditorSheet(expression, serviceProvider);
		}
	}
}
