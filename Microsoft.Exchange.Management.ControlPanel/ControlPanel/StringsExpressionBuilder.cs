using System;
using System.CodeDom;
using System.Web.Compilation;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ExpressionPrefix("Strings")]
	[ExpressionEditor(typeof(StringsExpressionEditor))]
	internal class StringsExpressionBuilder : ExpressionBuilder
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
			StringsExpressionEditorSheet stringsExpressionEditorSheet = (StringsExpressionEditorSheet)parsedData;
			return stringsExpressionEditorSheet.Evaluate();
		}

		public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
		{
			StringsExpressionEditorSheet stringsExpressionEditorSheet = (StringsExpressionEditorSheet)parsedData;
			return new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(stringsExpressionEditorSheet.Group.StringsType), stringsExpressionEditorSheet.StringID), "ToString", new CodeExpression[0]);
		}

		public override object ParseExpression(string expression, Type propertyType, ExpressionBuilderContext context)
		{
			return new StringsExpressionEditorSheet(expression, null);
		}
	}
}
