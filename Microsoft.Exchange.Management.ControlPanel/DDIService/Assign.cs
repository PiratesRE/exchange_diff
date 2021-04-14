using System;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class Assign : Activity
	{
		public Assign()
		{
		}

		protected Assign(Assign activity) : base(activity)
		{
			this.Variable = activity.Variable;
			this.Value = activity.Value;
		}

		public override Activity Clone()
		{
			return new Assign(this);
		}

		[DDIMandatoryValue]
		[DDIVariableNameExist]
		public string Variable { get; set; }

		[DDIMandatoryValue]
		[DDIValidLambdaExpression]
		public object Value { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult result = new RunResult();
			string text = this.Value as string;
			if (DDIHelper.IsLambdaExpression(text))
			{
				dataTable.Rows[0][this.Variable] = ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(text), typeof(object), DDIHelper.GetLambdaExpressionDataRow(dataTable), input);
			}
			else
			{
				dataTable.Rows[0][this.Variable] = this.Value;
			}
			return result;
		}
	}
}
