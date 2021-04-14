using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class LambdaExpressionCompilableRule : LambdaExpressionRule
	{
		protected override List<string> OnValidate(string lambdaExpression, PageConfigurableProfile profile)
		{
			List<string> list = new List<string>();
			try
			{
				ColumnExpression expression = ExpressionCalculator.BuildColumnExpression(lambdaExpression);
				DataTable dataTable = new DataTable();
				DataRow dataRow = dataTable.NewRow();
				ExpressionCalculator.CompileLambdaExpression(expression, typeof(object), dataRow, null);
			}
			catch (Exception ex)
			{
				list.Add(string.Format("Fail to compile lambda expression {0} with error {1}", lambdaExpression, ex.ToString()));
			}
			return list;
		}
	}
}
